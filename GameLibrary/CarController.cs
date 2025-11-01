using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameLibrary
{
    public class CarController : CharacterController
    {
        private float _acceleration = 1000f;
        private float _maxSpeed = 500f;

        public override void HandleInput(KeyboardState state)
        {
            if (state.IsKeyDown(Keys.W))
            {
                Vector2 forward = new Vector2(
                    (float)Math.Cos(GameObject.Rotation - (Math.PI / 2)),
                    (float)Math.Sin(GameObject.Rotation - (Math.PI / 2))
                );

                if (_rigidBody.Velocity.Length() <= _maxSpeed)
                {
                    _rigidBody.AddForce(forward * _acceleration);
                }
                else
                {
                    _rigidBody.Velocity = Vector2.Normalize(_rigidBody.Velocity) * _maxSpeed;
                }
            }

            if (state.IsKeyDown(Keys.S))
            {
                Vector2 backward = new Vector2(
                    (float)Math.Cos(GameObject.Rotation - Math.PI / 2),
                    (float)Math.Sin(GameObject.Rotation - Math.PI / 2)
                );

                if (_rigidBody.Velocity.Length() <= _maxSpeed)
                {
                    _rigidBody.AddForce(-backward * _acceleration * 0.5f);
                }
                else
                {
                    _rigidBody.Velocity = Vector2.Normalize(_rigidBody.Velocity) * _maxSpeed * 0.5f;
                }
            }

            if (state.IsKeyDown(Keys.A)) GameObject.Rotation -= 0.1f;
            if (state.IsKeyDown(Keys.D)) GameObject.Rotation += 0.1f;
        }
    }
}