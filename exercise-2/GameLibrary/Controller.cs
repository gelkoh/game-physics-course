using Microsoft.Xna.Framework.Input;

namespace GameLibrary;

public abstract class Controller : Component
{
    public abstract void HandleInput(KeyboardState state);

    public override void Connect(GameObject gameObject)
    {
        base.Connect(gameObject);
        Core.Controllers.Add(this);
    }
        
    public override void Destroy()
    {
        Core.Controllers.Remove(this);
    }
}