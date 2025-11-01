using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameLibrary;
using GameLibrary.Physics;

namespace Racing
{
    /// <summary>
    /// The main class for the racing game, responsible for managing game components, settings, 
    /// and platform-specific configurations. Inherits from Core, which provides the update and draw loop.
    /// </summary>
    public class RacingGame : Core
    {
        private GameObject.Template _carTemplate;

        public RacingGame() : base("GameRacer", 1280, 720, false)
        {
        }

        /// <summary>
        /// Initializes the game, including setting up localization and adding the 
        /// initial screens to the ScreenManager.
        /// </summary>
        protected override void Initialize()
        {

            base.Initialize();
            _carTemplate.Instantiate(new Vector2(100, 400));
        }

        /// <summary>
        /// Loads game content, such as textures, the level and particle systems.
        /// Creates all the templates for initialization.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            TileMapCreator.LoadLevel("Content/Levels/level00.txt", Content);

            CreateTemplates();
        }

        /// <summary>
        /// Updates the game's logic, called once per frame.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values used for game updates.
        /// </param>
        protected override void Update(GameTime gameTime)
        {
            // Exit the game if the Back button (GamePad) or Escape key (Keyboard) is pressed.
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// Draws the game's graphics, called once per frame.
        /// </summary>
        /// <param name="gameTime">
        /// Provides a snapshot of timing values used for rendering.
        /// </param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        /// <summary>
        /// Creates needed templates for instantiation. 
        /// Templates contain all components of a game object such as texture, rigidbody, collider and controller.
        /// </summary>
        private void CreateTemplates()
        {
            _carTemplate = new GameObject.Template([
                new SpriteRenderer(Content.Load<Texture2D>("Sprites/car_yellow_1"), Color.White),
                new RigidBody(),
                new CarController(),
                new PointCollider()
            ]);
        }
    }
}