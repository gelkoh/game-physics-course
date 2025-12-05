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
    public float Mass { get; private set; } = Mass;

    public void Update(double deltaTime)
    {
        // Todo: Move, Accelerate...
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