using GameLibrary.Physics;
using Microsoft.Xna.Framework;

namespace GameLibrary
{
    public class BoxCollider : Collider
    {
        public float Friction { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public BoxCollider(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override Vector2 Position
        {
            get
            {
                return GameObject.Position;
            }
        }
    }
}