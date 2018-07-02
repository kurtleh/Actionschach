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
        public Matrix projectionMatrix;
        public Matrix viewMatrix;
        Matrix worldMatrix;
        Matrix figurMatrix;

        Schachfigur test;
        public Model schachbrett;
        public Model figur;
        Schachfigur[] w=new Schachfigur[8]; //weiße Figuren
        Schachfigur[] b = new Schachfigur[8]; //schwarze Figuren

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this){
                PreferredBackBufferWidth=1024,
                PreferredBackBufferHeight=720 };
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
            _state = GameState.MainMenu;
            camTarget = new Vector3(0f, 0f, 0f);
            camPosition = new Vector3(0f, -5f, -100f);
            figurPos = new Vector3(1f, 0f, 0f);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45f),
                               GraphicsDevice.DisplayMode.AspectRatio,1f, 1000f);
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget,
                         new Vector3(0f, 1f, 0f));
            worldMatrix = Matrix.CreateWorld(camTarget, Vector3.Forward, Vector3.Up);
            figurMatrix = Matrix.CreateWorld(figurPos, Vector3.Forward, Vector3.Up);
              // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            base.Initialize();
            float j = -14;
            for (int i = 0; i < 8; i++)
            {
                w[i] = new Schachfigur(new Vector3(10f, j, 0f), figur);
                j += 4;
            }


    }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
         
            startButton = this.Content.Load<Texture2D>("StartButton");
            skinButton = this.Content.Load<Texture2D>("SkinButton");
            cursor = this.Content.Load<Texture2D>("Cursor");
            
            schachbrett = Content.Load<Model>("schachbrett");
            figur = Content.Load<Model>("coin2");
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
        position.Y < 350 &&
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
            if (Keyboard.GetState().IsKeyDown(Keys.T))
            {
                test.move(new Vector3(-5f, 3f, 0f));
            }
             if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                camPosition = Vector3.Normalize(camPosition + Vector3.Normalize(Vector3.Cross(new Vector3(0, 0, 2), camPosition))) * camPosition.Length();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                camPosition = Vector3.Normalize(camPosition - Vector3.Normalize(Vector3.Cross(new Vector3(0,0,2),camPosition))) * camPosition.Length();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (camPosition.X == 0 && camPosition.Y == 0)
                {
                    camPosition = Vector3.Normalize(camPosition - (new Vector3(0, 0, 2))) * camPosition.Length();
                }
                else
                {
                    camPosition = Vector3.Normalize(camPosition - (new Vector3(0, 0, 2))) * camPosition.Length();
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
               camPosition = Vector3.Normalize(camPosition + (new Vector3(0, 0, 2))) * camPosition.Length();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                camPosition -= Vector3.Normalize(camPosition);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                camPosition += Vector3.Normalize(camPosition);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))    //Standard Kameraposition
            {
                camPosition.X = 0;
                camPosition.Y = -5f;
                camPosition.Z = -100f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                figurPos.X -= 1f;
            }
            if (camPosition.Z > -3f)          //Grenze fürs Ranzoomen
            {
                float t = camPosition.Length();
                camPosition.Z = -3f;
                camPosition = Vector3.Normalize(camPosition) * t;
            }
            for (int i = 0; i < 8; i++){
                w[i].update(deltaTime);
            }
            figurMatrix = Matrix.CreateWorld(figurPos, Vector3.Forward, Vector3.Up);
            viewMatrix = Matrix.CreateLookAt(camPosition, camTarget,
                         Vector3.Up);
        }
         
         void UpdateSkinMenu(GameTime deltaTime)
         {

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
            for(int i = 0; i < 8; i++)
            {
                w[i].draw(deltaTime, viewMatrix, projectionMatrix);
            }
        }
        
        void DrawSkinMenu(GameTime deltaTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(skinButton, destinationRectangle: new Rectangle(300, 50, 150, 100));
            spriteBatch.End();
        }
        public class Schachfigur
        {
            public Schachfigur(Vector3 pos,Model mod)
            {
                position = pos;
                moving = false;
                destiny = pos;
                alive = true;
                m = mod;
            }

            public Matrix getPosition()
            {
                return Matrix.CreateWorld(this.position, Vector3.Forward, Vector3.Up);
            }

            public void move(Vector3 ziel)
            {
                destiny = ziel;
                moving = true;
            }

            public void update(GameTime deltatime)
            {
                if (moving)
                {
                    if (position == destiny)
                    {
                        moving = false;
                    }
                    else
                    {
                        Vector3 direction = Vector3.Normalize(destiny - position);
                        if ((destiny - position).Length() > destiny.Length())
                        {
                            position = position + direction;
                        }
                        else
                        {
                            position = destiny;
                        }
                    }
                }
            }

            public void draw(GameTime DeltaTime,Matrix viewMatrix,Matrix projectionMatrix)
            {
                foreach (ModelMesh mesh in this.m.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        //effect.EnableDefaultLighting();
                        effect.AmbientLightColor = new Vector3(1f, 0, 0);
                        effect.View = viewMatrix;
                        effect.World = Matrix.CreateWorld(position, Vector3.Forward, Vector3.Up);
                        effect.Projection = projectionMatrix;
                    }
                    mesh.Draw();
                }
            }


            private Vector3 position;
            private Vector3 destiny;
            private bool moving;
            Model m;
            bool alive;


        }
    }
}
