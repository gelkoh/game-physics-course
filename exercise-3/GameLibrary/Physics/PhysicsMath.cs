using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GameLibrary.Physics;

public struct Edge
{
    public Vector2 V1;
    public Vector2 V2;
    public Vector2 MaxVertex;
}

public static class PhysicsMath
{
    public static Vector2[] CalculateNormals(Vector2[] verts)
    {
        Vector2[] normals = new Vector2[verts.Length];

        for (int i = 0; i < verts.Length; i++)
        {
            Vector2 p1 = verts[i];
            Vector2 p2 = verts[(i + 1) % verts.Length];

            Vector2 edge = p2 - p1;
            
            Vector2 normal = new Vector2(-edge.Y, edge.X); 
    
            if (normal.LengthSquared() > 1e-8f)
            {
                normal.Normalize();
            }
    
            normals[i] = normal;
        }

        return normals;
    }
    
    public static void ProjectVertices(Vector2[] verts, Vector2 axis, out float min, out float max)
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

    public static Vector2 FindClosestVertex(Vector2 point, Vector2[] verts)
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
    
    public static void FindContactPoints(ref CollisionInfo info)
    {
        if (info.ColliderA is CircleCollider || info.ColliderB is CircleCollider)
        {
           FindCircleContactPoints(ref info); 
           return;
        }

        // Polygon-polygon collision
        IConvexPolygonCollider polyA = (IConvexPolygonCollider)info.ColliderA;
        IConvexPolygonCollider polyB = (IConvexPolygonCollider)info.ColliderB;
        Vector2 n = info.Normal;

        // Get incident edge
        Edge incidentEdge = FindBestEdge(polyB.GetWorldVertices(), -n);
        List<Vector2> clipPoints = new List<Vector2> { incidentEdge.V1, incidentEdge.V2 };

        // Get reference edge
        Edge referenceEdge = FindBestEdge(polyA.GetWorldVertices(), n);
        
        // Get reference edge tangent
        Vector2 refTan = Vector2.Normalize(referenceEdge.V2 - referenceEdge.V1);
        
        // Clipping operation
        float refOrigin = Vector2.Dot(referenceEdge.V1, refTan);
        float refEnd = Vector2.Dot(referenceEdge.V2, refTan);
        
        clipPoints = ClipSegmentToLine(clipPoints, refTan, refOrigin);
        
        if (clipPoints.Count < 2) return;
        
        clipPoints = ClipSegmentToLine(clipPoints, -refTan, -refEnd);
        
        if (clipPoints.Count < 2) return;

        // Check if the remaining points really within (or below) the surface of the reference edge
        float refDepth = Vector2.Dot(referenceEdge.MaxVertex, n);

        List<Vector2> finalContacts = new List<Vector2>();

        foreach (Vector2 p in clipPoints)
        {
            float depth = Vector2.Dot(p, n);
            
            if (depth <= refDepth + 0.01f) 
            {
                finalContacts.Add(p);
            }
        }

        info.ContactPoints = finalContacts.ToArray();
    }

    // Helper method for circles
    private static void FindCircleContactPoints(ref CollisionInfo info)
    {
        if (info.ColliderA is CircleCollider circleA && info.ColliderB is CircleCollider circleB)
        {
            Vector2 direction = Vector2.Normalize(circleB.Position - circleA.Position);
            info.ContactPoints = [circleA.Position + direction * circleA.Radius];
            return;
        }

        if (info.ColliderA is IConvexPolygonCollider && info.ColliderB is CircleCollider circle)
        {
            info.ContactPoints = [circle.Position - info.Normal * circle.Radius];
        }
    }
    
    // Find the edge whose normal most closely aligns with the collision normal (incident edge)
    // OR the edge that created the collision normal (reference edge)
    public static Edge FindBestEdge(Vector2[] verts, Vector2 normal)
    {
        float maxDot = float.MinValue;
        int bestIndex = 0;

        // We are looking for the vertex that lies furthest towards the normal
        for (int i = 0; i < verts.Length; i++)
        {
            // Project every point onto the collision normal to see how far it lies in the normals direction
            float dot = Vector2.Dot(verts[i], normal);
            
            if (dot > maxDot)
            {
                maxDot = dot;
                bestIndex = i;
            }
        }

        Vector2 v = verts[bestIndex];
        Vector2 vPrev = verts[(bestIndex - 1 + verts.Length) % verts.Length];
        Vector2 vNext = verts[(bestIndex + 1) % verts.Length];

        Vector2 leftEdge = Vector2.Normalize(v - vPrev);
        Vector2 rightEdge = Vector2.Normalize(vNext - v);
        
        // Find edge whose normal has the smallest scalar product with the MTV
        if (Vector2.Dot(rightEdge, normal) <= Vector2.Dot(leftEdge, normal))
        {
            // If 'right' edge is better
            return new Edge { V1 = v, V2 = vNext, MaxVertex = v };
        }

        // If 'left' edge is better
        return new Edge { V1 = vPrev, V2 = v, MaxVertex = v }; 
    }
    
    // Line clipping method
    public static List<Vector2> ClipSegmentToLine(List<Vector2> polygonPoints, Vector2 normal, float offset)
    {
        List<Vector2> output = new List<Vector2>();
        
        Vector2 v1 = polygonPoints[0];
        Vector2 v2 = polygonPoints[1];

        float dist1 = Vector2.Dot(v1, normal) - offset;
        float dist2 = Vector2.Dot(v2, normal) - offset;

        if (dist1 >= 0) output.Add(v1);
        if (dist2 >= 0) output.Add(v2);

        if (dist1 * dist2 < 0)
        {
            float t = dist1 / (dist1 - dist2);
            Vector2 intersection = v1 + (v2 - v1) * t;
            output.Add(intersection);
        }
        
        return output;
    }
}