using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameLibrary
{
    public class CarController : CharacterController
    {
        private const float FORWARD_ACCELERATION = 1000f;
        private const float BACKWARD_ACCELERATION = FORWARD_ACCELERATION * 0.5f;
        private const float MAX_SPEED = 500f;
        private const float MAX_REVERSE_SPEED = MAX_SPEED * 0.5f;
            
        public override void HandleInput(KeyboardState state)
        {
            Vector2 forward = new Vector2(
                (float)Math.Cos(GameObject.Rotation - (Math.PI / 2)),
                (float)Math.Sin(GameObject.Rotation - (Math.PI / 2))
            );

            float inputFactor = 0f;
            float currentMaxSpeed = MAX_SPEED;
            float currentAcceleration = FORWARD_ACCELERATION;

            if (state.IsKeyDown(Keys.W))
            {
                inputFactor = 1f;
                currentMaxSpeed = MAX_SPEED;
                currentAcceleration = FORWARD_ACCELERATION;
            }
            else if (state.IsKeyDown(Keys.S))
            {
                inputFactor = -0.5f;
                currentMaxSpeed = MAX_REVERSE_SPEED;
                currentAcceleration = BACKWARD_ACCELERATION;
            }

            if (inputFactor != 0)
            {
                if (_rigidBody.Velocity.Length() < currentMaxSpeed)
                {
                    _rigidBody.AddForce(inputFactor * forward * currentAcceleration);
                }
                else
                {
                    _rigidBody.Velocity = Vector2.Normalize(_rigidBody.Velocity) * currentMaxSpeed;
                }
            }
            
            if (state.IsKeyDown(Keys.A)) GameObject.Rotation -= 0.1f;
            if (state.IsKeyDown(Keys.D)) GameObject.Rotation += 0.1f;
        }
    }
}