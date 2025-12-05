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
        public Color Color { get; set; } = Color.White;
        public Vector2 Offset => _localOffset * GameObject.Scale;
        public bool Enabled { get; private set; }
        public float Scale => _localScale * GameObject.Scale;
        public Vector2 Position {
            get 
            {
                var rotatedOffset = Offset;
                rotatedOffset.Rotate(GameObject.Rotation);
                return GameObject.Position + rotatedOffset;   
            }
        }
        
        private float _localScale = 1.0f;
        private Vector2 _localOffset;

        private SpriteRenderer(Texture2D texture)
        {
            Texture = texture;
        }

        public override void Connect(GameObject gameObject)
        {
            Enable();
            base.Connect(gameObject);
        }

        public override void Destroy()
        {
            Disable();
        }

        public void Enable()
        {
            if (Enabled)
                return; 
            Enabled = true;
            Core.ActiveRenderers.Add(this);
        }

        public void Disable()
        {
            if (!Enabled)
                return;
            Enabled = false;
            Core.ActiveRenderers.Remove(this);
        }

        public class Builder(Texture2D texture)
        {
            private SpriteRenderer _renderer = new(texture);
            private Vector2 _customCenterOffset = Vector2.Zero;

            public Builder WithColor(Color color)
            {
                _renderer.Color = color;
                return this;
            }

            public Builder WithScale(float scale)
            {
                _renderer._localScale = scale;
                return this;
            }

            public Builder WithCenterOffset(Vector2 offset)
            {
                _customCenterOffset = offset;
                return this;
            }
            
            public SpriteRenderer Build() {
                _renderer._localOffset = new Vector2(-texture.Width / 2, -texture.Height / 2) * _renderer._localScale + _customCenterOffset;
                return _renderer;
            }
        }
    }
}