namespace GameLibrary;

public abstract class UpdateComponent : Component
{
    public bool DestroyEndOfFrame { get; private set; }

    public override void Connect(GameObject gameObject)
    {
        base.Connect(gameObject);
        Core.UpdateComponents.Add(this);
    }

    public override void Destroy()
    {
        DestroyEndOfFrame = true;
        base.Destroy();
    }

    public abstract void Update(double deltaTime);
}