using System;
using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

/// <summary>
/// Abstract Components that represents the form of a GameObject.
/// Handles Collisions.
/// </summary>
public abstract class Collider : Component
{
    public RigidBody RigidBody { get; private set; }
    public float Elasticity { get; set; }

    public abstract Vector2 Position { get; }
    public abstract AABB GetAABB();
    
    
    public Action Collided;
    public void TriggerCollision() => Collided?.Invoke();
    
    // For collision friction
    public float StaticFriction { get; } = 2f;
    public float DynamicFriction { get; } = 1f;

    public override void Connect(GameObject gameObject)
    {
        base.Connect(gameObject);
        RigidBody = GameObject.GetComponent<RigidBody>();
        PhysicsWorld.ActiveColliders.Add(this);
    }

    public override void Destroy()
    {
        PhysicsWorld.ActiveColliders.Remove(this);
    }
    
    public virtual void Initialize() {}
}
