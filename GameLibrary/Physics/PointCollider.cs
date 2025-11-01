using GameLibrary.Physics;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
    public class PointCollider : Collider
    {
        public override Vector2 Position
        {
            get
            {
                return GameObject.Position;
            }
        }
    }
}