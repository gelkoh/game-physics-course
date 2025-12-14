namespace GameLibrary.Physics;
using Microsoft.Xna.Framework;


public struct CollisionInfo
{
    public bool IsColliding { get; set; }
    public Vector2 MTV { get; set; }
    public Vector2 Normal { get; set; }
    public Collider ColliderA { get; set; }
    public Collider ColliderB { get; set; }
}