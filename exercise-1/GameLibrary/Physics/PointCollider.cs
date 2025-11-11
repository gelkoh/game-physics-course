using Microsoft.Xna.Framework;

namespace GameLibrary.Physics;

public class PointCollider : Collider
{
    public override Vector2 Position
    {
        get => GameObject.Position;
    }
}