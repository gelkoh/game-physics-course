using System;
using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

public class CircleCollider : Collider
{
    public float Radius { get; }

    public CircleCollider(float radius, float elasticity)
    {
        Radius = radius;
        Elasticity = elasticity;
    }

    public override Vector2 Position { get => GameObject.Position; }

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
    
    public override void Initialize()
    {
        if (RigidBody != null && RigidBody.Mass > 0f)
        {
            // From lecture 5 slide 20: I = 1/2 * m * r^2 * angle
            RigidBody.MomentOfInertia = 0.5f * RigidBody.Mass * (Radius * Radius);
        }
    }
}