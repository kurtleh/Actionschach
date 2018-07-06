using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

/*Wichtig: Skalierung im Pipeline-Tool anpassen !!*/

namespace Schach
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Model brett;
        private Model blauwurfl;
        private Model grunwurfl;
        float a, b, c;

        //private Matrix view = Matrix.CreateLookAt(new Vector3(0, -1.6f, 1.25f), new Vector3(0, -0.3f, 0), Vector3.UnitY);
        private Matrix view = Matrix.CreateLookAt(new Vector3(0, -1.6f, 1.25f), new Vector3(0, -0.3f, 0), Vector3.UnitY);

        private Matrix world = Matrix.CreateTranslation(new Vector3(0,0,0));


        
         private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),2
           , 1f, 1000f);
        private Matrix p1 = Matrix.CreateLookAt(new Vector3(0, -5f, -100f), new Vector3(0, 0f, 0), Vector3.UnitY);
        private Matrix worldp1 = Matrix.CreateTranslation(new Vector3(0,0,0));
        private Matrix projection2 = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 2f, 0.5f, 100f);

        //private Matrix sk = Matrix.CreateScale(new Vector3(0.5f, 0.5f, 0.5f));
        //private Matrix skview = Matrix.CreateScale(new Vector3(0,0,0));




        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
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
            /*_state = GameState.MainMenu;
            camTarget = new Vector3(0f, 0f, 0f);
            camPosition = new Vector3(0f, -5f, -100f);
            figurPos = new Vector3(1f, 0f, 0f);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                               MathHelper.ToRadians(45f),
                               GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f);
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
                w[i] = new Schachfigur(new Vector3(10f, j, 0f), coinw);
                b[i] = new Schachfigur(new Vector3(-10f, j, 0f), coinb);
                j += 4;
            }
            isselected = false;
                */
            // TODO: Add your initialization logic here
           
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Content = new ContentManager(this.Services, "Content");

           /* startButton = this.Content.Load<Texture2D>("StartButton");
            skinButton = this.Content.Load<Texture2D>("SkinButton");
            cursor = this.Content.Load<Texture2D>("Cursor");*/

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //model = Content.Load<Model>("untitled");
            brett = Content.Load<Model>("chessboard");
            blauwurfl = Content.Load<Model>("blwu");
           grunwurfl = Content.Load<Model>("grunerwurfl");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // For Mobile devices, this logic will close the Game when the Back button is pressed


            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                Exit();

            }

             if (Keyboard.GetState().IsKeyDown(Keys.X))
               {
                   a=a+0.05f;
               }
               if (Keyboard.GetState().IsKeyDown(Keys.Y))
               {
                   a=a-0.05f;
               }
                if (Keyboard.GetState().IsKeyDown(Keys.C))
               {
                   b=b+0.05f;
               }
               if (Keyboard.GetState().IsKeyDown(Keys.V))
               {
                   b=b-0.05f;
               }
                if (Keyboard.GetState().IsKeyDown(Keys.B))
               {
                   c=c+0.05f;
               }
               if (Keyboard.GetState().IsKeyDown(Keys.N))
               {
                   c=c-0.05f;
               }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                a = 0;b = 0;c = 0;
            }
            worldp1 = Matrix.CreateTranslation(new Vector3(a, b, c));


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState=DepthStencilState.Default;
            DrawModel(brett, world, view, projection);
            DrawModel(grunwurfl, worldp1, view, projection);
           // DrawModel(blauwurfl,world,view,projection2);
           
           // spriteBatch.Begin();
            
            //spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawModel(Model model_, Matrix world_, Matrix view_, Matrix projection_)
        {


       foreach (ModelMesh mesh in model_.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                   
                    effect.World = world_;
                  
                    effect.View = view_;
                    effect.Projection = projection_;
                }

                mesh.Draw();
            }
        }
    }
}