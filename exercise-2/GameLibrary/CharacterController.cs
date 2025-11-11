using System;
using GameLibrary.Physics;

namespace GameLibrary
{
    /// <summary>
    /// Abstract Component to handle Player Input. Implement this to write your own Controls.
    /// </summary>
    public abstract class CharacterController : Controller
    {
        protected RigidBody RigidBody;
        
        public override void Connect(GameObject gameObject)
        {
            RigidBody = GameObject.GetComponent<RigidBody>();
            if (RigidBody == null)
                throw new Exception("CharacterController could not find RigidBody component!");
            
            base.Connect(gameObject);
        }
    }
}