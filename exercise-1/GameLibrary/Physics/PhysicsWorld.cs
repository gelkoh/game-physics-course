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
    
    private static readonly float GRAVITY_MAGNITUDE = 9.81f;

    /// <summary>
    /// Gets called every frame.
    /// </summary>
    /// <param name="deltaTime">Time passed since the last call.</param>
    public void Update(double deltaTime)
    {
        foreach (var body in RigidBodies)
        {
            body.CurrentFriction = GetTileFrictionFor(body);
            
            float normalForceMagnitude = body.Mass * GRAVITY_MAGNITUDE;
            float frictionMagnitude = body.CurrentFriction * normalForceMagnitude;

            // Only apply friction if body is moving
            if (body.Velocity.LengthSquared() > 0.001f)
            {
                Vector2 frictionVector = -Vector2.Normalize(body.Velocity) * frictionMagnitude;
                body.AddForce(frictionVector);
            }
            
            body.Integrate((float)deltaTime);    
        }
    }

    private float GetTileFrictionFor(RigidBody body)
    {
        foreach (var col in ActiveColliders)
        {
            if (col is BoxCollider box && box.IsPointInsideBoxCollider(body.GameObject.Position, box))
            {
                return box.Friction;
            }
        }

        return 4f; // default off-road friction
    }
}
