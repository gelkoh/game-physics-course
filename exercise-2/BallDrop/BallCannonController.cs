using GameLibrary;
using GameLibrary.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BallDrop;

public class BallCannonController(GameObject.Template ballTemplate) : Controller
{
    private float _currentRotationAngle;
    private bool _pressedSpaceLastFrame;
    private bool _isLoaded = true;
    private float _inputStrength;
    private SpriteRenderer _arrowRenderer;
    
    private const float MaxRotation = 75f;
    private const float RotationSpeed = 5f;
    private const float ShootStrengthIncreasePerFrame = 0.01f;

    public override void Connect(GameObject gameObject)
    {
        base.Connect(gameObject);
        _arrowRenderer = GameObject.GetComponent<SpriteRenderer>();
    }

    public override void HandleInput(KeyboardState state)
    {
        if (!_isLoaded)
            return;
        
        if (state.IsKeyDown(Keys.Left))
        {
            _currentRotationAngle += RotationSpeed;
        }
        else if (state.IsKeyDown(Keys.Right))
        {
            _currentRotationAngle -= RotationSpeed;
        }
        _currentRotationAngle = Math.Clamp(_currentRotationAngle, -MaxRotation, MaxRotation);
        GameObject.Rotation = MathHelper.ToRadians(_currentRotationAngle);

        if (_pressedSpaceLastFrame)
        {
            if (state.IsKeyDown(Keys.Space))
            {
                _inputStrength = Math.Min(_inputStrength + ShootStrengthIncreasePerFrame, 1.0f);
                _arrowRenderer.Color = Color.Lerp(Color.White, Color.Red, _inputStrength);
            }
            else
            {
                ShootBall();
                _isLoaded = false;
                _arrowRenderer.Disable();
                _inputStrength = 0;
            }
        }
        _pressedSpaceLastFrame = state.IsKeyDown(Keys.Space);
    }

    private void ShootBall()
    {
        var ball = ballTemplate.Instantiate(GameObject.Position);
        ball.GetComponent<SelfDestroyWhenOffScreen>().OnDestroy += Reload;
        var shootDirection = new Vector2(0, 1);
        shootDirection.Rotate(GameObject.Rotation);
        
        float maxShootForce = 1500f;

        // Todo: Implement the actual shooting of "ball" using shootDirection and _inputStrength (value from 0 to 1)
        // hint: ball has a RigidBody component
        RigidBody ballRigidBody = ball.GetComponent<RigidBody>();
        
        ballRigidBody.AddImpulse(shootDirection * _inputStrength * maxShootForce);
    }

    private void Reload()
    {
        _isLoaded = true;
        _arrowRenderer.Color = Color.White;
        _arrowRenderer.Enable();
    }
}