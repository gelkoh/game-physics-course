using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

/// <summary>
/// The Core of the Physics Engine. Handles Collisions and updates RigidBodies every frame.
/// </summary>
public class PhysicsWorld()
{
    public static readonly List<Collider> ActiveColliders = [];
    public static readonly List<RigidBody> RigidBodies = [];

    /// <summary>
    /// Gets called every frame.
    /// </summary>
    /// <param name="deltaTime">Time passed since the last call.</param>
    public void Update(double deltaTime)
    {
        foreach (var body in RigidBodies)
        {
            if (body.UsesTileFriction)
                body.CurrentFriction = GetTileFrictionFor(body);
                
            body.Integrate((float)deltaTime);    
        }
    }

    private float GetTileFrictionFor(RigidBody body)
    {
        foreach (var col in ActiveColliders)
        {
            if (col is BoxCollider box && IsPointInsideBox(body.GameObject.Position, box))
            {
                return box.Friction;
            }
        }

        return 6.0f; // default off-road friction
    }

    private bool IsPointInsideBox(Vector2 point, BoxCollider box)
    {
        Vector2 pos = box.Position;
        float halfW = box.Width / 2f;
        float halfH = box.Height / 2f;

        return point.X >= pos.X - halfW &&
               point.X <= pos.X + halfW &&
               point.Y >= pos.Y - halfH &&
               point.Y <= pos.Y + halfH;
    }
}
