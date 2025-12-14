using System;
using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

/// <summary>
/// Component that represents a gameobjects physical presence in the world.
/// Does not define size or bounds of the object. 
/// Handles Mass and Force Application.
/// </summary>
/// <param name="Mass"></param>
public class RigidBody(float Mass) : Component
{
    public Vector2 Velocity { get; set; }
    public float Mass { get; set; } = Mass;
    private Vector2 _force;
    
    public void AddForce(Vector2 force)
    {
        _force += force;
    }
    
    public void AddImpulse(Vector2 impulse)
    {
        if (Mass == 0) return;
        
        Velocity += impulse / Mass; 
    }

    public void Integrate(float deltaTime)
    {
        Vector2 acceleration = _force / Mass;

        Velocity += deltaTime * acceleration;

        GameObject.Position += deltaTime * Velocity;

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