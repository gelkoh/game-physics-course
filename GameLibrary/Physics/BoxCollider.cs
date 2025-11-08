using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

public class BoxCollider : Collider
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public float Friction { get; private set; }

    public BoxCollider(int width, int height, float friction)
    {
        Width = width;
        Height = height;
        Friction = friction;
    }

    public override Vector2 Position
    {
        get => GameObject.Position;
    }
}