using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameLibrary.Physics;

namespace GameLibrary;

/// <summary>
/// The base class for any physics game with an update and draw loop.
/// </summary>
public class Core : Game
{
    internal static Core instance;

    /// <summary>
    /// Gets a reference to the Core instance.
    /// </summary>
    public static Core Instance => instance;

    /// <summary>
    /// Gets the graphics device manager to control the presentation of graphics.
    /// </summary>
    public static GraphicsDeviceManager Graphics { get; private set; }

    /// <summary>
    /// Gets the graphics device used to create graphical resources and perform primitive rendering.
    /// </summary>
    public static new GraphicsDevice GraphicsDevice { get; private set; }

    /// <summary>
    /// Gets the sprite batch used for all 2D rendering.
    /// </summary>
    public static SpriteBatch SpriteBatch { get; private set; }

    /// <summary>
    /// Gets the content manager used to load global assets.
    /// </summary>
    public static new ContentManager Content { get; private set; }

    /// <summary>
    /// Gets or sets the game background color.
    /// </summary>
    public Color ClearColor { get; set; } = Color.MonoGameOrange;

    /// <summary>
    /// List of all active SpriteRenderers to be drawn in the Draw-Function.
    /// </summary>
    public static readonly List<SpriteRenderer> ActiveRenderers = [];
    /// <summary>
    /// List of all Controllers that handle player input in the Update-Function.
    /// </summary>
    public static readonly List<Controller> Controllers = [];
    /// <summary>
    /// List of all components that get have an Update-Method that gets called every frame.
    /// </summary>
    public static readonly List<UpdateComponent> UpdateComponents = [];
    /// <summary>
    /// The PhysicsWorld that applies physics to all RigidBody-Components in the Update-Function.
    /// </summary>
    private PhysicsWorld _physics;

    /// <summary>
    /// Creates a new Core instance.
    /// </summary>
    /// <param name="title">The title to display in the title bar of the game window.</param>
    /// <param name="width">The initial width, in pixels, of the game window.</param>
    /// <param name="height">The initial height, in pixels, of the game window.</param>
    /// <param name="fullScreen">Indicates if the game should start in fullscreen mode.</param>
    public Core(string title, int width, int height, bool fullScreen)
    {
        if (instance != null)
        {
            throw new InvalidOperationException($"Only a single Core instance can be created");
        }
        instance = this;

        Graphics = new GraphicsDeviceManager(this)
        {
            PreferredBackBufferWidth = width,
            PreferredBackBufferHeight = height,
            IsFullScreen = fullScreen
        };
        Graphics.ApplyChanges();
        IsMouseVisible = true;

        Window.Title = title;
        Content = base.Content;
        Content.RootDirectory = "Content";

        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1 / 60.0);
    }

    protected override void Initialize()
    {
        base.Initialize();
        _physics = new PhysicsWorld();
        GraphicsDevice = base.GraphicsDevice;
        SpriteBatch = new SpriteBatch(GraphicsDevice);
    }

    /// <summary>
    /// Called every frame. Handles player input and physics logic.
    /// </summary>
    /// <param name="gameTime">Records time elapsed since last frame and start of the game.</param>
    protected override void Update(GameTime gameTime)
    {
        _physics.Update(gameTime.ElapsedGameTime.TotalSeconds);
        foreach (var controller in Controllers)
        {
            controller.HandleInput(Keyboard.GetState());
        }
        foreach (var updateComponent in UpdateComponents)
        {
            if (updateComponent.DestroyEndOfFrame)
                continue;
            updateComponent.Update(gameTime.ElapsedGameTime.TotalSeconds);
        }

        for (var i = UpdateComponents.Count - 1; i >= 0; i--)
        {
            if (UpdateComponents[i].DestroyEndOfFrame)
                UpdateComponents.RemoveAt(i);
        }

        base.Update(gameTime);
    }

    /// <summary>
    /// Called every frame after Update(). Renders Sprites.
    /// </summary>
    /// <param name="gameTime">Records time elapsed since last frame and start of the game.</param>
    protected override void Draw(GameTime gameTime)
    {
        // Clears the screen with the MonoGame orange color before drawing.
        GraphicsDevice.Clear(ClearColor);

        SpriteBatch.Begin();

        foreach (SpriteRenderer s in ActiveRenderers)
        {
            SpriteBatch.Draw(s.Texture, s.Position, null, s.Color, s.GameObject.Rotation, Vector2.Zero, s.Scale, SpriteEffects.None, 0.0f);
        }

        SpriteBatch.End();
        base.Draw(gameTime);
    }
}
