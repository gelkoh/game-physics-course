using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

public static class CollisionResolver
{
    public static void ResolveCollisions(List<CollisionInfo> collisions, float deltaTime, Vector2 gravity)
    {
        // Iterative solver iterations >= number of contacts (lecture 7 slide 16)
        // We resolve the conflicts multiple times because resolving one contact can shift another
        int iterations = 4;
        Console.WriteLine(iterations);

        for (int i = 0; i < iterations; i++)
        {
            foreach (CollisionInfo info in collisions)
            {
                RigidBody bodyA = info.ColliderA?.RigidBody;
                RigidBody bodyB = info.ColliderB?.RigidBody;

                float invMassA = bodyA?.InverseMass ?? 0f;
                float invMassB = bodyB?.InverseMass ?? 0f;
                float invInertiaA = bodyA?.InverseInertia ?? 0f;
                float invInertiaB = bodyB?.InverseInertia ?? 0f;

                // If both objects have an infinite mass/are static, don't do anything
                if (invMassA + invMassB == 0f) continue;

                // Position correction with linear projection
                ApplyLinearProjection(info, bodyA, bodyB, invMassA, invMassB);

                // Impulse calculation, calculate physical impacts and friction
                ResolveContactConstraints(info, bodyA, bodyB, invMassA, invMassB, invInertiaA, invInertiaB, deltaTime, gravity);
            }
        }
    }

    private static void ApplyLinearProjection(CollisionInfo info, RigidBody bodyA, RigidBody bodyB, float invMassA, float invMassB)
    {
        float penetrationSq = info.MTV.LengthSquared();
    
        // Ignore tiny overlaps for performance reasons
        if (penetrationSq < 0.0001f) return;
    
        float penetration = (float)Math.Sqrt(penetrationSq);
    
        // Allow small penetration to avoid jitter and correct only part of the error per iteration, so that
        // objects don't shoot out too much and trigger more collisions
        float slop = 0.1f;
        float percent = 0.15f; 
    
        // Calculation of the correction vector along the normal
        Vector2 correction = info.Normal * Math.Max(penetration - slop, 0f) * percent;

        float totalInvMass = invMassA + invMassB;
    
        // Application of displacement weighted by mass
        if (bodyA != null) bodyA.GameObject.Position -= correction * (invMassA / totalInvMass);
        if (bodyB != null) bodyB.GameObject.Position += correction * (invMassB / totalInvMass);
    }
    
    private static void ResolveContactConstraints(
        CollisionInfo info, 
        RigidBody bodyA, RigidBody bodyB,
        float invMassA, float invMassB,
        float invInertiaA, float invInertiaB,
        float deltaTime, Vector2 gravity)
    {
        Vector2 n = info.Normal;
        
        // If clipping doesn't provide points, just use the center as a fallback
        Vector2[] contacts = (info.ContactPoints != null && info.ContactPoints.Length > 0) 
            ? info.ContactPoints 
            : new Vector2[] { (info.ColliderA.Position + info.ColliderB.Position) / 2f };

        // Impulse calculation for every contact point
        foreach (Vector2 contactPoint in contacts)
        {
            // Vectors from the center of mass to the contact point
            Vector2 rA = contactPoint - (bodyA?.GameObject.Position ?? Vector2.Zero);
            Vector2 rB = contactPoint - (bodyB?.GameObject.Position ?? Vector2.Zero);

            // Calculate normal impulse (the rebound)
            float normalImpulseMagnitude = ApplyNormalImpulse(
                info, bodyA, bodyB, n, rA, rB, 
                invMassA, invMassB, invInertiaA, invInertiaB, 
                deltaTime, gravity);

            // Calculate friction impulse
            ApplyFrictionImpulse(
                info, bodyA, bodyB, n, rA, rB, 
                invMassA, invMassB, invInertiaA, invInertiaB, 
                normalImpulseMagnitude);
        }
    }

    private static float ApplyNormalImpulse(
        CollisionInfo info, RigidBody bodyA, RigidBody bodyB, 
        Vector2 n, Vector2 rA, Vector2 rB,
        float invMassA, float invMassB, float invInertiaA, float invInertiaB,
        float deltaTime, Vector2 gravity)
    {
        // Get current velocity
        Vector2 vA = (bodyA?.Velocity ?? Vector2.Zero);
        Vector2 vB = (bodyB?.Velocity ?? Vector2.Zero);

        // Include angular velocity at this contact point
        if (bodyA != null) vA += new Vector2(-rA.Y, rA.X) * bodyA.AngularVelocity;
        if (bodyB != null) vB += new Vector2(-rB.Y, rB.X) * bodyB.AngularVelocity;

        // Determine relative velocity
        Vector2 relativeVelocity = vA - vB;
        float velocityAlongNormal = Vector2.Dot(relativeVelocity, n);
        
        // We break out of the method if objects are going into different directions instead of approaching each other
        if (velocityAlongNormal < 0) return 0f;
        
        // Factor out velocity due to gravity (to prevent microcollisions) (from lecture 7)
        Vector2 gravityA = (bodyA != null && bodyA.InverseMass > 0) ? gravity : Vector2.Zero;
        Vector2 gravityB = (bodyB != null && bodyB.InverseMass > 0) ? gravity : Vector2.Zero;
        float velocityFromGravity = Vector2.Dot(gravityA - gravityB, n) * deltaTime;
        
        float realVelocityAlongNormal = velocityAlongNormal - velocityFromGravity;

        // Elasticity
        float e = (info.ColliderA.Elasticity + info.ColliderB.Elasticity) / 2f;
        
        // Don't 'bounce' if relative velocity is really small (also to prevent microcollisions) (from lecture 7)
        if (Math.Abs(realVelocityAlongNormal) < 10f) e = 0f;

        // Impulse calculation math (with rotation)
        float numerator = -(1 + e) * realVelocityAlongNormal;
        
        float rA_cross_n = rA.X * n.Y - rA.Y * n.X;
        float rB_cross_n = rB.X * n.Y - rB.Y * n.X;

        float termA = (bodyA != null) ? (rA_cross_n * rA_cross_n) * invInertiaA : 0f;
        float termB = (bodyB != null) ? (rB_cross_n * rB_cross_n) * invInertiaB : 0f;
        float denominator = invMassA + invMassB + termA + termB;

        if (denominator == 0f) return 0f;

        // Impulse scalar J
        float j = numerator / denominator;
        Vector2 impulse = j * n;

        // Apply impulse (translation + rotation)
        if (bodyA != null)
        {
            bodyA.Velocity += impulse * invMassA;
            bodyA.AngularVelocity += (rA.X * impulse.Y - rA.Y * impulse.X) * invInertiaA;
        }
        if (bodyB != null)
        {
            bodyB.Velocity -= impulse * invMassB;
            bodyB.AngularVelocity -= (rB.X * impulse.Y - rB.Y * impulse.X) * invInertiaB;
        }

        return j;
    }

    private static void ApplyFrictionImpulse(
        CollisionInfo info, RigidBody bodyA, RigidBody bodyB,
        Vector2 n, Vector2 rA, Vector2 rB,
        float invMassA, float invMassB, float invInertiaA, float invInertiaB,
        float normalImpulseMagnitude)
    {
        // Recalculate velocities
        Vector2 vA = (bodyA?.Velocity ?? Vector2.Zero);
        Vector2 vB = (bodyB?.Velocity ?? Vector2.Zero);

        if (bodyA != null) vA += new Vector2(-rA.Y, rA.X) * bodyA.AngularVelocity;
        if (bodyB != null) vB += new Vector2(-rB.Y, rB.X) * bodyB.AngularVelocity;

        Vector2 relativeVelocity = vA - vB;

        // Calculate tangent (perpendicular to the normal, in the direction of movement)
        Vector2 tangent = relativeVelocity - Vector2.Dot(relativeVelocity, n) * n;

        // If there is almost no sideways movement, we don't need to calculate friction
        if (tangent.LengthSquared() < 1e-6f) return;

        tangent.Normalize();

        // Calculate friction impulse Jt
        // Identical formula as for normal momentum, but with tangent instead of normal
        float numeratorT = -Vector2.Dot(relativeVelocity, tangent);

        float rA_cross_t = rA.X * tangent.Y - rA.Y * tangent.X;
        float rB_cross_t = rB.X * tangent.Y - rB.Y * tangent.X;

        float termAT = (bodyA != null) ? (rA_cross_t * rA_cross_t) * invInertiaA : 0f;
        float termBT = (bodyB != null) ? (rB_cross_t * rB_cross_t) * invInertiaB : 0f;
        float denominatorT = invMassA + invMassB + termAT + termBT;

        if (denominatorT == 0f) return;

        float jt = numeratorT / denominatorT;

        // Apply coulomb's law
        float muStatic = (info.ColliderA.StaticFriction + info.ColliderB.StaticFriction) * 0.5f;
        float muDynamic = (info.ColliderA.DynamicFriction + info.ColliderB.DynamicFriction) * 0.5f;

        Vector2 frictionImpulse;

        // Static friction: If force < limit, we stop the movement completely
        if (Math.Abs(jt) <= Math.Abs(normalImpulseMagnitude) * muStatic)
        {
            frictionImpulse = jt * tangent;
        }
        else
        {
            // Dynamic sliding friction => we only brake with maximum sliding force
            frictionImpulse = normalImpulseMagnitude * muDynamic * tangent;
        }

        // Finally apply friction impulse
        if (bodyA != null)
        {
            bodyA.Velocity += frictionImpulse * invMassA;
            bodyA.AngularVelocity += (rA.X * frictionImpulse.Y - rA.Y * frictionImpulse.X) * invInertiaA;
        }
        if (bodyB != null)
        {
            bodyB.Velocity -= frictionImpulse * invMassB;
            bodyB.AngularVelocity -= (rB.X * frictionImpulse.Y - rB.Y * frictionImpulse.X) * invInertiaB;
        }
    }
}