using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

public static class CollisionChecker
{
    public static List<CollisionInfo> CheckForCollisions(List<Collider> colliders)
    {
        List<CollisionInfo> collisions = new List<CollisionInfo>();
    
        BroadPhaseCheck(colliders, collisions); 
    
        return collisions;
    }

    // Check for AABB collisions
    private static void BroadPhaseCheck(List<Collider> colliders, List<CollisionInfo> collisionList)
    {
        int n = colliders.Count;
        
        for (int i = 0; i < n; i++)
        {
            Collider colA = colliders[i];
            
            for (int j = i + 1; j < n; j++)
            {
                Collider colB = colliders[j];
                if (colA == colB) continue;

                if (colA.RigidBody == null && colB.RigidBody == null) continue;

                AABB a = colA.GetAABB();
                AABB b = colB.GetAABB();

                if (!AABB.intersects(a, b)) continue;

                CollisionInfo info = NarrowPhaseCheck(colA, colB);

                if (info.IsColliding)
                {
                    collisionList.Add(info);
                }
            }
        }
    }
    
    // Check for SAT collisions
    private static CollisionInfo NarrowPhaseCheck(Collider a, Collider b)
    {
        return (a, b) switch
        {
            (CircleCollider colA, BoxCollider colB) => CheckCircleBoxCollision(colA, colB),
            (BoxCollider colA, CircleCollider colB) => CheckCircleBoxCollision(colB, colA),
            (CircleCollider colA, CircleCollider colB) => CheckCircleCircleCollision(colA, colB),
            _ => new CollisionInfo { IsColliding = false }
        };
    }

    private static CollisionInfo CheckCircleBoxCollision(Collider colA, Collider colB)
    {
        CircleCollider circle = (CircleCollider)colA;
        BoxCollider box = (BoxCollider)colB;

        Vector2 circleCenter = circle.Position;
        float radius = circle.Radius;
        
        Vector2 closestPoint = FindClosestPointOnBox(circleCenter, box);

        // Dynamic SAT axis
        Vector2 dynamicAxis = circleCenter - closestPoint;

        // Edge case => circle center is exactly on closest point
        if (dynamicAxis.LengthSquared() < 1e-8f)
        {
            Vector2 fallbackNormal = Vector2.UnitY;

            return new CollisionInfo
            {
                IsColliding = true,
                Normal = fallbackNormal,
                MTV = fallbackNormal * radius,
                ColliderA = colA,
                ColliderB = colB
            };
        }

        // Find all SAT axes
        Vector2[] boxNormals = box.GetNormals();
        Vector2[] axes = new Vector2[boxNormals.Length + 1];

        for (int i = 0; i < boxNormals.Length; i++)
        {
            axes[i] = boxNormals[i];
        }

        axes[axes.Length - 1] = dynamicAxis;

        // SAT projections
        float minOverlap = float.MaxValue;
        Vector2 minAxis = Vector2.Zero;

        Vector2[] boxCorners = box.GetCorners();

        foreach (Vector2 axis in axes)
        {
            if (axis.LengthSquared() < 1e-8f)
            {
                continue;
            }

            Vector2 n = Vector2.Normalize(axis);

            // Circle projection
            float centerProj = Vector2.Dot(circleCenter, n);
            float circleMin = centerProj - radius;
            float circleMax = centerProj + radius;

            // Box projection
            float boxMin = float.MaxValue;
            float boxMax = float.MinValue;

            foreach (Vector2 corner in boxCorners)
            {
                float proj = Vector2.Dot(corner, n);
                boxMin = Math.Min(boxMin, proj);
                boxMax = Math.Max(boxMax, proj);
            }

            // Separation axis test
            if (circleMax < boxMin || boxMax < circleMin)
            {
                return new CollisionInfo { IsColliding = false };
            }
            
            // Calculate overlap
            float overlap = Math.Min(circleMax, boxMax) - Math.Max(circleMin, boxMin);

            if (overlap < minOverlap)
            {
                minOverlap = overlap;
                minAxis = n;
            }
        }

        // Calculate mtv direction
        Vector2 boxCenter = box.Position;
        Vector2 centerDiff = circleCenter - boxCenter;

        if (Vector2.Dot(minAxis, centerDiff) < 0)
        {
            minAxis = -minAxis;
        }

        Vector2 mtv = minAxis * minOverlap;

        return new CollisionInfo
        {
            IsColliding = true,
            MTV = mtv,
            Normal = minAxis,
            ColliderA = colA,
            ColliderB = colB
        };
    }
        
    private static CollisionInfo CheckCircleCircleCollision(Collider colA, Collider colB)
    {
        Vector2 vecToOtherCircle = colA.Position - colB.Position;
    
        float distanceSquared = vecToOtherCircle.LengthSquared();

        float combinedRadius = ((CircleCollider)colA).Radius + ((CircleCollider)colB).Radius;
        float combinedRadiusSquared = combinedRadius * combinedRadius;

        Vector2 mtv = Vector2.Zero;
        
        if (distanceSquared <= combinedRadiusSquared)
        {
            float distance = (float)Math.Sqrt(distanceSquared);
            float collisionDepth = combinedRadius - distance;

            Vector2 separationAxis;
            
            if (distance < 1e-6f)
            {
                separationAxis = Vector2.UnitY;
            }
            else
            {
                separationAxis = vecToOtherCircle / distance;
            }

            mtv = separationAxis * collisionDepth;
            
            return new CollisionInfo
            {
                IsColliding = true, 
                MTV = mtv, 
                Normal = Vector2.Normalize(mtv),
                ColliderA = colA,
                ColliderB = colB
            };
        }
    
        return new CollisionInfo { IsColliding = false };
    }

    private static Vector2 FindClosestPointOnBox(Vector2 point, BoxCollider box)
    {
        Vector2 boxPos = box.Position;
        float halfWidth = box.Width / 2f;
        float halfHeight = box.Height / 2f;
        float rotation = box.GameObject.Rotation;

        Vector2 localPoint = Vector2.Rotate(point - boxPos, -rotation);

        float localX = MathHelper.Clamp(localPoint.X, -halfWidth, halfWidth);
        float localY = MathHelper.Clamp(localPoint.Y, -halfHeight, halfHeight);

        Vector2 clampedLocal = new Vector2(localX, localY);

        Vector2 worldPoint = boxPos + Vector2.Rotate(clampedLocal, rotation);

        return worldPoint;
    }
}