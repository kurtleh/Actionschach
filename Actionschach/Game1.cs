using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Actionschach {

    enum GameState
    {
        MainMenu,
        Gameplay,
        Skinmenu,
        WhiteWinner,
        BlackWinner,
    }

    public enum Team
    {
        blau,
        rot
    }

    public enum Figurentyp
    {
        Bauer,
        Turm,
        Springer,
        Laeufer,
        Queen,
        King
    }
    public class Game1 : Game {
        GraphicsDeviceManager graphics; 
        SpriteBatch spriteBatch;
        SpriteFont text;
        Vector2 textposition;
        GameState _state;
        Vector2 position;
        public String Ausgabe="";
        Schachfigur select;
        Texture2D startButton;
        Texture2D skinButton;
        Texture2D menueButton;
        Texture2D resetButton;
        int d, e;
       static float zoom=45;
        static float zeit = 0;
        static float zeit2 = 0;
        ButtonState lastmousestate;
        bool whiteturn = true;
        //Camera
        Vector3 camTarget;
        Vector3 camPosition =new Vector3(0, -1.6f, 1.25f);
        Vector3 figurPos;

        //Camera empfindlichkeit
        float c = 0.3f;

        //globale Hilfsvariablen
        bool klick = false;
        bool zugErfolgt = false;
        int itmp=-1;
        int wartemal = 0;
        Matrix TMP;  // zum Zwischenspeichern der temporären Weltmatrix
        

        //Brett
        private Model brett; //scale=1
        private Matrix view;

        //Modelle
        public Model bauermw;
        public Model turmmw;
        public Model springermw;
        public Model laufermw;
        public Model kingmw;
        public Model queenmw;
        public Model bauermb;
        public Model turmmb;
        public Model springermb;
        public Model laufermb;
        public Model kingmb;
        public Model queenmb;

        private Matrix world = Matrix.CreateTranslation(new Vector3(0, 0, 0));
        private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(zoom),2
           , 1f, 1000f);

        //Figuren
        //private Matrix[] worldf = new Matrix[32];   
        //private Model[] figur= new Model[32];   //scale:-0,11
        private Schachfigur[] figur=new Schachfigur[32];

        //Zeiger
        static Texture2D zeiger;
        Vector2 Maus=new Vector2(0,0);
        private Model maus;    //scale:0,1
        private Matrix[] worldm = new Matrix[64];

        //Raster
        public static Vector2 raster(Vector2 tmp){
            tmp.X = (-7 + 2 * tmp.X) * 0.1245f;
            tmp.Y = (-7 + 2 * tmp.Y) * 0.1245f;
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
                if (m.M41==figur[i].figurm.M41&&m.M42==figur[i].figurm.M42) { 
                a = true;
                    itmp = i; //Nummer der Schachfigur auf belegtem feld
                }
            }
            return a;
        } 


        public Game1(){
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1024,
                PreferredBackBufferHeight = 720
            };
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
         }

        protected override void Initialize() {
           _state = GameState.MainMenu;
            view = Matrix.CreateLookAt(camPosition, new Vector3(0, -0.28f, 0), Vector3.UnitY);
            int k = 0; 
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    worldm[k] = Matrix.CreateScale(-0.9f) * Matrix.CreateTranslation(raster(new Vector2(j,i)).X,raster(new Vector2(j,i)).Y, 0);
                    k++;
                }
            }
            //Startaufstellung
            figur[0] = new Schachfigur(0, 0, Figurentyp.Turm, true);
            figur[1] = new Schachfigur(1, 0, Figurentyp.Springer, true);
            figur[2] = new Schachfigur(2, 0, Figurentyp.Laeufer, true);
            figur[3] = new Schachfigur(3, 0, Figurentyp.Queen, true);
            figur[4] = new Schachfigur(4, 0, Figurentyp.King, true);
            figur[5] = new Schachfigur(5, 0, Figurentyp.Laeufer, true);
            figur[6] = new Schachfigur(6, 0, Figurentyp.Springer, true);
            figur[7] = new Schachfigur(7, 0, Figurentyp.Turm, true);
            for (int i = 0; i < 8; i++)
            {
                figur[i+8] =new Schachfigur(i,1, Figurentyp.Bauer,true);
                figur[i+16] =new Schachfigur(i,6, Figurentyp.Bauer,false);
            }
            figur[24] = new Schachfigur(0, 7, Figurentyp.Turm, false);
            figur[25] = new Schachfigur(1, 7, Figurentyp.Springer, false);
            figur[26] = new Schachfigur(2, 7, Figurentyp.Laeufer, false);
            figur[27] = new Schachfigur(3, 7, Figurentyp.King, false);
            figur[28] = new Schachfigur(4, 7, Figurentyp.Queen, false);
            figur[29] = new Schachfigur(5, 7, Figurentyp.Laeufer, false);
            figur[30] = new Schachfigur(6, 7, Figurentyp.Springer, false);
            figur[31] = new Schachfigur(7, 7, Figurentyp.Turm, false);


            base.Initialize();
        }
        public bool pressedSkinbutton()
        {
            MouseState state = Mouse.GetState();
            position.X = state.X;
            position.Y = state.Y;
            if (position.X < 587 &&
        position.X > 437 &&
        position.Y < 350 &&
        position.Y > 200 && click())
            {
                return true;
            }
            return false; ;
        }


public bool pressedStartbutton()
        {
            MouseState state = Mouse.GetState();
            position.X = state.X;
            position.Y = state.Y;
            if (position.X < 587  &&
        position.X > 437 &&
        position.Y < 150 &&
        position.Y > 50 && click())
            {
                return true;
            }
            return false;
        }

        public bool pressedWinnerMenuebutton()
        {
            MouseState state = Mouse.GetState();
            position.X = state.X;
            position.Y = state.Y;
            if (position.X < 331 &&
        position.X > 181 &&
        position.Y < 480 &&
        position.Y > 380 && click())
            {
                return true;
            }
            return false;
        }

        public bool pressedWinnerResetbutton()
        {
            MouseState state = Mouse.GetState();
            position.X = state.X;
            position.Y = state.Y;
            if (position.X < 843 &&
        position.X > 693 &&
        position.Y < 480 &&
        position.Y > 380 && click())
            {
                return true;
            }
            return false;
        }

        protected override void LoadContent() {
            Content = new ContentManager(this.Services, "Content");

             startButton = this.Content.Load<Texture2D>("StartButton");
             skinButton = this.Content.Load<Texture2D>("SkinButton");
            menueButton = this.Content.Load<Texture2D>("MenueButton");
            resetButton = Content.Load<Texture2D>("ResetButton");

            spriteBatch = new SpriteBatch(GraphicsDevice);
            text = Content.Load<SpriteFont>("text");
            textposition = new Vector2(0, 0);

            brett = Content.Load<Model>("chessboard");
            turmmw = Content.Load<Model>("turmblau");
            turmmb = Content.Load<Model>("turmrot");
            bauermw = Content.Load<Model>("bauerblau");
            bauermb = Content.Load<Model>("bauerrot");
            springermw = Content.Load<Model>("springerblau");
            springermb = Content.Load<Model>("springerrot");
            laufermw = Content.Load<Model>("lauferblau");
            laufermb = Content.Load<Model>("lauferrot");
            queenmw = Content.Load<Model>("dameblau");
            queenmb = Content.Load<Model>("damerot");
            kingmw = Content.Load<Model>("konigblau");
            kingmb = Content.Load<Model>("konigrot");

            for (int i = 8; i < 16; i++)
                figur[i].SetModel(bauermw);
            for (int i = 16; i < 24; i++)
                figur[i].SetModel(bauermb);
            figur[0].SetModel(turmmw);
            figur[1].SetModel(springermw);
            figur[2].SetModel(laufermw);
            figur[3].SetModel(queenmw);
            figur[4].SetModel(kingmw);
            figur[5].SetModel(laufermw);
            figur[6].SetModel(springermw);
            figur[7].SetModel(turmmw);
            figur[24].SetModel(turmmb);
            figur[25].SetModel(springermb);
            figur[26].SetModel(laufermb);
            figur[27].SetModel(queenmb);
            figur[28].SetModel(kingmb);
            figur[29].SetModel(laufermb);
            figur[30].SetModel(springermb);
            figur[31].SetModel(turmmb);
            maus = Content.Load<Model>("kugel");
            zeiger = Content.Load<Texture2D>("Cursor"); }

        public bool pressedMenubutton()
        {
            MouseState state = Mouse.GetState();
            position.X = state.X;
            position.Y = state.Y;
            if (position.X < 587 &&
        position.X > 437 &&
        position.Y < 150 &&
        position.Y > 50 && click())
            {
                return true;
            }
            return false;

        }

        public bool click()
        {
            MouseState state = Mouse.GetState();
            return (state.LeftButton == ButtonState.Pressed && lastmousestate == ButtonState.Released);
        }

        void UpdateMainMenu(GameTime deltaTime)
        {
            // Respond to user input for menu selections, etc
            if (pressedSkinbutton())
                _state = GameState.Skinmenu;
            if (pressedStartbutton())
                _state = GameState.Gameplay;
        }

        protected override void Update(GameTime gameTime) {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            MouseState state = Mouse.GetState();
            position.X = state.X;
            position.Y = state.Y;
            //Spiel verlassen
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
           

            /*Der registriert immer zu viele Tasten- oder Mausdruecke!!*/


            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                zoom = 45;
            }
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
                case GameState.WhiteWinner:
                    UpdateWhiteWinner(gameTime);
                    break;
                case GameState.BlackWinner:
                    UpdateBlackWinner(gameTime);
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
                

                void UpdateGameplay(GameTime deltaTime)
                {
            

            //Kamerasteuerung
            if (Keyboard.GetState().IsKeyDown(Keys.Left))  
            {
                camPosition = Vector3.Normalize(camPosition + c*Vector3.Normalize(Vector3.Cross(new Vector3(0, 0, 2), camPosition))) * camPosition.Length();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                camPosition = Vector3.Normalize(camPosition - c*Vector3.Normalize(Vector3.Cross(new Vector3(0, 0, 2), camPosition))) * camPosition.Length();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                if (camPosition.X == 0 && camPosition.Y == 0)
                {
                    camPosition = Vector3.Normalize(camPosition - (c*new Vector3(0, 0, 2))) * camPosition.Length();
                }
                else
                {
                    camPosition = Vector3.Normalize(camPosition - (c*new Vector3(0, 0, 2))) * camPosition.Length();
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                camPosition = Vector3.Normalize(camPosition + c*(new Vector3(0, 0, 2))) * camPosition.Length();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Space))    //Standard Kameraposition
            {
                camPosition.X = 0;
                camPosition.Y = -1.6f; 
                camPosition.Z = 1.25f;
            }
            if (camPosition.Z < 0f)          //Grenze fürs Ranzoomen
            {
                float t = camPosition.Length();
                camPosition.Z = 0f;
                camPosition = Vector3.Normalize(camPosition) * t;
            }
            view = Matrix.CreateLookAt(camPosition, new Vector3(0, -0.28f, 0), Vector3.UnitY);

            //immer aufm Boden bleiben
            for (int i = 0; i < 32; i++)
                figur[i].figurm.M43 = 0;
            if (deltaTime.ElapsedGameTime.TotalMinutes > 10)
                ResetElapsedTime();

            /*Der registriert immer zu viele Tasten- oder Mausdruecke!!*/

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
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
            catch (System.ArgumentException)
            {
                if (zoom <= 0)
                    zoom = 0.1f;
                if (zoom >= Math.PI)
                    zoom = (float)Math.PI - 0.1f;
            }

            //<Maus>
            Maus = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            Viewport viewport = this.GraphicsDevice.Viewport;
            if (whiteturn)
            {
                if (!klick)
                {
                    for (int i = 0; i < 64; i++)
                    {
                        if (Intersects(Maus, maus, worldm[i], view, projection, viewport) && belegt(worldm[i]))
                        {
                            if (figur[itmp].iswhite) { 
                            figur[itmp].figurm.M43 = 0.1f;
                            Console.WriteLine(itmp);
                            if (click())
                            {
                                klick = true;
                            }
                        }
                    }
                    }
                }
                else
                {
                    figur[itmp].figurm.M43 = 0.2f;
                    figur[itmp].figurm = Matrix.CreateRotationZ((float)deltaTime.ElapsedGameTime.TotalSeconds) * figur[itmp].figurm;
                }
            }
            else
            {
                if (!klick)
                {
                    for (int i = 0; i < 64; i++)
                    {
                        if (Intersects(Maus, maus, worldm[i], view, projection, viewport) && belegt(worldm[i]))
                        {
                            if (!figur[itmp].iswhite)
                            {
                                figur[itmp].figurm.M43 = 0.1f;
                                Console.WriteLine(itmp);
                                if (click())
                                {
                                    klick = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    figur[itmp].figurm.M43 = 0.2f;
                    figur[itmp].figurm = Matrix.CreateRotationZ((float)deltaTime.ElapsedGameTime.TotalSeconds) * figur[itmp].figurm;
                }
            }
            if (klick && Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                wartemal++;
                if (wartemal >= 5)
                {
                    figur[itmp].figurm.M43 = 0;
                    wartemal = 0;
                    klick = false;
                    itmp = -1;
                }
            }
            for (int i = 0; i < 64; i++)
            {
                if (klick && Intersects(Maus, maus, worldm[i], view, projection, viewport)
                && Mouse.GetState().RightButton == ButtonState.Pressed && !besetzt(worldm[i]))
                {
                    if(figur[itmp].possiblemove(feld(i),this)){
                        figur[itmp].move(feld(i), this, i);
                    klick = false;
                    itmp = -1;
                    zugErfolgt = true;
                        whiteturn = !whiteturn;
                }
                }
            }//</Maus>

            //nach jedem Zug soll die komplette welt um 180grd gedreht werden
            /*  irgendwie so oder auch nicht:
             *  if (zugErfolgt) {
              float t= (float)gameTime.ElapsedGameTime.TotalMinutes;
                  float t1 = (float)gameTime.TotalGameTime.TotalMinutes+(float)Math.PI;
                  if (t<t1)
                  for (int i=0;i<32;i++)
                      worldf[i] = worldf[i] * Matrix.CreateRotationZ((float)gameTime.ElapsedGameTime.TotalMinutes);

                  worldf[5]=worldf[5]*Matrix.CreateRotationZ((float)Math.PI);
                  zugErfolgt = false;
              }
              */

            for (int i = 8; i < 16; i++)
            {
                if (figur[i].position.Y == 7)
                    figur[i].bauerntausch(this);
            }
            for (int i = 16; i < 24; i++)
            {
                if (figur[i].position.Y == 0)
                    figur[i].bauerntausch(this);
            }
            for (int i=0;i<32; i++)
            {
                figur[i].update(deltaTime);
            }

            Ausgabe = "";
            for(int i=0;i<16; ++i)
            {
                if (figur[i].possiblemove(figur[28].position, this))
                {
                    Ausgabe += "Schach für schwarzen Koenig\n";
                }
            }
            for (int i = 16; i < 32; ++i)
            {
                if (figur[i].possiblemove(figur[4].position, this))
                {
                    Ausgabe += "Schach für weissen Koenig\n";
                }
            }
        }

        /*  public void figurselection(GameTime deltaTime)
          {
              Vector3 t = GetVector();
              select = GetFigur(t);
              if (select != null)
                  isselected = true;
          }*/

        /* public Schachfigur GetFigur(Vector3 v)
         {
             for(int i = 0; i < w.Length; i++)
             {
                 if (v.Equals(w[i].position))
                 {
                     return w[i];
                 }
                 if (v.Equals(b[i].position))
                 {
                     return b[i];
                 }
             }
             return null;
         }*/
        bool besetzt(Matrix m)
        {
            bool a = false;
            for (int i = 0; i < 32; i++)
            {
                if (m.M41 == figur[itmp].figurm.M41 && m.M42 == figur[itmp].figurm.M42)
                    a = true;
            }
            return a;
        }
        void UpdateSkinMenu(GameTime deltaTime)
         {

             if (pressedMenubutton())
                 _state = GameState.MainMenu;
         }

        void UpdateWhiteWinner(GameTime deltaTime)
        {
            reset();
            whiteturn = true;
            if (pressedWinnerMenuebutton())
                _state = GameState.MainMenu;
            if (pressedWinnerResetbutton())
                _state = GameState.Gameplay;
        }

        void UpdateBlackWinner(GameTime deltaTime)
        {
            reset();
            if (pressedWinnerMenuebutton())
                _state = GameState.MainMenu;
            if (pressedWinnerResetbutton())
                _state = GameState.Gameplay;
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
        /*  Maus = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
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
            */
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



        protected override void Draw(GameTime gameTime) {
            graphics.GraphicsDevice.Clear(Color.White);
            

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
                case GameState.BlackWinner:
                    DrawBlackWinner(gameTime);
                    break;
                case GameState.WhiteWinner:
                    DrawWhiteWinner(gameTime);
                    break;
            }
            spriteBatch.Begin();
            spriteBatch.Draw(zeiger, position, origin: new Vector2(0, 0));
            spriteBatch.End();
        }

        void DrawMainMenu(GameTime deltaTime) { 
        
            spriteBatch.Begin();
            spriteBatch.Draw(startButton, destinationRectangle: new Rectangle(437, 50, 150, 100));
            spriteBatch.Draw(skinButton, destinationRectangle: new Rectangle(437, 200, 150, 100));
            spriteBatch.End();

        }

        void DrawGameplay(GameTime deltaTime)
        {
            graphics.GraphicsDevice.Clear(Color.DeepSkyBlue);
            DrawModel(brett, world, view, projection);
            for (int i = 0; i < 32; i++)
                figur[i].draw(view, projection);
            spriteBatch.Begin();
            spriteBatch.DrawString(text, Ausgabe, textposition, Color.Black);
            spriteBatch.End();

        }

        void DrawWhiteWinner(GameTime deltaTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(menueButton, destinationRectangle: new Rectangle(181, 380, 150, 100));
            spriteBatch.Draw(resetButton, destinationRectangle: new Rectangle(693, 380, 150, 100));
            spriteBatch.End();
        }

        void DrawBlackWinner(GameTime deltaTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(menueButton, destinationRectangle: new Rectangle(181, 380, 150, 100));
            spriteBatch.Draw(resetButton, destinationRectangle: new Rectangle(693, 380, 150, 100));
            spriteBatch.End();
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

        void reset()
        {
            figur[0].reset(0, 0, Figurentyp.Turm, true);
            figur[1].reset(1, 0, Figurentyp.Springer, true);
            figur[2].reset(2, 0, Figurentyp.Laeufer, true);
            figur[3].reset(3, 0, Figurentyp.King, true);
            figur[4].reset(4, 0, Figurentyp.Queen, true);
            figur[5].reset(5, 0, Figurentyp.Laeufer, true);
            figur[6].reset(6, 0, Figurentyp.Springer, true);
            figur[7].reset(7, 0, Figurentyp.Turm, true);
            for (int i = 0; i < 8; i++)
            {
                figur[i + 8].reset(i, 1, Figurentyp.Bauer, true);
                figur[i + 16].reset(i, 6, Figurentyp.Bauer, false);
            }
            figur[24].reset(0, 7, Figurentyp.Turm, false);
            figur[25].reset(1, 7, Figurentyp.Springer, false);
            figur[26].reset(2, 7, Figurentyp.Laeufer, false);
            figur[27].reset(3, 7, Figurentyp.King, false);
            figur[28].reset(4, 7, Figurentyp.Queen, false);
            figur[29].reset(5, 7, Figurentyp.Laeufer, false);
            figur[30].reset(6, 7, Figurentyp.Springer, false);
            figur[31].reset(7, 7, Figurentyp.Turm, false);
            for (int i = 8; i < 16; i++)
                figur[i].SetModel(bauermw);
            for (int i = 16; i < 24; i++)
                figur[i].SetModel(bauermb);
            figur[0].SetModel(turmmw);
            figur[1].SetModel(springermw);
            figur[2].SetModel(laufermw);
            figur[3].SetModel(queenmw);
            figur[4].SetModel(kingmw);
            figur[5].SetModel(laufermw);
            figur[6].SetModel(springermw);
            figur[7].SetModel(turmmw);
            figur[24].SetModel(turmmb);
            figur[25].SetModel(springermb);
            figur[26].SetModel(laufermb);
            figur[27].SetModel(queenmb);
            figur[28].SetModel(kingmb);
            figur[29].SetModel(laufermb);
            figur[30].SetModel(springermb);
            figur[31].SetModel(turmmb);
        }

        void DrawSkinMenu(GameTime deltaTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(menueButton, destinationRectangle: new Rectangle(437, 50, 150, 100));
            spriteBatch.End();
        }

        public Vector3 GetVector()
        {
            int schritt = 4;// Breite der Feldér
            int x=400;
            int y=216;
            int zeile = 0;
            int spalte = 0;
            while (position.X > x)
            {
                spalte++;
                x += (schritt * 7);
            }
            while (position.Y > y)
            {
                zeile++;
                y += (schritt * 9);
            }
            return new Vector3(-((spalte-4)*schritt-2), -((zeile - 4) * schritt-2),0);
        }

        public int wer(Vector2 pos)
        {
            for (int i = 0; i < 32; i++)
            {
                if (figur[i].position == pos && figur[i].alive)
                {
                    return i;
                }
            }
            return -1;
        }

        //gibt position des Feldes zurück im Bezug auf Schachbrett, erhält id der worldm
        public Vector2 feld(int ri)
        {
            return new Vector2(ri % 8, ri / 8);
        }
        

        public class Schachfigur
        {
            public Vector2 position;
            private Vector3 destiny;
            private bool moving;
            Model m;
            public bool alive;
            public Matrix figurm;
            private Figurentyp f;
            private Vector3 movposition;
            public bool iswhite;
            Schachfigur opfer = null;
            Matrix TMPf;
            Matrix TMPm;
            float a=0;
            float b=0;

            public Schachfigur(int x,int y,Figurentyp typ,bool white)
            {
                position = new Vector2(x, y);
                alive = true;
                f = typ;
                figurm= Matrix.CreateScale(-0.12f) * Matrix.CreateTranslation(new Vector3(raster(new Vector2(x, y)), 0));
                iswhite = white;
                if (iswhite)
                {
                    figurm= Matrix.CreateRotationZ((float)Math.PI) * figurm;
                }
            }

            public Matrix getPosition()
            {
                return figurm;
            }

            public void SetModel(Model model)
            {
                m = model;
            }

            public Model GetModel()
            {
                return m;
            }

            public bool isEnemy(bool other)
            {
                return iswhite != other;
            }

            public bool possiblemove(Vector2 test, Game1 g)
            {
                if (test.X < 0 || test.X > 7 || test.Y < 0 || test.Y > 7)
                    return false;
                int fi = g.wer(test);
                switch (f)
                {
                    case Figurentyp.Bauer:
                        if (iswhite)
                        {
                            if (test.Y - position.Y == 1 && position.X == test.X && fi < 0)
                                return true;
                            else
                            {
                                if (position.Y == 1 && test.Y - position.Y == 2 && position.X==test.X && fi < 0)
                                    if(g.wer(new Vector2(test.X,2))<0)
                                    return true;
                            {
                                    if (test.Y - position.Y == 1 && Math.Abs(test.X - position.X) == 1 && fi>=0 && g.figur[fi].isEnemy(iswhite))
                                        return true;
                                }
                            }
                            return false;
                        }
                        else
                        {
                            if (test.Y - position.Y == -1 && position.X == test.X && fi < 0)
                                return true;
                            else
                            {
                                if (position.Y == 6 && test.Y - position.Y == -2 && position.X == test.X && fi < 0)
                                    if (g.wer(new Vector2(test.X, 5)) < 0)
                                        return true;
                                
                                {
                                    if (test.Y - position.Y == -1 && Math.Abs(test.X - position.X) == 1 && fi >= 0 && g.figur[fi].isEnemy(iswhite))
                                        return true;
                                }
                            }
                            return false;
                        }
                    case Figurentyp.Turm:
                        if (((position.X == test.X && position.Y != test.Y) || (position.X != test.X && position.Y == test.Y)) && fi<0)
                        {
                            if (position.X == test.X)
                            {
                                for(float i=Math.Min(position.Y,test.Y)+1;i<Math.Max(position.Y, test.Y); i++)
                                {
                                    if (g.wer(new Vector2(position.X, i)) >= 0)
                                        return false;
                                }
                                return true;
                            }
                            else
                            {
                                for (float i = Math.Min(position.X, test.X)+1; i < Math.Max(position.X, test.X); i++)
                                {
                                    if (g.wer(new Vector2(i, test.Y)) >= 0)
                                        return false;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            if (((position.X == test.X && position.Y != test.Y) || (position.X != test.X && position.Y == test.Y)) && fi >= 0)
                            {
                                if (g.figur[fi].isEnemy(iswhite))
                                {
                                    if (position.X == test.X)
                                    {
                                        for (float i = Math.Min(position.Y, test.Y)+1; i < Math.Max(position.Y, test.Y); i++)
                                        {
                                            if (g.wer(new Vector2(position.X, i)) >= 0)
                                                return false;
                                        }
                                        return true;
                                    }
                                    else
                                    {
                                        for (float i = Math.Min(position.X, test.X)+1; i < Math.Max(position.X, test.X); i++)
                                        {
                                            if (g.wer(new Vector2(i, test.Y)) >= 0)
                                                return false;
                                        }
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                    case Figurentyp.Laeufer:
                        if (Math.Abs(position.X - test.X) == Math.Abs(position.Y - test.Y))
                        {
                            Vector2 p = new Vector2(position.X,position.Y);
                            while (!p.Equals(test))
                            {
                                if (position.X < test.X)
                                    p.X += 1;
                                else
                                    p.X -= 1;
                                if (position.Y < test.Y)
                                    p.Y += 1;
                                else
                                    p.Y -= 1;
                                if (g.wer(p) >= 0)
                                {
                                    if (p.Equals(test))
                                    {
                                        if (fi >= 0 && g.figur[fi].isEnemy(iswhite))
                                            return true;
                                    }
                                    return false;
                                }
                            }return true;
                        }
                        return false;
                    case Figurentyp.Springer:
                        if(Math.Abs(Math.Abs(position.X-test.X)-Math.Abs(position.Y-test.Y))==1 && Math.Abs(position.X-test.X)<3 && Math.Abs(position.Y - test.Y) < 3)
                        {
                            if (fi < 0)
                                return true;
                            else
                            {
                                if (g.figur[fi].isEnemy(iswhite))
                                    return true;
                            }
                        }
                        return false;
                    case Figurentyp.King:
                        if(Math.Abs(position.X-test.X)<2 && Math.Abs(position.Y - test.Y) < 2)
                        {
                            if (fi < 0)
                                return true;
                            else
                            {
                                if (g.figur[fi].isEnemy(iswhite))
                                    return true;
                            }
                        }
                        return false;
                    case Figurentyp.Queen:
                        if (((position.X == test.X && position.Y != test.Y) || (position.X != test.X && position.Y == test.Y)) && fi < 0)
                        {
                            if (position.X == test.X)
                            {
                                for (float i = Math.Min(position.Y, test.Y)+1; i < Math.Max(position.Y, test.Y); i++)
                                {
                                    if (g.wer(new Vector2(position.X, i)) >= 0)
                                        return false;
                                }
                                return true;
                            }
                            else
                            {
                                for (float i = Math.Min(position.X, test.X)+1; i < Math.Max(position.X, test.X); i++)
                                {
                                    if (g.wer(new Vector2(i, test.Y)) >= 0)
                                        return false;
                                }
                                return true;
                            }
                        }
                        else
                        {
                            if (((position.X == test.X && position.Y != test.Y) || (position.X != test.X && position.Y == test.Y)) && fi >= 0)
                            {
                                if (g.figur[fi].isEnemy(iswhite))
                                {
                                    if (position.X == test.X)
                                    {
                                        for (float i = Math.Min(position.Y, test.Y)+1; i < Math.Max(position.Y, test.Y); i++)
                                        {
                                            if (g.wer(new Vector2(position.X, i)) >= 0)
                                                return false;
                                        }
                                        return true;
                                    }
                                    else
                                    {
                                        for (float i = Math.Min(position.X, test.X)+1; i < Math.Max(position.X, test.X); i++)
                                        {
                                            if (g.wer(new Vector2(i, test.Y)) >= 0)
                                                return false;
                                        }
                                        return true;
                                    }
                                }
                            }
                        }
                        if (Math.Abs(position.X - test.X) == Math.Abs(position.Y - test.Y))
                        {
                            Vector2 p = new Vector2(position.X, position.Y);
                            while (!p.Equals(test))
                            {
                                if (position.X < test.X)
                                    p.X += 1;
                                else
                                    p.X -= 1;
                                if (position.Y < test.Y)
                                    p.Y += 1;
                                else
                                    p.Y -= 1;
                                if (g.wer(p) >= 0)
                                {
                                    if (p.Equals(test))
                                    {
                                        if (fi >= 0 && g.figur[fi].isEnemy(iswhite))
                                            return true;
                                    }
                                    return false;
                                }
                            }return true;
                        }
                        return false;
                }
                return false;
            }

            public void move(Vector2 ziel, Game1 g,int i)
            {
                   movposition = new Vector3(raster(position), 0);
                    destiny.X = raster(ziel).X;
                    destiny.Y = raster(ziel).Y;
                    destiny.Z = 0;
                    moving = true;
                
                    if (g.wer(ziel) >= 0) {
                    if (iswhite)
                    {
                        if (g.wer(ziel) == 28)
                            g._state = GameState.WhiteWinner;
                    }
                    else
                    {
                        if (g.wer(ziel) == 4)
                            g._state = GameState.BlackWinner;
                    }
                        opfer=g.figur[g.wer(ziel)];
                    }
                    position = ziel;
                //figurm = Matrix.CreateScale(figurm.M33)* Matrix.CreateTranslation(new Vector3(g.worldm[i].M41, g.worldm[i].M42, 0));
                //figurm = Matrix.CreateRotationZ((float)Math.PI) *figurm;
                TMPf = figurm;
                TMPm = g.worldm[i];
            }

            public void reset(int x, int y, Figurentyp typ, bool white)
            {
                position = new Vector2(x, y);
                alive = true;
                f = typ;
                figurm = Matrix.CreateScale(-0.12f) * Matrix.CreateTranslation(new Vector3(raster(new Vector2(x, y)), 0));
                iswhite = white;
                if (iswhite)
                {
                    figurm = Matrix.CreateRotationZ((float)Math.PI) * figurm;
                }
            }

            private void schlagen()
            {
                opfer.alive = false;
                opfer.position=new Vector2(-1,-1);
                opfer.figurm= Matrix.CreateWorld(new Vector3(100,100,100), Vector3.Forward, Vector3.Up);
                opfer = null;
            }

            public void bauerntausch(Game1 g)
            {
                g.Ausgabe += "Ein Bauer hat die hintere Linie erreicht,\nwelche Form soll er annehmen?\nTurm(1),Springer(2),Lauufer(3),Dame(4)\n";
                if (Keyboard.GetState().IsKeyDown(Keys.D1))
                {
                    f = Figurentyp.Turm;
                    if (iswhite)
                        SetModel(g.turmmw);
                    else
                        SetModel(g.turmmb);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D2))
                {
                    f = Figurentyp.Springer;
                    if (iswhite)
                        SetModel(g.springermw);
                    else
                        SetModel(g.springermb);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D3))
                {
                    f = Figurentyp.Laeufer;
                    if (iswhite)
                        SetModel(g.laufermw);
                    else
                        SetModel(g.laufermb);
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D4))
                {
                    f = Figurentyp.Queen;
                    if (iswhite)
                        SetModel(g.queenmw);
                    else
                        SetModel(g.queenmb);
                }
            }


            public void update(GameTime deltatime)
            {
                if (moving)
                {
                    float abstandx = TMPm.M41 - TMPf.M41;
                    float abstandy = TMPm.M42 - TMPf.M42;
                    // if (abstandx > 0)


                    if (abstandx != 0 && Math.Abs(a) < Math.Abs(abstandx))
                        a = a + 0.001f * Math.Sign(abstandx);
                    if (abstandy != 0 && Math.Abs(b) < Math.Abs(abstandy))
                        b = b + 0.001f * Math.Sign(abstandy);
                    if (Math.Abs(a) < Math.Abs(abstandx) || Math.Abs(b) < Math.Abs(abstandy))
                    {
                        figurm = Matrix.CreateScale(TMPf.M33)
                            * Matrix.CreateTranslation(new Vector3(TMPf.M41 + a, TMPf.M42 + b, 0));
                        if (abstandy > 0 && abstandx == 0)
                            figurm = Matrix.CreateRotationZ((float)Math.PI) * figurm;
                        if (abstandy > 0 && abstandx < 0)
                            figurm = Matrix.CreateRotationZ((float)Math.PI * (-0.75f)) * figurm;
                        if (abstandy > 0 && abstandx > 0)
                            figurm = Matrix.CreateRotationZ((float)Math.PI * 0.75f) * figurm;
                        if (abstandy < 0 && abstandx < 0)
                            figurm = Matrix.CreateRotationZ((float)Math.PI * (-0.25f)) * figurm;
                        if (abstandy < 0 && abstandx > 0)
                            figurm = Matrix.CreateRotationZ((float)Math.PI * 0.25f) * figurm;
                        if (abstandy == 0 && abstandx < 0)
                            figurm = Matrix.CreateRotationZ((float)Math.PI * (-0.5f)) * figurm;
                        if (abstandy == 0 && abstandx > 0)
                            figurm = Matrix.CreateRotationZ((float)Math.PI * 0.5f) * figurm;
                        if (abstandy < 0 && abstandx == 0) { }
                    }
                    else
                    {
                        if (opfer != null)
                            schlagen();
                        figurm = Matrix.CreateScale(figurm.M33)
                                  * Matrix.CreateTranslation(new Vector3(TMPm.M41, TMPm.M42, 0));
                        figurm = Matrix.CreateRotationZ((float)Math.PI) * figurm;
                        //nicht_gedreht = true;
                        moving = false;
                        //BLOCKIERT = false;
                        a = 0;
                        b = 0;
                    }
                }
            }

            public void draw(Matrix viewMatrix, Matrix projectionMatrix)
            {
                if (alive)
                {
                    if (moving)
                    {
                        foreach (ModelMesh mesh in this.m.Meshes)

                        {

                            foreach (BasicEffect effect in mesh.Effects)

                            {

                                effect.View = viewMatrix;

                                effect.World = Matrix.CreateWorld(movposition, Vector3.Forward, Vector3.Up);

                                effect.Projection = projectionMatrix;

                            }

                            mesh.Draw();
                        }
                    }
                    else
                    {
                        foreach (ModelMesh mesh in m.Meshes)
                        {
                            foreach (BasicEffect effect in mesh.Effects)
                            {
                                effect.World = figurm;
                                effect.View = viewMatrix;
                                effect.Projection = projectionMatrix;
                            }
                            mesh.Draw();
                        }
                    }
                }
            }            
        }

    }
}