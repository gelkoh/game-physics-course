using System;
using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

/// <summary>
/// Component that represents a gameobjects physical presence in the world.
/// Does not define size or bounds of the object. 
/// Handles Mass and Force Application.
/// </summary>
/// <param name="Mass"></param>
public class RigidBody : Component
{
    public Vector2 Velocity { get; set; }
    private Vector2 _force;

    public float Mass { get; set; }
    public float InverseMass => Mass > 0f ? 1f / Mass : 0f;
    
    public float AngularVelocity; // Omega
    public float MomentOfInertia; // I
    public float InverseInertia => MomentOfInertia > 0f ? 1f / MomentOfInertia : 0f;

    public RigidBody(float mass)
    {
        this.Mass = mass;
        MomentOfInertia = mass * 500f;
    }
    
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
        
        GameObject.Rotation += AngularVelocity * deltaTime;
        
        // Dämpfung (Luftwiderstand für Drehung), damit sie nicht ewig drehen
        AngularVelocity *= 0.98f;

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