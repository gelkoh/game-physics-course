using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

public class BoxCollider : Collider
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float Elasticity { get; private set; }

    public BoxCollider(int width, int height, float elasticity)
    {
        Width = width;
        Height = height;
        Elasticity = elasticity;
    }

    public override Vector2 Position
    {
        get => GameObject.Position;
    }

    public override AABB GetAABB()
    {
        float halfWidth = (float)Width / 2f;
        float halfHeight = (float)Height / 2f;
        
        return new AABB
        {
            MinX = GameObject.Position.X - halfWidth,
            MaxX = GameObject.Position.X + halfWidth,
            MinY = GameObject.Position.Y - halfHeight,
            MaxY = GameObject.Position.Y + halfHeight
        };
    }
}