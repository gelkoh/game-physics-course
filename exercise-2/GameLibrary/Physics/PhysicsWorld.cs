using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

/// <summary>
/// The Core of the Physics Engine. Handles Collisions and updates RigidBodies every frame.
/// </summary>
public class PhysicsWorld
{
    public static readonly List<Collider> ActiveColliders = [];
    public static readonly List<RigidBody> RigidBodies = [];

    private Vector2 gravity = new Vector2(0, 500f);
    
    public void Update(double deltaTime)
    {
        foreach (RigidBody g in RigidBodies)
        {
            g.AddForce(gravity * g.Mass);
            g.Integrate((float)deltaTime);
        }

        List<CollisionInfo> collisions = CollisionChecker.CheckForCollisions(ActiveColliders);

        CollisionResolver.ResolveCollisions(collisions);
    }
}
