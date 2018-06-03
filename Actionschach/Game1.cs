using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Text;

namespace Actionschach
{

    enum GameState
    {
        MainMenu,
        Gameplay,
        Skinmenu,
    }
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameState _state;
        Vector2 position;
        Texture2D cursor;
        Texture2D startButton;
        Texture2D skinButton;
        ButtonState lastmousestate;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

           
            base.Initialize();
            position = new Vector2(graphics.GraphicsDevice.Viewport.
                      Width / 2,
                                   graphics.GraphicsDevice.Viewport.
                                   Height / 2);

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            startButton = this.Content.Load<Texture2D>("StartButton");
            skinButton = this.Content.Load<Texture2D>("SkinButton");
            cursor = this.Content.Load<Texture2D>("Cursor");
            _state = GameState.MainMenu;
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            MouseState state = Mouse.GetState();
            position.X = state.X;
            position.Y = state.Y;
            if (state.RightButton == ButtonState.Pressed)
                Exit();

            // TODO: Add your update logic here
           
            base.Update(gameTime);
            switch (_state)
            {
                case GameState.MainMenu:
                    UpdateMainMenu(gameTime);
                    break;
              /*  case GameState.Gameplay:
                    UpdateGameplay(gameTime);
                    break;*/
                case GameState.Skinmenu:
                    UpdateSkinMenu(gameTime);
                    break;
            }
            if (state.LeftButton == ButtonState.Pressed)
            {
                lastmousestate = ButtonState.Pressed;
            }
            else
            {
                lastmousestate = ButtonState.Released;
            }
        }

        public bool pressedStartbutton()
        {
            MouseState state = Mouse.GetState();
            position.X = state.X;
            position.Y = state.Y;
            if (position.X < 450  &&
        position.X > 300 &&
        position.Y < 150 &&
        position.Y > 50)
            {
                return true;
            }
            return false;
        }

        public bool pressedSkinbutton()
        {
            MouseState state = Mouse.GetState();
            position.X = state.X;
            position.Y = state.Y;
            if (position.X < 450 &&
        position.X > 300 &&
        position.Y < 150 &&
        position.Y > 50 && state.LeftButton==ButtonState.Pressed && lastmousestate==ButtonState.Released)
            {
                return true;
            }
            return false;
        }

        public bool pressedMenubutton()
        {
            MouseState state = Mouse.GetState();
            position.X = state.X;
            position.Y = state.Y;
            if (position.X < 450 &&
        position.X > 300 &&
        position.Y < 150 &&
        position.Y > 50 && state.LeftButton == ButtonState.Pressed && lastmousestate == ButtonState.Released)
            {
                return true;
            }
            return false;
        }


        void UpdateMainMenu(GameTime deltaTime)
         {
             // Respond to user input for menu selections, etc
             if (pressedSkinbutton())
                 _state = GameState.Skinmenu;
         }
        /*
         void UpdateGameplay(GameTime deltaTime)
         {
             // Respond to user actions in the game.
             // Update enemies
             // Handle collisions
             if (pushedSkinButton)
                 _state = GameState.Skinmenu;
         }
         */
         void UpdateSkinMenu(GameTime deltaTime)
         {
             // Update scores
             // Do any animations, effects, etc for getting a high score
             // Respond to user input to restart level, or go back to main menu
             if (pressedMenubutton())
                 _state = GameState.MainMenu;
         }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

        // TODO: Add your drawing code here
        

        base.Draw(gameTime);
            switch (_state)
            {
                case GameState.MainMenu:
                    DrawMainMenu(gameTime);
                    break;
               // case GameState.Gameplay:
               //     DrawGameplay(gameTime);
               //     break;
                case GameState.Skinmenu:
                    DrawSkinMenu(gameTime);
                    break;
            }
        }

         void DrawMainMenu(GameTime deltaTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(startButton, destinationRectangle: new Rectangle(300, 50, 150, 100));
            spriteBatch.Draw(cursor, position, origin: new Vector2(0, 0));
            spriteBatch.End();
        }

     /*   void DrawGameplay(GameTime deltaTime)
        {
            // Draw the background the level
            // Draw enemies
            // Draw the player
            // Draw particle effects, etc
        }
        */
        void DrawSkinMenu(GameTime deltaTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(skinButton, destinationRectangle: new Rectangle(300, 50, 150, 100));
            spriteBatch.Draw(cursor, position, origin: new Vector2(0, 0));
            spriteBatch.End();
        }
    }
}
