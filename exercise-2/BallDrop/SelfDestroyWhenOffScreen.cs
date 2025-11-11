using GameLibrary;

namespace BallDrop;

public class SelfDestroyWhenOffScreen : UpdateComponent
{
    public Action OnDestroy;

    private readonly float _destroyHeight;

    public SelfDestroyWhenOffScreen()
    {
        _destroyHeight = Core.Graphics.PreferredBackBufferHeight + 40f;
    }

    public override void Update(double deltaTime)
    {
        if (GameObject.Position.Y > _destroyHeight)
        {
            OnDestroy?.Invoke();
            GameObject.Destroy();
        }
    }
}