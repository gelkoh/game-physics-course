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
        
        float minOverlap = float.MaxValue;
        Vector2 minOverlapAxis = Vector2.Zero;

        Vector2 closestPoint = FindClosestPointOnBox(circle.Position, box); 
        Vector2 circleCenter = circle.Position;
        Vector2 dynamicAxis = circleCenter - closestPoint; 

        float radius = circle.Radius;

        if (dynamicAxis == Vector2.Zero)
        {
            return new CollisionInfo
            {
                IsColliding = true, 
                MTV = Vector2.UnitY * radius,
                Normal = Vector2.UnitY,
                ColliderA = colA,
                ColliderB = colB
            };
        }

        Vector2[] boxNormals = box.GetNormals();
        Vector2[] axesToCheck = new Vector2[boxNormals.Length + 1];

        for (int i = 0; i < boxNormals.Length; i++)
        {
            axesToCheck[i] = boxNormals[i];
        }
        axesToCheck[axesToCheck.Length - 1] = dynamicAxis;

        Vector2[] boxCorners = box.GetCorners();
        
        foreach (Vector2 axis in axesToCheck)
        {
            float axisLen = axis.Length();
            if (axisLen < 1e-6f) continue;
            
            Vector2 normal = axis / axisLen;
            
            float centerProj = Vector2.Dot(circleCenter, normal);
            float colAMin = centerProj - radius;
            float colAMax = centerProj + radius;
            
            float colBMin = float.MaxValue;
            float colBMax = float.MinValue;
            
            foreach (Vector2 corner in boxCorners)
            {
                float proj = Vector2.Dot(corner, normal);
                if (proj < colBMin) colBMin = proj;
                if (proj > colBMax) colBMax = proj;
            }
            
            if (colAMax < colBMin || colBMax < colAMin)
            {
                return new CollisionInfo { IsColliding = false };
            }
            
            float lengthA = colAMax - colAMin;
            float lengthB = colBMax - colBMin;
            float combinedLength = lengthA + lengthB;
            float totalLength = Math.Max(colAMax, colBMax) - Math.Min(colAMin, colBMin);
            
            float overlap = combinedLength - totalLength;
            
            if (overlap < minOverlap)
            {
                minOverlap = overlap;
                minOverlapAxis = normal;
            }
        }
        
        Vector2 mtv = minOverlapAxis * minOverlap;
        
        Vector2 boxCenter = box.Position;

        Vector2 centerDiff = circleCenter - boxCenter;

        if (Vector2.Dot(minOverlapAxis, centerDiff) < 0)
        {
            mtv = -mtv;
        }
        
        return new CollisionInfo
        {
            IsColliding = true, 
            MTV = mtv,
            Normal = Vector2.Normalize(mtv),
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