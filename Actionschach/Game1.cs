using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Actionschach {
    public class Game1 : Game {
        GraphicsDeviceManager graphics; 
        SpriteBatch spriteBatch;

       
      
        

        int d, e;
        
    
       static float zoom=45;
        static float zeit = 0;
        static float zeit2 = 0;
        
      
        //globale Hilfsvariablen
        bool klick = false;
        bool zugErfolgt = false;
        int itmp=-1;
        int wartemal = 0;
        Matrix TMP;  // zum Zwischenspeichern der temporären Weltmatrix
        

        //Brett
        private Model brett; //scale=1
       private Matrix view = Matrix.CreateLookAt(new Vector3(0, -1.6f, 1.25f), new Vector3(0, -0.28f, 0), Vector3.UnitY);

       
        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(zoom),2
           , 1f, 1000f);

        //Figuren
        private Matrix[] worldf = new Matrix[32];   
        private Model[] figur= new Model[32];   //scale:-0,11

        //Zeiger
        static Texture2D zeiger;
        Vector2 Maus=new Vector2(0,0);
        private Model maus;    //scale:0,1
        private Matrix[] worldm = new Matrix[64];

        //Raster
        private Vector2 raster(int x, int y){
            Vector2 tmp;
            tmp.X = (-7 + 2 * x) * 0.1245f;
            tmp.Y = (-7 + 2 * y) * 0.1245f;
            return tmp;
        }

        //Mousepicking
        public Ray CalculateRay(Vector2 maus_, Matrix view_, Matrix projection_, Viewport viewport_) {
            Vector3 nearPoint= viewport_.Unproject(new Vector3 (maus_.X,maus_.Y,0.0f),projection_,view_,
                Matrix.Identity);
            Vector3 farPoint = viewport_.Unproject(new Vector3(maus_.X, maus_.Y, 1), projection_, view_,
                Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }
        public float? IntersectDistance(BoundingSphere sphere, Vector2 maus_,Matrix view_, Matrix projection_,
            Viewport viewport_) {
            Ray mouseRay = CalculateRay(maus_, view_, projection_, viewport_);
            return mouseRay.Intersects(sphere);
        }
        public bool Intersects(Vector2 maus_, Model model, Matrix world_, Matrix view_, Matrix projection_,
            Viewport viewport_) {
            for (int i = 0; i < model.Meshes.Count; i++) {
                BoundingSphere sphere = model.Meshes[i].BoundingSphere;
                sphere = sphere.Transform(world_);
                 float? distance = IntersectDistance(sphere, maus_, view_, projection_, viewport_);
                 if (distance != null)
                 return true;
            }
            return false;
        }//</mousepicking>
        
        //testet ob Schachfeld belegt ist
        public bool belegt(Matrix m) {
            bool a = false;
            for (int i = 0; i < 32; i++) {
                if (m == worldf[i]) { 
                a = true;
                    itmp = i; //Nummer der Schachfigur auf belegtem feld
                }
            }
            return a;
        } 


        public Game1(){
            graphics = new GraphicsDeviceManager(this);
            /*{
                PreferredBackBufferWidth = 1024,
                PreferredBackBufferHeight = 720; };*/
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
         }

        protected override void Initialize() {
            int k = 0; 
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    worldm[k] = Matrix.CreateTranslation(raster(j,i).X,raster(j,i).Y, 0);
                    k++;
                }
            }

            //Startaufstellung
            for (int i = 0; i < 8; i++)
            {
                worldf[i] = Matrix.CreateTranslation(new Vector3(raster(i, 0).X, raster(i, 0).Y, 0));
            }
            for (int i = 8; i < 16; i++)
            {
                worldf[i] = Matrix.CreateTranslation(new Vector3(raster(i - 8, 1).X, raster(i - 8, 1).Y, 0));
            }
            for (int i = 16; i < 24; i++)
            {
                worldf[i] = Matrix.CreateTranslation(new Vector3(raster(i - 16, 6).X, raster(i - 16, 6).Y, 0));
            }
            for (int i = 24; i < 32; i++)
            {
                worldf[i] = Matrix.CreateTranslation(new Vector3(raster(i - 24, 7).X, raster(i - 24, 7).Y, 0));
            }


         base.Initialize();
        }

        protected override void LoadContent() {
            Content = new ContentManager(this.Services, "Content");

           /* startButton = this.Content.Load<Texture2D>("StartButton");
            skinButton = this.Content.Load<Texture2D>("SkinButton");
            cursor = this.Content.Load<Texture2D>("Cursor");*/

            spriteBatch = new SpriteBatch(GraphicsDevice);

            
            brett = Content.Load<Model>("chessboard");

            for(int i=0;i<32;i++)
            figur[i]= Content.Load<Model>("turmneu");
          
                maus = Content.Load<Model>("kugel");
          zeiger = Content.Load<Texture2D>("Cursor");
        }



        protected override void Update(GameTime gameTime) {
           GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            
            //Spiel verlassen
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //immer aufm Boden bleiben
            for (int i = 0; i < 32; i++)
                worldf[i].M43 = 0;
            if (gameTime.ElapsedGameTime.TotalMinutes > 10)
                ResetElapsedTime();

            /*Der registriert immer zu viele Tasten- oder Mausdruecke!!*/

            
                if (Keyboard.GetState().IsKeyDown(Keys.Enter)){
                zoom = 45;
            }


            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
            {
                zoom += 0.1f;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Add))
            {
                zoom -= 0.1f;
            }


            //Zoom zoomt richtung mitte aber nicht auf 0,0,0
            
            try
            {
                projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(zoom), 2
                  , 1f, 1000f);
            }
            catch (System.ArgumentException) {
                if (zoom <= 0)
                    zoom = 0.1f;
                if (zoom >= Math.PI)
                    zoom = (float)Math.PI - 0.1f;
            }

            /*
             * Tastatursteuerung etwas schwierig
             * Der registiriert immer zu viele Tastendrücke
             * 
                if (Keyboard.GetState().IsKeyDown(Keys.S)) { 
                    d++;
                { if (d == 1)
                        Schachfigur.setzen();
                }
                if (d >= 5)
                    d = 0;
            }*/

              
        //<Maus>
        Maus = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Viewport viewport = this.GraphicsDevice.Viewport;
            if (!klick){
                for (int i = 0; i < 64; i++){
                    if (Intersects(Maus, maus, worldm[i], view, projection, viewport) && belegt(worldm[i])){
                         worldf[itmp].M43 = 0.1f;
                        Console.WriteLine(itmp);
                         if (Mouse.GetState().LeftButton == ButtonState.Pressed){
                            klick = true;TMP = worldm[i];
                        }
                    }
                }
            }else {
                worldf[itmp].M43 = 0.5f;
                worldf[itmp] = Matrix.CreateRotationZ((float)gameTime.ElapsedGameTime.TotalSeconds) * worldf[itmp];
            }

            if (klick && Keyboard.GetState().IsKeyDown(Keys.Space)){
                wartemal++;
                if (wartemal >= 5){
                    worldf[itmp] = TMP;
                    wartemal = 0;
                    klick = false;
                    itmp = -1;
                }
            }
            for (int i = 0; i < 64; i++){
                if (klick && Intersects(Maus, maus, worldm[i], view, projection, viewport) 
                && Mouse.GetState().RightButton == ButtonState.Pressed) {
                    worldf[itmp]=worldm[i];
                    klick = false;
                    itmp = -1;
                    zugErfolgt = true;
                }              
          }//</Maus>

          /*  if (zugErfolgt) {
            float t= (float)gameTime.ElapsedGameTime.TotalMinutes;
                float t1 = (float)gameTime.TotalGameTime.TotalMinutes+(float)Math.PI;
                if (t<t1)
                for (int i=0;i<32;i++)
                    worldf[i] = worldf[i] * Matrix.CreateRotationZ((float)gameTime.ElapsedGameTime.TotalMinutes);

                worldf[5]=worldf[5]*Matrix.CreateRotationZ((float)Math.PI);
                zugErfolgt = false;
            }
            */

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            graphics.GraphicsDevice.Clear(Color.DeepSkyBlue);
            DrawModel(brett, world, view, projection);
            for (int i=0;i<32;i++)
            DrawModel(figur[i], worldf[i], view, projection);
           spriteBatch.Begin();
           spriteBatch.Draw(zeiger, Maus);
           spriteBatch.End();
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