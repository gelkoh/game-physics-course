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

    private readonly Vector2 gravity = new Vector2(0f, 500f);
    
    public void Update(double deltaTime)
    {
        // This method follows principle from lecture 1 slide 13: Simulate forces => check collisions => update positions
        
        float dt = (float)deltaTime;
        
        // Simulate forces
        foreach (RigidBody g in RigidBodies)
        {
            g.AddForce(gravity * g.Mass);
            g.Integrate(dt);
        }

        // Check collisions
        List<CollisionInfo> collisions = CollisionChecker.CheckForCollisions(ActiveColliders);
        
        foreach (CollisionInfo info in collisions)
        {
            info.ColliderA?.TriggerCollision();
            info.ColliderB?.TriggerCollision();
        }
        
        // Update positions (by resolving collisions)
        CollisionResolver.ResolveCollisions(collisions, dt, gravity);
    }
}
