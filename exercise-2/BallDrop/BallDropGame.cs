using GameLibrary;
using GameLibrary.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BallDrop;

public class BallDropGame() : Core("BallDrop", 600, 960, false)
{
    protected override void Initialize()
    {
        base.Initialize();
        ClearColor = new Color(225, 232, 240);
        
        var levelEnvironment = new LevelEnvironment(Content);
        levelEnvironment.Build();
        
        CreateBallCannon();
    }

    private void CreateBallCannon()
    {
        var ballTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(Content.Load<Texture2D>("red_circle")).WithScale(0.5f).Build(),
            new RigidBody(5f),
            new SelfDestroyWhenOffScreen(),
            // Todo: Add a circle collider => the size of the sprite in game is 40x40
            new CircleCollider(20f, 0.5f)
        ]);
        var ballCannonTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(Content.Load<Texture2D>("UI/arrow")).WithCenterOffset(new Vector2(0, 30)).Build(),
            new BallCannonController(ballTemplate)
        ]);
        ballCannonTemplate.Instantiate(new Vector2(300, 70));
    }
}