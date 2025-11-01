using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

/// <summary>
/// Abstract Components that represents the form of a GameObject.
/// Handles Collisions.
/// </summary>
public abstract class Collider : Component
{
    public RigidBody RigidBody { get; private set; }
    public abstract Vector2 Position { get; }

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
}
