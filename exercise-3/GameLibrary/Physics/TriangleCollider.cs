using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

public class TriangleCollider : Collider
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    private Vector2 a;
    private Vector2 b;
    private Vector2 c;

    public TriangleCollider(Vector2 a, Vector2 b, Vector2 c, float elasticity)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        Elasticity = elasticity;

        Width = 10;
        Height = 10;
    }

    public override Vector2 Position
    {
        get => GameObject.Position;
    }

    public override AABB GetAABB()
    {
        Vector2[] corners = GetCorners(); 

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
        float rotation = GameObject.Rotation;
        
        Vector2 rotatedUp = Vector2.UnitY;
        rotatedUp = Vector2.Rotate(rotatedUp, rotation);

        Vector2 rotatedRight = Vector2.UnitX;
        rotatedRight = Vector2.Rotate(rotatedRight, rotation);

        return new Vector2[] { rotatedUp, rotatedRight };
    }

    public override Vector2[] GetCorners()
    {
        float halfWidth = (float)Width / 2f;
        float halfHeight = (float)Height / 2f;
        float rotation = GameObject.Rotation;
        Vector2 center = GameObject.Position;
    
        Vector2[] localCorners = new Vector2[]
        {
            new(-halfWidth, -halfHeight),
            new( halfWidth, -halfHeight),
            new( halfWidth,  halfHeight),
            new(-halfWidth,  halfHeight)
        };

        Vector2[] worldCorners = new Vector2[4];
        
        for (int i = 0; i < 4; i++)
        {
            Vector2 rotatedCorner = Vector2.Rotate(localCorners[i], rotation); 
        
            worldCorners[i] = center + rotatedCorner;
        }
    
        return worldCorners;
    }
}