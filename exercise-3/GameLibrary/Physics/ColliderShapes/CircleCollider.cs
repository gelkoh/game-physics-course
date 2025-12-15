using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

public class CircleCollider : Collider
{
    public float Radius { get; private set; }

    public CircleCollider(float radius, float elasticity)
    {
        Radius = radius;
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
            MinX = GameObject.Position.X - Radius,
            MaxX = GameObject.Position.X + Radius,
            MinY = GameObject.Position.Y - Radius,
            MaxY = GameObject.Position.Y + Radius
        };
    }

    public override Vector2[] GetNormals()
    {
        return [Vector2.Zero];
    }
}