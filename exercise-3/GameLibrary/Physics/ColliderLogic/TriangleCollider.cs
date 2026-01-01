using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

public class TriangleCollider : Collider, IConvexPolygonCollider
{
    private Vector2[] localVertices = new Vector2[3];

    public TriangleCollider(Vector2 corner1, Vector2 corner2, Vector2 corner3, float elasticity)
    {
        localVertices = [corner1, corner2, corner3];
        
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
        Vector2[] worldVertices = new Vector2[3];
        
        for (int i = 0; i < worldVertices.Length; i++)
        {
            // Rotate every local corner and place it according to the world position of the collider
            Vector2 rotatedCorner = Vector2.Rotate(localVertices[i], GameObject.Rotation); 
            worldVertices[i] = GameObject.Position + rotatedCorner;
        }
    
        return worldVertices;
    }
    
    public Vector2[] GetNormals()
    {
        return PhysicsMath.CalculateNormals(GetWorldVertices());
    }
    
    public override void Initialize()
    {
        if (RigidBody != null && RigidBody.Mass > 0f)
        {
            AABB aabb = GetAABB();

            float width = aabb.MaxX - aabb.MinX;
            float height = aabb.MaxY - aabb.MinY;
            
            // Found online
            RigidBody.MomentOfInertia = 1f / 18f * RigidBody.Mass * (width * width + height * height);
        }
    }
}