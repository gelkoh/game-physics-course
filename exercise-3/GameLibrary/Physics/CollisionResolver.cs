using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

public class CollisionResolver
{
    public static void ResolveCollisions(List<CollisionInfo> collisions, float deltaTime, Vector2 gravity)
    {
        foreach (CollisionInfo info in collisions)
        {
            RigidBody bodyA = info.ColliderA?.RigidBody;
            RigidBody bodyB = info.ColliderB?.RigidBody;

            float invMassA = (bodyA != null) ? bodyA.InverseMass : 0f;
            float invMassB = (bodyB != null) ? bodyB.InverseMass : 0f;
            float invInertiaA = (bodyA != null) ? bodyA.InverseInertia : 0f;
            float invInertiaB = (bodyB != null) ? bodyB.InverseInertia : 0f;

            if (invMassA + invMassB == 0f) continue;

            // Position correction
            if (Math.Abs(info.MTV.LengthSquared()) > 0.0001f) 
            {
                float penetration = info.MTV.Length();
                Vector2 correction = info.Normal * Math.Max(penetration - 0.01f, 0f) * 0.8f;
                
                float totalInvMass = invMassA + invMassB;
                if (bodyA != null) bodyA.GameObject.Position -= correction * (invMassA / totalInvMass);
                if (bodyB != null) bodyB.GameObject.Position += correction * (invMassB / totalInvMass);
            }

            Vector2 n = info.Normal;
            
            Vector2 contactPoint = (info.ContactPoints != null && info.ContactPoints.Length > 0) 
                                   ? info.ContactPoints[0] 
                                   : (info.ColliderA.Position + info.ColliderB.Position) / 2f;

            Vector2 rA = contactPoint - (bodyA?.GameObject.Position ?? Vector2.Zero);
            Vector2 rB = contactPoint - (bodyB?.GameObject.Position ?? Vector2.Zero);

            Vector2 vA = (bodyA?.Velocity ?? Vector2.Zero);
            Vector2 vB = (bodyB?.Velocity ?? Vector2.Zero);
            
            if (bodyA != null) vA += new Vector2(-rA.Y, rA.X) * bodyA.AngularVelocity;
            if (bodyB != null) vB += new Vector2(-rB.Y, rB.X) * bodyB.AngularVelocity;
            
            Vector2 relativeVelocity = vA - vB;

            float velocityAlongNormal = Vector2.Dot(relativeVelocity, n);

            // We stop here if objects are moving away from each other
            if (velocityAlongNormal < 0) continue;

            // Subtract the velocity coming from the gravity as a measurement against micro collisions
            Vector2 gravityA = (bodyA != null && bodyA.InverseMass > 0) ? gravity : Vector2.Zero;
            Vector2 gravityB = (bodyB != null && bodyB.InverseMass > 0) ? gravity : Vector2.Zero;
            Vector2 relativeAcceleration = gravityA - gravityB;
            
            float velocityFromGravity = Vector2.Dot(relativeAcceleration, n) * deltaTime;
            
            float realVelocityAlongNormal = velocityAlongNormal - velocityFromGravity;

            float e = (info.ColliderA.Elasticity + info.ColliderB.Elasticity) / 2f;
            
            if (Math.Abs(realVelocityAlongNormal) < 120f)
            {
                e = 0f;
            }

            float numerator = -(1 + e) * realVelocityAlongNormal;
            
            float rA_cross_n = rA.X * n.Y - rA.Y * n.X;
            float rB_cross_n = rB.X * n.Y - rB.Y * n.X;
            
            float k =
                invMassA + invMassB +
                rA_cross_n * rA_cross_n * invInertiaA +
                rB_cross_n * rB_cross_n * invInertiaB;
            
            float termA = (bodyA != null) ? (rA_cross_n * rA_cross_n) * invInertiaA : 0f;
            float termB = (bodyB != null) ? (rB_cross_n * rB_cross_n) * invInertiaB : 0f;
            float denominator = invMassA + invMassB + termA + termB;

            if (denominator == 0f) continue;

            float j = numerator / denominator;
            Vector2 impulse = j * n;

            // Apply impulse
            if (bodyA != null)
            {
                bodyA.Velocity += impulse * invMassA;
                
                float torqueA = rA.X * impulse.Y - rA.Y * impulse.X;
                bodyA.AngularVelocity += torqueA * invInertiaA;
            }
            if (bodyB != null)
            {
                bodyB.Velocity -= impulse * invMassB;
                
                float torqueB = rB.X * impulse.Y - rB.Y * impulse.X;
                bodyB.AngularVelocity -= torqueB * invInertiaB;
            }
            
            // Friction
            vA = (bodyA?.Velocity ?? Vector2.Zero);
            vB = (bodyB?.Velocity ?? Vector2.Zero);
            
            if (bodyA != null) vA += new Vector2(-rA.Y, rA.X) * bodyA.AngularVelocity;
            if (bodyB != null) vB += new Vector2(-rB.Y, rB.X) * bodyB.AngularVelocity;
            
            relativeVelocity = vA - vB;

            Vector2 tangent = relativeVelocity - Vector2.Dot(relativeVelocity, n) * n;
            
            if (tangent.LengthSquared() < 1e-6f) continue; 
            
            tangent.Normalize();

            float numeratorT = -Vector2.Dot(relativeVelocity, tangent);
            
            float rA_cross_t = rA.X * tangent.Y - rA.Y * tangent.X;
            float rB_cross_t = rB.X * tangent.Y - rB.Y * tangent.X;
            
            float termAT = (bodyA != null) ? (rA_cross_t * rA_cross_t) * invInertiaA : 0f;
            float termBT = (bodyB != null) ? (rB_cross_t * rB_cross_t) * invInertiaB : 0f;
            float denominatorT = invMassA + invMassB + termAT + termBT;

            if (denominatorT == 0f) continue;
            
            float jt = numeratorT / denominatorT;

            float muStatic = (info.ColliderA.StaticFriction + info.ColliderB.StaticFriction) * 0.5f;
            float muDynamic = (info.ColliderA.DynamicFriction + info.ColliderB.DynamicFriction) * 0.5f;

            Vector2 frictionImpulse;

            if (Math.Abs(jt) <= Math.Abs(j) * muStatic)
            {
                frictionImpulse = jt * tangent;
            }
            else
            {
                // Dynamic friction acts against opposite direction of movement
                frictionImpulse = j * muDynamic * tangent; 
            }
            
            // Apply friction impulse
            if (bodyA != null)
            {
                bodyA.Velocity += frictionImpulse * invMassA;
                float torqueA = rA.X * frictionImpulse.Y - rA.Y * frictionImpulse.X;
                bodyA.AngularVelocity += torqueA * invInertiaA;
            }
            if (bodyB != null)
            {
                bodyB.Velocity -= frictionImpulse * invMassB;
                float torqueB = rB.X * frictionImpulse.Y - rB.Y * frictionImpulse.X;
                bodyB.AngularVelocity -= torqueB * invInertiaB;
            }
        }
    }
}