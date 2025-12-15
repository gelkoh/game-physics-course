using System;
using GameLibrary;
using GameLibrary.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AngryMonkeys;

public class MonkeyController(ContentManager content, GameObject.Template flyingMonkeyTemplate) : Controller
{
    private bool _mousePressedLastFrame;
    private bool _isDraggingMonkey;
    private GameObject.Template _shootArrowTemplate;
    private GameObject _shootArrow;
    
    private const float MonkeyRadius = 26;
    private const float MaxArrowScale = 1f;
    private const float MinArrowScale = 0.1f;
    private const float MaxDragDistance = 100;

    public override void Connect(GameObject gameObject)
    {
        base.Connect(gameObject);
        _shootArrowTemplate =
            new GameObject.Template([new SpriteRenderer.Builder(content.Load<Texture2D>("arrowFull")).WithCenterOffset(new Vector2(100, 0)).Build()]);
    }

    public override void HandleInput(KeyboardState _, MouseState mouseState)
    {
        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            if (_mousePressedLastFrame)
            {
                HandleMouseDrag(mouseState.Position.ToVector2());
            }
            else
            {
                HandleJustPressedMouse(mouseState.Position.ToVector2());
            }
        }
        else
        {
            HandleMouseRelease(mouseState.Position.ToVector2());
        }
        _mousePressedLastFrame = mouseState.LeftButton == ButtonState.Pressed;
    }

    private void HandleJustPressedMouse(Vector2 mousePosition)
    {
        if (Vector2.Distance(GameObject.Position, mousePosition) <= MonkeyRadius)
        {
            _isDraggingMonkey = true;
            _shootArrow = _shootArrowTemplate.Instantiate(GameObject.Position);
        }
    }

    private void HandleMouseDrag(Vector2 mousePosition)
    {
        if (!_isDraggingMonkey)
            return;
        
        var dragVector = GameObject.Position - mousePosition;
        var dragAngle = Math.Atan(dragVector.Y / dragVector.X);
        if (dragVector.X < 0)
            dragAngle -= MathHelper.ToRadians(180);
        
        _shootArrow.Rotation = (float)dragAngle;
        
        var dragDistance = dragVector.Length();
        _shootArrow.Scale = MathHelper.Lerp(MinArrowScale, MaxArrowScale, InverseLerp(0, MaxDragDistance, dragDistance));
    }

    private void HandleMouseRelease(Vector2 mousePosition)
    {
        if (_isDraggingMonkey)
        {
            Console.WriteLine("Should shoot monkey");
            _shootArrow.Destroy();
            _shootArrow = null;
            var dragVector = GameObject.Position - mousePosition;
            var dragDistance = dragVector.Length();
            dragVector.Normalize();
            var flyingMonkeyPosition = GameObject.Position + dragVector * 100f;
            var flyingMonkey = flyingMonkeyTemplate.Instantiate(flyingMonkeyPosition);
            
            RigidBody flyingMonkeyRigidBody = flyingMonkey.GetComponent<RigidBody>();

            float maxShootForce = 500f;
            
            flyingMonkeyRigidBody.AddImpulse(dragVector * dragDistance * maxShootForce);
        }
        _isDraggingMonkey = false;
    }
    
    private float InverseLerp(float min, float max, float value)
    {
        return MathHelper.Clamp((value - min) / (max - min), 0, 1);
    }
}