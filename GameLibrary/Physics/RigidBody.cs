using System;
using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

/// <summary>
/// Component that represents a gameobjects physical presence in the world.
/// Does not define size or bounds of the object. 
/// Handles Mass and Force Application.
/// </summary>
public class RigidBody : Component
{
    public Vector2 Velocity { get; set; }
    public float Mass = 1f;

    private Vector2 _force;
    public float CurrentFriction { get; set; } = 0.0f;
    public bool UsesTileFriction { get; private set; }

    public RigidBody(bool usesTileFriction = false)
    {
        UsesTileFriction = usesTileFriction;
    }

    public void AddForce(Vector2 force)
    {
        _force += force;
    }

    public void Integrate(float deltaTime)
    {
        Vector2 acceleration = _force / Mass;
        Velocity += acceleration * deltaTime;
        
        Velocity -= Velocity * CurrentFriction * deltaTime;

        Vector2 forward = new(
            (float)Math.Cos(GameObject.Rotation - Math.PI / 2),
            (float)Math.Sin(GameObject.Rotation - Math.PI / 2)
        );

        float forwardSpeed = Vector2.Dot(Velocity, forward);

        Velocity = forward * forwardSpeed;

        GameObject.Position += Velocity * deltaTime;

        _force = Vector2.Zero;
    }

    public override void Connect(GameObject gameObject)
    {
        PhysicsWorld.RigidBodies.Add(this);
        base.Connect(gameObject);
    }

    public override void Destroy()
    {
        PhysicsWorld.RigidBodies.Remove(this);
    }
}