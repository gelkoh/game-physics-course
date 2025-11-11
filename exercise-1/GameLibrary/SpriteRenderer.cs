using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameLibrary
{
    /// <summary>
    /// Handles the texture and visual appearance of a GameObject.
    /// </summary>
    public class SpriteRenderer : Component
    {
        public Texture2D Texture { get; set; }
        public Color Color { get; set; }
        public Vector2 Offset { get; private set; }

        public SpriteRenderer(Texture2D texture, Color color)
        {
            Texture = texture;
            Color = color;
            Offset = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public SpriteRenderer(Texture2D texture)
        {
            Texture = texture;
            Color = Color.White;
            Offset = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public override void Connect(GameObject gameObject)
        {
            Core.ActiveRenderers.Add(this);
            base.Connect(gameObject);
        }

        public override void Destroy()
        {
            Core.ActiveRenderers.Remove(this);
        }
    }
}