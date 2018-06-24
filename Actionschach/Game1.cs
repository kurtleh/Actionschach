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

        //Camera
        Vector3 camTarget;
        Vector3 camPosition;
        Vector3 figurPos;
        Matrix projectionMatrix;
        Matrix viewMatrix;
        Matrix worldMatrix;
        Matrix figurMatrix;

        Model schachbrett;
        Model figur;

        //Orbit
        bool orbit = false;

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

            //Setup Camera
            camTarget = new Vector3(0f, 0f, 0f);
            camPosition = new Vector3(0f, 0f, -100f);
            figurPos = new Vector3(1f, 0f, 0f);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45f),
                               GraphicsDevice.DisplayMode.AspectRatio,1f, 1000f);
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget,
                         new Vector3(0f, 1f, 0f));// Y up
            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);
            figurMatrix = Matrix.CreateWorld(figurPos, Vector3.Forward, Vector3.Up);
            schachbrett = Content.Load<Model>("schachbrett");
            figur = Content.Load<Model>("coin");
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
            

            // TODO: Add your update logic here
            
            base.Update(gameTime);
            switch (_state)
            {
                case GameState.MainMenu:
                    UpdateMainMenu(gameTime);
                    break;
                case GameState.Gameplay:
                    UpdateGameplay(gameTime);
                    break;
                case GameState.Skinmenu:
                    UpdateSkinMenu(gameTime);
                    break;
            }
            if (state.LeftButton == ButtonState.Pressed)    //für klicks
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
        position.Y > 50 && state.LeftButton == ButtonState.Pressed && lastmousestate == ButtonState.Released)
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
        position.Y > 200 && state.LeftButton==ButtonState.Pressed && lastmousestate==ButtonState.Released)
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
            if (pressedStartbutton())
                _state = GameState.Gameplay;
         }
        
         void UpdateGameplay(GameTime deltaTime)
         {
             if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                camPosition.X -= 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                camPosition.X += 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                camPosition.Y -= 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                camPosition.Y += 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                camPosition.Z += 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                camPosition.Z -= 1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                camPosition.X = 0;
                camPosition.Y = 0;
                camPosition.Z = -10f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                figurPos.X -= 1f;
            }
            figurMatrix = Matrix.CreateWorld(figurPos, Vector3.Forward, Vector3.Up);
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget,
                         Vector3.Up);
        }
         
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
                case GameState.Gameplay:
                    DrawGameplay(gameTime);
                    break;
                case GameState.Skinmenu:
                    DrawSkinMenu(gameTime);
                    break;
            }
            spriteBatch.Begin();
            spriteBatch.Draw(cursor, position, origin: new Vector2(0, 0));
            spriteBatch.End();

        }

         void DrawMainMenu(GameTime deltaTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(startButton, destinationRectangle: new Rectangle(300, 50, 150, 100));
            spriteBatch.Draw(skinButton, destinationRectangle: new Rectangle(300, 200, 150, 100));
            //spriteBatch.Draw(cursor, position, origin: new Vector2(0, 0));
            spriteBatch.End();
        }

       void DrawGameplay(GameTime deltaTime)
        {
            foreach (ModelMesh mesh in schachbrett.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    effect.AmbientLightColor = new Vector3(1f, 0, 0);
                    effect.View = viewMatrix;
                    effect.World = worldMatrix;
                    effect.Projection = projectionMatrix;
                }
                mesh.Draw();
            }
            foreach (ModelMesh mesh in figur.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                    effect.AmbientLightColor = new Vector3(1f, 0, 0);
                    effect.View = viewMatrix;
                    effect.World = figurMatrix;
                    effect.Projection = projectionMatrix;
                }
                mesh.Draw();
            }
        }
        
        void DrawSkinMenu(GameTime deltaTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(skinButton, destinationRectangle: new Rectangle(300, 50, 150, 100));
            //spriteBatch.Draw(cursor, position, origin: new Vector2(0, 0));
            spriteBatch.End();
        }
    }
}
