using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

public class CircleCollider : Collider
{
    public float Radius { get; private set; }
    public float Elasticity { get; private set; }

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
            MinY = GameObject.Position.X - Radius,
            MaxY = GameObject.Position.X + Radius
        };
    }
}