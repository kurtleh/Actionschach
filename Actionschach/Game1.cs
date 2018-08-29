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
        GameState _state;
        Vector2 position;
        bool isselected;
        Schachfigur select;
        Texture2D startButton;
        Texture2D skinButton;
        int d, e;
       static float zoom=45;
        static float zeit = 0;
        static float zeit2 = 0;
        ButtonState lastmousestate;
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
        public Model bauerm;
        public Model turmm;
        public Model springerm;
        public Model lauferm;
        public Model kingm;
        public Model queenm;
       
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
                if (m.Equals(figur[i].figurm)) { 
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
           _state = GameState.MainMenu;
            view = Matrix.CreateLookAt(camPosition, new Vector3(0, -0.28f, 0), Vector3.UnitY);
            int k = 0; 
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    worldm[k] = Matrix.CreateTranslation(raster(new Vector2(j,i)).X,raster(new Vector2(j,i)).Y, 0);
                    k++;
                }
            }
            //Startaufstellung
            for (int i = 0; i < 8; i++)
            {
                figur[i]=new Schachfigur(i,0,Figurentyp.Turm,true);
            }
            for (int i = 8; i < 16; i++)
            {
                figur[i] =new Schachfigur(i-8,1, Figurentyp.Bauer,true);
            }
            for (int i = 16; i < 24; i++)
            {
                figur[i] =new Schachfigur(i-16,6, Figurentyp.Bauer,false);
            }
              for (int i = 24; i < 32; i++)
            {
                figur[i] =new Schachfigur(i-24,7, Figurentyp.Turm,false);
            }


         base.Initialize();
        }
        public bool pressedSkinbutton()
        {
            MouseState state = Mouse.GetState();
            position.X = state.X;
            position.Y = state.Y;
            if (position.X < 450 &&
        position.X > 300 &&
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
            if (position.X < 450  &&
        position.X > 300 &&
        position.Y < 150 &&
        position.Y > 50 && click())
            {
                return true;
            }
            return false;
        }

        protected override void LoadContent() {
            Content = new ContentManager(this.Services, "Content");

             startButton = this.Content.Load<Texture2D>("StartButton");
             skinButton = this.Content.Load<Texture2D>("SkinButton");

            spriteBatch = new SpriteBatch(GraphicsDevice);


            brett = Content.Load<Model>("chessboard");
            turmm = Content.Load<Model>("turmneu");

            for (int i = 0; i < 32; i++)
                figur[i].SetModel(turmm);

            maus = Content.Load<Model>("kugel");
            zeiger = Content.Load<Texture2D>("Cursor"); }

        public bool pressedMenubutton()
        {
            MouseState state = Mouse.GetState();
            position.X = state.X;
            position.Y = state.Y;
            if (position.X < 450 &&
        position.X > 300 &&
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
            if (!klick)
            {
                for (int i = 0; i < 64; i++)
                {
                    if (Intersects(Maus, maus, worldm[i], view, projection, viewport) && belegt(worldm[i]))
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
            else
            {
                figur[itmp].figurm.M43 = 0.2f;
                figur[itmp].figurm = Matrix.CreateRotationZ((float)deltaTime.ElapsedGameTime.TotalSeconds) * figur[itmp].figurm;
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
                    figur[itmp].figurm = Matrix.CreateScale(figur[itmp].figurm.M33) * Matrix.CreateTranslation(new Vector3(worldm[i].M41, worldm[i].M42, 0));
                    klick = false;
                    itmp = -1;
                    zugErfolgt = true;
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
              for (int i=0;i<32; i++)
            {
                figur[i].update(deltaTime);
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
            graphics.GraphicsDevice.Clear(Color.DeepSkyBlue);
            

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
            }             spriteBatch.Begin();
            spriteBatch.Draw(zeiger, position, origin: new Vector2(0, 0));
            spriteBatch.End();
        }

        void DrawMainMenu(GameTime deltaTime) { 
        
            spriteBatch.Begin();
            spriteBatch.Draw(startButton, destinationRectangle: new Rectangle(300, 50, 150, 100));
            spriteBatch.Draw(skinButton, destinationRectangle: new Rectangle(300, 200, 150, 100));
            spriteBatch.End();

        }

        void DrawGameplay(GameTime deltaTime)
        {
            graphics.GraphicsDevice.Clear(Color.DeepSkyBlue);
            DrawModel(brett, world, view, projection);
            for (int i = 0; i < 32; i++)
                figur[i].draw(view, projection);
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



        void DrawSkinMenu(GameTime deltaTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(skinButton, destinationRectangle: new Rectangle(300, 50, 150, 100));
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
            bool iswhite;

            public Schachfigur(int x,int y,Figurentyp typ,bool white)
            {
                position = new Vector2(x, y);
                alive = true;
                f = typ;
                figurm=Matrix.CreateTranslation(new Vector3(raster(new Vector2(x, y)), 0));
                iswhite = white;
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
                int fi = g.wer(test);
                switch (f)
                {
                    case Figurentyp.Bauer:
                        if (iswhite)
                        {
                            if (test.Y - position.Y == 1 && fi < 0)
                                return true;
                            else
                            {
                                if (position.Y == 1 && test.Y - position.Y == 2 && fi < 0)
                                    return true;
                                else
                                {
                                    if (test.Y - position.Y == 1 && Math.Abs(test.X - position.X) == 1 && fi>=0 && g.figur[fi].isEnemy(iswhite))
                                        return true;
                                }
                            }
                            return false;
                        }
                        break;
                    case Figurentyp.Turm:
                        if (position.X == test.X && position.Y != test.Y || position.X != test.X && position.Y == test.Y)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case Figurentyp.Springer:
                        return true;
                    case Figurentyp.Laeufer:
                        return true;
                    case Figurentyp.King:
                        return true;
                    case Figurentyp.Queen:
                        return true;
                }
                return false;
            }

            public void move(Vector2 ziel)
            {
                movposition = new Vector3(raster(position), 0);
                destiny.X = raster(ziel).X;
                destiny.Y = raster(ziel).Y;
                destiny.Z = 0;
                moving = true;
                position = ziel;
            }

            public void update(GameTime deltatime)
            {
                
                  if (moving)
                  {
                      if (movposition == destiny)
                      {
                          moving = false;
                      }
                      else
                      {
                          Vector3 direction = Vector3.Normalize(destiny - movposition);
                          if ((destiny - movposition).Length() > destiny.Length())
                          {
                              movposition = movposition + direction;
                          }
                          else
                          {
                              movposition = destiny;
                          }
                      }
                }
                else
                {
                    figurm= Matrix.CreateTranslation(new Vector3(raster(position), 0));
                }
            }

            public void draw(Matrix viewMatrix,Matrix projectionMatrix)
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