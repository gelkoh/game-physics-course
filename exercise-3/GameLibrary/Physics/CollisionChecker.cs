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
    private static CollisionInfo NarrowPhaseCheck(Collider colA, Collider colB)
    {
        if (colA is CircleCollider circleColACC && colB is CircleCollider circleColBCC)
        {
            var info = CheckCircleCircleCollision(circleColACC, circleColBCC);
            
            if (info.IsColliding)
            {
                PhysicsMath.FindContactPoints(ref info); 
            }
            
            return info;
        }

        if (colA is CircleCollider circleColACP && colB is IConvexPolygonCollider polygonColBCP)
        {
            var info = CheckCirclePolygonCollision(circleColACP, polygonColBCP);
            
            if (info.IsColliding)
            {
                PhysicsMath.FindContactPoints(ref info); 
            }
            
            return info;
        }

        if (colA is IConvexPolygonCollider circleColACP2 && colB is CircleCollider circleColBCP2)
        {
            var info = CheckCirclePolygonCollision(circleColBCP2, circleColACP2);
            
            if (info.IsColliding)
            {
                PhysicsMath.FindContactPoints(ref info); 
            }
            
            return info;
        }

        if (colA is IConvexPolygonCollider colAPP && colB is IConvexPolygonCollider colBPP)
        {
            var info = CheckPolygonPolygonCollision(colAPP, colBPP);
            
            if (info.IsColliding)
            {
                PhysicsMath.FindContactPoints(ref info); 
            }
            
            return info;
        }

        return new CollisionInfo { IsColliding = false };
    }
        
    private static CollisionInfo CheckCircleCircleCollision(Collider colA, Collider colB)
    {
        Vector2 vecAtoB = colB.Position - colA.Position;

        float distanceSquared = vecAtoB.LengthSquared();
        float combinedRadius = ((CircleCollider)colA).Radius + ((CircleCollider)colB).Radius;
        float combinedRadiusSquared = combinedRadius * combinedRadius;

        if (distanceSquared <= combinedRadiusSquared)
        {
            float distance = (float)Math.Sqrt(distanceSquared);
            
            Vector2 normal;
            
            if (distance < 1e-6f)
            {
                // Exception: Circles are exactly on the same position => random direction
                normal = Vector2.UnitY;
            }
            else
            {
                normal = vecAtoB / distance;
            }

            float collisionDepth = combinedRadius - distance;
            Vector2 mtv = normal * collisionDepth;
        
            return new CollisionInfo
            {
                IsColliding = true, 
                MTV = mtv, 
                Normal = normal,
                ColliderA = colA,
                ColliderB = colB
            };
        }

        return new CollisionInfo { IsColliding = false };
    }

    private static CollisionInfo CheckCirclePolygonCollision(
        CircleCollider circle,
        IConvexPolygonCollider polygon)
    {
        Vector2[] verts = polygon.GetWorldVertices();

        float minOverlap = float.MaxValue;
        Vector2 smallestAxis = Vector2.Zero;

        List<Vector2> axesToTest = new List<Vector2>();
        axesToTest.AddRange(polygon.GetNormals());

        Vector2 closest = PhysicsMath.FindClosestVertex(circle.Position, verts);
        Vector2 axisToVertex = circle.Position - closest;

        if (axisToVertex.LengthSquared() > 1e-8f)
        {
            axesToTest.Add(Vector2.Normalize(axisToVertex));
        }
    
        foreach (Vector2 axis in axesToTest)
        {
            PhysicsMath.ProjectVertices(verts, axis, out float polyMin, out float polyMax);

            // Project circle
            float centerProj = Vector2.Dot(circle.Position, axis);
            float circleMin = centerProj - circle.Radius;
            float circleMax = centerProj + circle.Radius;

            if (circleMax < polyMin || polyMax < circleMin)
                return new CollisionInfo { IsColliding = false };

            float overlap = Math.Min(circleMax, polyMax) - Math.Max(circleMin, polyMin);

            if (overlap < minOverlap)
            {
                minOverlap = overlap;
                smallestAxis = axis;
            }
        }
    
        Vector2 polyCenter = ((Collider)polygon).Position;
        Vector2 directionPolyToCircle = circle.Position - polyCenter;

        if (Vector2.Dot(smallestAxis, directionPolyToCircle) < 0)
        {
            smallestAxis = -smallestAxis;
        }
    
        return new CollisionInfo
        {
            IsColliding = true,
            Normal = smallestAxis,
            MTV = smallestAxis * minOverlap,
            ColliderA = (Collider)polygon,
            ColliderB = circle
        };
    }
    
    private static CollisionInfo CheckPolygonPolygonCollision(
        IConvexPolygonCollider a,
        IConvexPolygonCollider b)
    {
        Vector2[] vertsA = a.GetWorldVertices();
        Vector2[] vertsB = b.GetWorldVertices();

        float minOverlap = float.MaxValue;
        Vector2 smallestAxis = Vector2.Zero;

        // We remember who provided us with the normal
        IConvexPolygonCollider refPoly = a;
        IConvexPolygonCollider incPoly = b;

        // Check normals of A
        foreach (Vector2 axis in a.GetNormals())
        {
            PhysicsMath.ProjectVertices(vertsA, axis, out float minA, out float maxA);
            PhysicsMath.ProjectVertices(vertsB, axis, out float minB, out float maxB);

            if (maxA < minB || maxB < minA) return new CollisionInfo { IsColliding = false };

            float overlap = Math.Min(maxA, maxB) - Math.Max(minA, minB);
            if (overlap < minOverlap)
            {
                minOverlap = overlap;
                smallestAxis = axis;
                refPoly = a;
                incPoly = b;
            }
        }

        // Check normals of B
        foreach (Vector2 axis in b.GetNormals())
        {
            PhysicsMath.ProjectVertices(vertsA, axis, out float minA, out float maxA);
            PhysicsMath.ProjectVertices(vertsB, axis, out float minB, out float maxB);

            if (maxA < minB || maxB < minA) return new CollisionInfo { IsColliding = false };

            float overlap = Math.Min(maxA, maxB) - Math.Max(minA, minB);
            if (overlap < minOverlap)
            {
                minOverlap = overlap;
                smallestAxis = axis;
                refPoly = b;
                incPoly = a;
            }
        }
        
        Vector2 centerRef = ((Collider)refPoly).Position;
        Vector2 centerInc = ((Collider)incPoly).Position;
        Vector2 directionRefToInc = centerInc - centerRef;

        // If axis points to opposite direction turn it around
        if (Vector2.Dot(smallestAxis, directionRefToInc) < 0)
        {
            smallestAxis = -smallestAxis;
        }
        
        return new CollisionInfo
        {
            IsColliding = true,
            Normal = smallestAxis,
            MTV = smallestAxis * minOverlap,
            ColliderA = (Collider)refPoly,
            ColliderB = (Collider)incPoly
        };
    }
}