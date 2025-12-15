using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

public class TriangleCollider : Collider, IConvexPolygonCollider
{
    private Vector2[] localVertices;

    public TriangleCollider(Vector2 vertex1, Vector2 vertex2, Vector2 vertex3, float elasticity)
    {
        localVertices = [vertex1, vertex2, vertex3];
        
        Elasticity = elasticity;
    }

    public override Vector2 Position
    {
        get => GameObject.Position;
    }

    public override AABB GetAABB()
    {
        Vector2[] corners = GetWorldVertices(); 

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (Vector2 corner in corners)
        {
            if (corner.X < minX) minX = corner.X;
            if (corner.X > maxX) maxX = corner.X;
            if (corner.Y < minY) minY = corner.Y;
            if (corner.Y > maxY) maxY = corner.Y;
        }
    
        return new AABB
        {
            MinX = minX,
            MaxX = maxX,
            MinY = minY,
            MaxY = maxY
        };
    }

    public override Vector2[] GetNormals()
    {
        Vector2[] corners = GetWorldVertices();

        Vector2 edge1 = corners[1] - corners[0];
        Vector2 edge2 = corners[2] - corners[1];
        Vector2 edge3 = corners[0] - corners[2];
        
        Vector2 normal1 = new Vector2(-edge1.Y, edge1.X);
        Vector2 normal2 = new Vector2(-edge2.Y, edge2.X);
        Vector2 normal3 = new Vector2(-edge3.Y, edge3.X);
        
        normal1.Normalize();
        normal2.Normalize();
        normal3.Normalize();
        
        return new Vector2[] { normal1, normal2, normal3 };
    }

    public Vector2[] GetWorldVertices()
    {
        float rotation = GameObject.Rotation;
        Vector2 center = GameObject.Position;
        
        Vector2[] worldCorners = new Vector2[3];
        
        for (int i = 0; i < 3; i++)
        {
            Vector2 rotatedCorner = Vector2.Rotate(localVertices[i], rotation); 
        
            worldCorners[i] = center + rotatedCorner;
        }
    
        return worldCorners;
    }
}