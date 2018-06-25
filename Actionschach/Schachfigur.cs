using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Actionschach;

namespace Schachfigur
{
    public class Schachfigur
    {
        public Schachfigur(Vector3 pos)
        {
            position = pos;
            moving = false;
            destiny = pos;
        }

        public Matrix getPosition()
        {
            return Matrix.CreateWorld(this.position, Vector3.Forward, Vector3.Up);
        }

        public move(Vector3 ziel)
        {
            destiny = ziel;
            moving = true;
        }

        public update(Gametime deltatime)
        {
            if (moving)
            {
                if (position == destiny)
                {
                    moving = false;
                }
                else
                {
                    position =destiny.
                }
            }
        }


        Vector3 position;
        private Vector3 destiny;
        bool moving;



    }
}
