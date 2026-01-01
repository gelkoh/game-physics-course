using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

public class BoxCollider : Collider, IConvexPolygonCollider
{
    private int Width { get; }
    private int Height { get; }
    private float HalfWidth => Width / 2.0f;
    private float HalfHeight => Height / 2.0f;

    public BoxCollider(int width, int height, float elasticity)
    {
        Width = width;
        Height = height;
        // Elasticity is usually part of the physics material of the collider => here it's a direct member variable
        // of the collider because this games architecture doesn't have physics materials yet
        Elasticity = elasticity;
    }

    public override Vector2 Position { get => GameObject.Position; }

    public override AABB GetAABB()
    {
        Vector2[] worldVertices = GetWorldVertices(); 

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        foreach (Vector2 vertex in worldVertices)
        {
            if (vertex.X < minX) minX = vertex.X;
            if (vertex.X > maxX) maxX = vertex.X;
            if (vertex.Y < minY) minY = vertex.Y;
            if (vertex.Y > maxY) maxY = vertex.Y;
        }
    
        return new AABB
        {
            MinX = minX,
            MaxX = maxX,
            MinY = minY,
            MaxY = maxY
        };
    }

    public Vector2[] GetWorldVertices()
    {
        float rotation = GameObject.Rotation;
        Vector2 center = GameObject.Position;
    
        Vector2[] localCorners = new Vector2[]
        {
            new(-HalfWidth, -HalfHeight),
            new( HalfWidth, -HalfHeight),
            new( HalfWidth,  HalfHeight),
            new(-HalfWidth,  HalfHeight)
        };

        Vector2[] worldVertices = new Vector2[4];

        for (int i = 0; i < worldVertices.Length; i++)
        {
            // Rotate every local corner and place it according to the world position of the collider
            Vector2 rotatedCorner = Vector2.Rotate(localCorners[i], rotation); 
            worldVertices[i] = center + rotatedCorner;
        }
    
        return worldVertices;
    }
    
    public Vector2[] GetNormals()
    {
        float rotation = GameObject.Rotation;
    
        Vector2 rotatedUp = Vector2.Rotate(Vector2.UnitY, rotation);
        Vector2 rotatedRight = Vector2.Rotate(Vector2.UnitX, rotation);

        return [rotatedUp, rotatedRight];
    }
    
    public override void Initialize()
    {
        if (RigidBody != null && RigidBody.Mass > 0f)
        {
            // From lecture 5 slide 20: I =  1/12 * m * (a^2 + b^2) * angle
            float w = Width;
            float h = Height;
            
            RigidBody.MomentOfInertia = 1f / 12f * RigidBody.Mass * (w * w + h * h);
        }
    }
}