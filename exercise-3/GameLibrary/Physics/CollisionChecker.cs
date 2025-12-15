using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            return CheckCircleCircleCollision(circleColACC, circleColBCC);
        
        if (colA is CircleCollider circleColACP && colB is IConvexPolygonCollider polygonColBCP)
            return CheckCirclePolygonCollision(circleColACP, polygonColBCP);
        
        if (colA is IConvexPolygonCollider circleColACP2 && colB is CircleCollider circleColBCP2)
            return CheckCirclePolygonCollision(circleColBCP2, circleColACP2);
        
        if (colA is IConvexPolygonCollider colAPP && colB is IConvexPolygonCollider colBPP)
            return CheckPolygonPolygonCollision(colAPP, colBPP);
        
        return new CollisionInfo { IsColliding = false };
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

        Vector2[] boxCorners = box.GetWorldVertices();

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

    private static CollisionInfo CheckCirclePolygonCollision(
        CircleCollider circle,
        IConvexPolygonCollider polygon)
    {
        Vector2[] verts = polygon.GetWorldVertices();

        float minOverlap = float.MaxValue;
        Vector2 smallestAxis = Vector2.Zero;

        foreach (Vector2 axis in GetPolygonAxes(verts))
        {
            Project(verts, axis, out float polyMin, out float polyMax);

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

        Vector2 closest = FindClosestVertex(circle.Position, verts);
        Vector2 axisToVertex = circle.Position - closest;
        
        if (axisToVertex.LengthSquared() > 1e-8f)
        {
            Vector2 dynamicAxis = Vector2.Normalize(axisToVertex);

            Project(verts, dynamicAxis, out float polyMin, out float polyMax);

            float centerProj = Vector2.Dot(circle.Position, dynamicAxis);
            float circleMin = centerProj - circle.Radius;
            float circleMax = centerProj + circle.Radius;

            if (circleMax < polyMin || polyMax < circleMin)
                return new CollisionInfo { IsColliding = false };

            float overlap = Math.Min(circleMax, polyMax) - Math.Max(circleMin, polyMin);

            if (overlap < minOverlap)
            {
                minOverlap = overlap;
                smallestAxis = dynamicAxis;
            }
        }
        
        Vector2 polyCenter = ComputeCentroid(verts);
        Vector2 centerDiff = circle.Position - polyCenter;

        if (Vector2.Dot(smallestAxis, centerDiff) < 0)
        {
            smallestAxis = -smallestAxis;
        }
        
        return new CollisionInfo
        {
            IsColliding = true,
            Normal = smallestAxis,
            MTV = smallestAxis * minOverlap,
            ColliderA = circle,
            ColliderB = (Collider)polygon
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
        
        string nameA = (a is BoxCollider) ? "Box" : "Poly";
        string nameB = (b is BoxCollider) ? "Boden" : "Poly";

        int axisIndex = 0;
        foreach (Vector2 axis in GetPolygonAxes(vertsA, vertsB))
        {
            axisIndex++;
            Project(vertsA, axis, out float minA, out float maxA);
            Project(vertsB, axis, out float minB, out float maxB);

            if (maxA < minB || maxB < minA)
            {
                return new CollisionInfo { IsColliding = false };
            }

            float overlap = Math.Min(maxA, maxB) - Math.Max(minA, minB);

            if (overlap < minOverlap)
            {
                minOverlap = overlap;
                smallestAxis = axis;
            }
        }

        Vector2 centerA = ComputeCentroid(vertsA);
        Vector2 centerB = ComputeCentroid(vertsB);
        Vector2 centerDiff = centerA - centerB;
        
        bool flipped = false;
        
        if (Vector2.Dot(smallestAxis, centerDiff) < 0)
        {
            smallestAxis = -smallestAxis;
            flipped = true;
        }
        
        return new CollisionInfo
        {
            IsColliding = true,
            Normal = smallestAxis,
            MTV = smallestAxis * minOverlap,
            ColliderA = (Collider)a,
            ColliderB = (Collider)b
        };
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
    
    private static IEnumerable<Vector2> GetPolygonAxes(Vector2[] verts)
    {
        int count = verts.Length;

        for (int i = 0; i < count; i++)
        {
            Vector2 p1 = verts[i];
            Vector2 p2 = verts[(i + 1) % count];

            Vector2 edge = p2 - p1;

            if (edge.LengthSquared() < 1e-8f)
                continue;

            Vector2 normal = new Vector2(-edge.Y, edge.X);
            normal.Normalize();

            yield return normal;
        }
    }

    private static IEnumerable<Vector2> GetPolygonAxes(Vector2[] a, Vector2[] b)
    {
        foreach (var axis in GetPolygonAxes(a))
            yield return axis;

        foreach (var axis in GetPolygonAxes(b))
            yield return axis;
    }

    private static void Project(Vector2[] verts, Vector2 axis, out float min, out float max)
    {
        float projection = Vector2.Dot(verts[0], axis);
        min = projection;
        max = projection;

        for (int i = 1; i < verts.Length; i++)
        {
            projection = Vector2.Dot(verts[i], axis);
            if (projection < min) min = projection;
            if (projection > max) max = projection;
        }
    }
    
    private static Vector2 ComputeCentroid(Vector2[] verts)
    {
        Vector2 sum = Vector2.Zero;
        foreach (var v in verts)
            sum += v;

        return sum / verts.Length;
    }

    private static Vector2 FindClosestVertex(Vector2 point, Vector2[] verts)
    {
        float minDistSq = float.MaxValue;
        Vector2 closest = verts[0];

        foreach (var v in verts)
        {
            float distSq = Vector2.DistanceSquared(point, v);
            if (distSq < minDistSq)
            {
                minDistSq = distSq;
                closest = v;
            }
        }

        return closest;
    }
}