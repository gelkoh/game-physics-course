using System;
using Microsoft.Xna.Framework.Input;
using GameLibrary.Physics;

namespace GameLibrary
{
    /// <summary>
    /// Abstract Component to handle Player Input. Implement this to write your own Controls.
    /// </summary>
    public abstract class CharacterController() : Component
    {
        protected RigidBody _rigidBody;

        public abstract void HandleInput(KeyboardState state);

        public override void Connect(GameObject gameObject)
        {
            base.Connect(gameObject);

            _rigidBody = GameObject.GetComponent<RigidBody>();
            
            if (_rigidBody == null)
                throw new Exception("CharacterController could not find RigidBody component!");

            Core.Controllers.Add(this);
        }
        
        public override void Destroy()
        {
            Core.Controllers.Remove(this);
        }
    }
}