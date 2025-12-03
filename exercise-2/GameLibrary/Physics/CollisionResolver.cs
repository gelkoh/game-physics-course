using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

public class CollisionResolver
{
    public static void ResolveCollisions(List<CollisionInfo> collisions)
    {
        foreach (CollisionInfo info in collisions)
        {
            RigidBody bodyA = info.ColliderA?.RigidBody;
            RigidBody bodyB = info.ColliderB?.RigidBody;

            float invMassA = (bodyA != null && bodyA.Mass > 0f) ? 1f / bodyA.Mass : 0f;
            float invMassB = (bodyB != null && bodyB.Mass > 0f) ? 1f / bodyB.Mass : 0f;

            if (invMassA + invMassB == 0f) continue;

            float totalInvMass = invMassA + invMassB;
            Vector2 correction = info.MTV;

            if (bodyA != null)
            {
                bodyA.GameObject.Position += correction * (invMassA / totalInvMass);
            }
            if (bodyB != null)
            {
                bodyB.GameObject.Position -= correction * (invMassB / totalInvMass);
            }

            Vector2 n = info.Normal;

            float elasticity = info.ColliderA.Elasticity * info.ColliderB.Elasticity;

            Vector2 relativeVelocity = (bodyA?.Velocity ?? Vector2.Zero) - (bodyB?.Velocity ?? Vector2.Zero);
            float separatingVelocity = Vector2.Dot(relativeVelocity, n);

            if (separatingVelocity > 0f) continue;

            float numerator = -(1f + elasticity) * separatingVelocity;
            float denominator = totalInvMass;
            float j = numerator / denominator;

            Vector2 impulse = j * n;

            if (bodyA != null)
            {
                bodyA.Velocity += impulse * invMassA;
            }
            if (bodyB != null)
            {
                bodyB.Velocity -= impulse * invMassB;
            }
            
            info.ColliderA?.TriggerCollision();
            info.ColliderB?.TriggerCollision();
        }
    }
}