using System.Diagnostics;
using System.Net.Mime;
using GameLibrary;
using GameLibrary.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AngryMonkeys;

public class AngryMonkeysGame() : Core("AngryMonkeys", 1700, 950, false)
{
    protected override void Initialize()
    {
        base.Initialize();
        ClearColor = new Color(81, 168, 90);
        
        CreateBackground();
        CreateGround();

        CreateShootableObstacles();
        CreateMonkeyCannon();
    }

    private static void CreateBackground()
    {
        var backgroundTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(Content.Load<Texture2D>("background")).WithScale(1.7f).WithCenterOffset(new Vector2(0, -150)).Build(),
        ]);
        backgroundTemplate.Instantiate(new Vector2(Graphics.GraphicsDevice.Viewport.Width / 2, Graphics.GraphicsDevice.Viewport.Height / 2));
    }
    
    private static void CreateGround()
    {
        var groundTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(Content.Load<Texture2D>("Environment/elementWood012")).WithScale(8.5f).Build(),
            // Todo: Add a 2000x595 BoxCollider
        ]);
        groundTemplate.Instantiate(new Vector2(Graphics.GraphicsDevice.Viewport.Width / 2, Graphics.GraphicsDevice.Viewport.Height + 100));
    }

    private void CreateShootableObstacles()
    {
        var woodObstacleTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(Content.Load<Texture2D>("Environment/elementWood016")).Build(),
            new RigidBody(20f),
            // Todo: Add a 70x140 BoxCollider
        ]);
        var stoneObstacleTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(Content.Load<Texture2D>("Environment/elementStone017")).Build(),
            new RigidBody(40f),
            // Todo: Add a 70x140 BoxCollider
        ]);
        var alienGreenTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(Content.Load<Texture2D>("Aliens/alienGreen_suit")).WithScale(0.8f).Build(),
            new RigidBody(8f),
            // Todo: Add a 56x56 BoxCollider
        ]);
        var alienPinkTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(Content.Load<Texture2D>("Aliens/alienPink_round")).WithScale(0.8f).Build(),
            new RigidBody(6f),
            // Todo: Add a CircleCollider with a 28 radius
        ]);
        var woodenTriangleTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(Content.Load<Texture2D>("Environment/elementWood054")).Build(),
            new RigidBody(10f),
            // Todo: Add a Triangle collider with following corners: (-70, 35), (0, -35), (70, 35) these are relative positions to the center
        ]);

        var baseY = Graphics.GraphicsDevice.Viewport.Height - 260;
        
        stoneObstacleTemplate.Instantiate(new Vector2(1000, baseY));
        stoneObstacleTemplate.Instantiate(new Vector2(1140, baseY));
        stoneObstacleTemplate.Instantiate(new Vector2(1280, baseY));
        stoneObstacleTemplate.Instantiate(new Vector2(1420, baseY));
        
        woodObstacleTemplate.Instantiate(new Vector2(1070, baseY - 105), MathHelper.ToRadians(90));
        stoneObstacleTemplate.Instantiate(new Vector2(1210, baseY - 105), MathHelper.ToRadians(90));
        woodObstacleTemplate.Instantiate(new Vector2(1350, baseY - 105), MathHelper.ToRadians(90));
        
        woodObstacleTemplate.Instantiate(new Vector2(1070, baseY - 210));
        woodObstacleTemplate.Instantiate(new Vector2(1210, baseY - 210));
        woodObstacleTemplate.Instantiate(new Vector2(1350, baseY - 210));
        
        woodObstacleTemplate.Instantiate(new Vector2(1140, baseY - 315), MathHelper.ToRadians(90));
        woodObstacleTemplate.Instantiate(new Vector2(1280, baseY - 315), MathHelper.ToRadians(90));
        
        woodObstacleTemplate.Instantiate(new Vector2(1140, baseY - 420));
        woodObstacleTemplate.Instantiate(new Vector2(1280, baseY - 420));

        alienPinkTemplate.Instantiate(new Vector2(1140, baseY - 210));
        alienPinkTemplate.Instantiate(new Vector2(1280, baseY - 210));
        alienGreenTemplate.Instantiate(new Vector2(1210, baseY - 420));

        woodenTriangleTemplate.Instantiate(new Vector2(1210, baseY - 525));
    }
    
    private void CreateMonkeyCannon()
    {
        var flyingMonkeyTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(Content.Load<Texture2D>("Monkey")).WithScale(0.2f).Build(),
            new RigidBody(50f),
            // Todo: Add a CircleCollider with the radius 26
        ]); 
        var monkeyTemplate = new GameObject.Template([
            new SpriteRenderer.Builder(Content.Load<Texture2D>("Monkey")).WithScale(0.2f).Build(),
            new MonkeyController(Content, flyingMonkeyTemplate)
        ]);
        monkeyTemplate.Instantiate(new Vector2(200, Graphics.GraphicsDevice.Viewport.Height - 500));
    }
}