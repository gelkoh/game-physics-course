using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameLibrary
{
    public class CarController : CharacterController
    {
        private const float FORWARD_ACCELERATION = 500f;
        private const float BACKWARD_ACCELERATION = FORWARD_ACCELERATION * 0.75f;
        private const float MAX_SPEED = 500f;
        private const float MAX_REVERSE_SPEED = MAX_SPEED * 0.75f;
            
        public override void HandleInput(KeyboardState state)
        {
            Vector2 forward = new Vector2(
                (float)Math.Cos(GameObject.Rotation - (Math.PI / 2)),
                (float)Math.Sin(GameObject.Rotation - (Math.PI / 2))
            );
            
            Vector2 currentVelocity = _rigidBody.Velocity;
            float currentSpeed = currentVelocity.Length();
            
            if (state.IsKeyDown(Keys.W))
            {
                _rigidBody.AddForce(1f * forward * FORWARD_ACCELERATION);
                
                if (currentSpeed > MAX_SPEED)
                {
                    _rigidBody.Velocity = Vector2.Normalize(currentVelocity) * MAX_SPEED;
                }
            }
            else if (state.IsKeyDown(Keys.S))
            {
                _rigidBody.AddForce(-1f * forward * BACKWARD_ACCELERATION);
                
                if (currentSpeed > MAX_REVERSE_SPEED)
                {
                    _rigidBody.Velocity = Vector2.Normalize(currentVelocity) * MAX_REVERSE_SPEED;
                }
            } 
            
            if (state.IsKeyDown(Keys.A)) GameObject.Rotation -= 0.1f;
            else if (state.IsKeyDown(Keys.D)) GameObject.Rotation += 0.1f;
        }
    }
}