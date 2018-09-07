using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SankaSkepp
{
    class Boat
    {
        int length, life;
        float direction;
        Vector2 position;


        public Boat(Vector2 position, int length, float direction)
        {
            this.position = position;
            this.length = length;
            life = length;
            this.direction = direction;
        }

        public int Length
        {
            get { return length; }
        }

        public int Life
        {
            get { return life; }
            set { life = value; }
        }

        public float Direction
        {
            get { return direction; }
        }

        public Vector2 Position
        {
            get { return position; }
        }

        public List<Vector2> Occupies()
        {
            List<Vector2> occ = new List<Vector2>();
            for (int i = 0; i < length; i++)
            {
                if ((int)direction == 0)
                    occ.Add(new Vector2(position.X + i, position.Y));
                else
                    occ.Add(new Vector2(position.X, position.Y + i));
            }
            return occ;
        }
    }
}
