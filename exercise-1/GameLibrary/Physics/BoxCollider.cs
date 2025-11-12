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
    
    public bool IsPointInsideBoxCollider(Vector2 point, BoxCollider box)
    {
        Vector2 pos = box.Position;
        float halfW = box.Width / 2f;
        float halfH = box.Height / 2f;

        return point.X >= pos.X - halfW &&
               point.X <= pos.X + halfW &&
               point.Y >= pos.Y - halfH &&
               point.Y <= pos.Y + halfH;
    }

    public override Vector2 Position
    {
        get => GameObject.Position;
    }
}