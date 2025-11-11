using GameLibrary;
using GameLibrary.Physics;
using Microsoft.Xna.Framework.Graphics;

namespace BallDrop;

public class BounceBallCollisionEffect(Texture2D _effectTexture) : UpdateComponent
{
    private SpriteRenderer _spriteRenderer;
    private Collider _collider;
    private double _effectTimer = double.MinValue;
    private Texture2D _defaultTexture;
    
    private const double EffectDuration = 0.2f;
    
    public override void Connect(GameObject gameObject)
    {
        base.Connect(gameObject);
        _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
        if (_spriteRenderer == null)
            throw new Exception("BounceBallCollisionEffect could not find a SpriteRenderer!");
        _defaultTexture = _spriteRenderer.Texture;
        
        _collider = GameObject.GetComponent<Collider>();
        if (_collider == null)
            return;
        _collider.Collided += StartCollisionEffect;
    }

    public override void Destroy()
    {
        base.Destroy();
        if (_collider != null)
            _collider.Collided -= StartCollisionEffect;
    }

    public override void Update(double deltaTime)
    {
        if (_effectTimer > 0)
        {
            _effectTimer -= deltaTime;
            if (_effectTimer <= 0)
            {
                _spriteRenderer.Texture = _defaultTexture;
            }
        }
    }

    private void StartCollisionEffect()
    {
        _effectTimer = EffectDuration;
        _spriteRenderer.Texture = _effectTexture;
    }
}