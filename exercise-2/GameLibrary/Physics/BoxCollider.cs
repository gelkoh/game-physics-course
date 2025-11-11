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
        return new AABB
        {
            MinX = GameObject.Position.X - (float)Width / 2,
            MaxX = GameObject.Position.X + (float)Width / 2,
            MinY = GameObject.Position.X - (float)Width / 2,
            MaxY = GameObject.Position.X + (float)Width / 2
        };
    }
}