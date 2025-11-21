using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

public static class CollisionChecker
{
    public static void CheckForCollisions(List<Collider> colliders)
    {
        foreach (Collider col in colliders)
        {
            // Only check collisions for rigid body
            if (col.RigidBody == null) continue;
            
            AABB aabb = col.GetAABB();

            foreach (Collider otherCol in colliders)
            {
                // Don't check collision with itself
                if (col == otherCol) continue;

                AABB otherAABB = otherCol.GetAABB();

                bool isIntersecting = AABB.intersects(aabb, otherAABB);
                
                if (isIntersecting)
                {
                    col.TriggerCollision();
                }
            }
        }
    }
}