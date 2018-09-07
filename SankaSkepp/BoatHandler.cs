using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SankaSkepp
{
    class BoatHandler
    {
        Texture2D cap, midsection, missTexture, hitTexture, nil;
        List<Boat> boats;
        Rectangle cell;
        List<Vector2> misses, hits;
        bool limitOn, hasFired;
        Boat tempPlace;

        public BoatHandler()
        {
            boats = new List<Boat>();
            misses = new List<Vector2>();
            hits = new List<Vector2>();
            limitOn = true;
            hasFired = false;
        }

        public BoatHandler(Texture2D cap, Texture2D midsection, Texture2D nil, Texture2D missTexture, Texture2D hitTexture, Rectangle cell) : this ()
        {
            this.cap = cap;
            this.midsection = midsection;
            this.nil = nil;
            this.missTexture = missTexture;
            this.hitTexture = hitTexture;
            this.cell = cell;
        }

        public int Len
        {
            get { return boats.Count; }
        }

        public List<Vector2> Misses
        {
            get { return misses; }
        }

        public List<Vector2> Hits
        {
            get { return hits; }
        }

        public bool HasFired
        {
            get { return hasFired; }
            set { hasFired = value; }
        }

        public bool LimitOn
        {
            get { return limitOn; }
            set { limitOn = value; }
        }

        public bool Alive
        {
            get
            {
                return !(hits.Count == 16);
            }
        }

        public int Sum
        {
            get
            {
                int sum = 0;
                foreach (Boat boat in boats)
                {
                    sum += boat.Length;
                }
                return sum;
            }
        }

        public void Add(Boat boat)
        {
            if (!DoesOverlap(boat) && !ExceedsLimit(boat.Length))
                boats.Add(boat);
        }

        public string AddS(Boat boat) // Debug Use
        {
            if (!DoesOverlap(boat) && !ExceedsLimit(boat.Length))
                boats.Add(boat);
            return "Overlap: " + DoesOverlap(boat) + ", Exceeds Limit: " + ExceedsLimit(boat.Length);
        }

        public void Place(List<Vector2> loc)
        {
            int len;
            float pos;
            if (loc[0].X == loc[1].X)
            {
                len = (int)Math.Abs((float)(loc[0].Y - loc[1].Y)) + 1;
                pos = loc[0].Y < loc[1].Y ? loc[0].Y : loc[1].Y;
                this.Add(new Boat(new Vector2(loc[0].X, pos), len, (float)Math.PI / 2));
            }
            else if (loc[0].Y == loc[1].Y)
            {
                len = (int)Math.Abs((float)(loc[0].X - loc[1].X)) + 1;
                pos = loc[0].X < loc[1].X ? loc[0].X : loc[1].X;
                this.Add(new Boat(new Vector2(pos, loc[0].Y), len, 0f));
            }
        }

        private bool DoesOverlap(Boat boat)
        {
            List<Vector2> otherBoat, thisBoat = boat.Occupies();
            foreach(Boat b in boats)
            {
                otherBoat = b.Occupies();
                foreach(Vector2 other in otherBoat)
                {
                    foreach(Vector2 thiss in thisBoat)
                    {
                        if (other == thiss)
                            return true;
                    }
                }
            }
            return false;
        }

        public bool FireAt(Vector2 loc)
        {
            if (!misses.Contains(loc) && !hits.Contains(loc) && loc.X != -1 && loc.Y != -1)
            {
                if (DoesHit(loc))
                    hits.Add(loc);
                else
                    misses.Add(loc);
                return true;
            }
            return false;
        }

        private bool DoesHit(Vector2 loc)
        {
            List<Vector2> boat;
            foreach (Boat b in boats)
            {
                boat = b.Occupies();
                foreach (Vector2 space in boat)
                {
                    if (loc == space)
                    {
                        b.Life--;
                        return true;
                    }  
                }
            }
            return false;
        }

        private bool ExceedsLimit(int length)
        {
            if (limitOn)
            {
                if (length > 5 || length <= 1)
                    return true;

                int size2 = 0;
                foreach (Boat boat in boats)
                {
                    if (length == 2)
                    {
                        if (boat.Length == 2)
                            size2++;
                        if (size2 == 2)
                            return true;
                    }
                    else
                    {
                        if (boat.Length == length)
                            return true;
                    }
                }
                return false;
            }
            else
            {
                if (length > 1 && length <= 16 - Sum && (16 - (Sum + length)) != 1)
                    return false;

                return true;
            }
        }

        public void Reset()
        {
            boats.Clear();
            misses.Clear();
            hits.Clear();
            hasFired = false;
        }

        public void UpdateCell(Rectangle cell)
        {
            this.cell = cell;
        }

        public void UpdateTemp(Vector2 loc, Vector2 mouse)
        {
            int len;
            float pos;
            if (loc.X != mouse.X || loc.Y != mouse.Y)
            {
                if (loc.X == mouse.X)
                {
                    len = (int)Math.Abs((float)(loc.Y - mouse.Y)) + 1;
                    pos = loc.Y < mouse.Y ? loc.Y : mouse.Y;
                    tempPlace = new Boat(new Vector2(loc.X, pos), len, (float)Math.PI / 2);
                }
                else if (loc.Y == mouse.Y)
                {
                    len = (int)Math.Abs((float)(loc.X - mouse.X)) + 1;
                    pos = loc.X < mouse.X ? loc.X : mouse.X;
                    tempPlace = new Boat(new Vector2(pos, loc.Y), len, 0f);
                }
            }
            else
            {
                tempPlace = new Boat(new Vector2(loc.X, loc.Y), 0, 0f);
            }
        }

        private void DrawBoat(SpriteBatch spriteBatch, Boat boat, Color color)
        {
            if (boat.Length <= 1)
            {
                spriteBatch.Draw(
                    nil,
                    new Vector2(boat.Position.X * cell.Width + (cell.Width / 2), boat.Position.Y * cell.Height + (cell.Height / 2)),
                    null,
                    color,
                    boat.Direction,
                    new Vector2(nil.Width / 2, nil.Height / 2),
                    new Vector2((float)cell.Height / (float)nil.Height, (float)cell.Width / (float)nil.Width),
                    SpriteEffects.None,
                    0f
                );
                return;
            }

            spriteBatch.Draw(
                cap,
                new Vector2(boat.Position.X * cell.Width + (cell.Width / 2), boat.Position.Y * cell.Height + (cell.Height / 2)),
                null,
                color,
                boat.Direction,
                new Vector2(cap.Width / 2, cap.Height / 2),
                new Vector2((float)cell.Height / (float)cap.Height, (float)cell.Width / (float)cap.Width),
                SpriteEffects.None,
                0f
            );

            if ((int)boat.Direction == 0) // float comps ?????!!
            {
                spriteBatch.Draw(
                    cap,
                    new Vector2((boat.Position.X + boat.Length) * cell.Width - (cell.Width / 2), boat.Position.Y * cell.Height + (cell.Height / 2)),
                    null,
                    color,
                    boat.Direction + (float)Math.PI,
                    new Vector2(cap.Width / 2, cap.Height / 2),
                    new Vector2((float)cell.Height / (float)cap.Height, (float)cell.Width / (float)cap.Width),
                    SpriteEffects.None,
                    0f
                );

                for (int i = 1; i <= boat.Length - 2; i++)
                {
                    spriteBatch.Draw(
                        midsection,
                        new Vector2((boat.Position.X + i) * cell.Width + (cell.Width / 2), boat.Position.Y * cell.Height + (cell.Height / 2)),
                        null,
                        color,
                        boat.Direction,
                        new Vector2(midsection.Width / 2, midsection.Height / 2),
                        new Vector2((float)cell.Height / (float)midsection.Height, (float)cell.Width / (float)midsection.Width),
                        SpriteEffects.None,
                        0f
                    );
                }
            }
            else
            {
                spriteBatch.Draw(
                    cap,
                    new Vector2(boat.Position.X * cell.Width + (cell.Width / 2), (boat.Position.Y + boat.Length) * cell.Height - (cell.Height / 2)),
                    null,
                    color,
                    boat.Direction + (float)Math.PI,
                    new Vector2(cap.Width / 2, cap.Height / 2),
                    new Vector2((float)cell.Height / (float)cap.Height, (float)cell.Width / (float)cap.Width),
                    SpriteEffects.None,
                    0f
                );

                for (int i = 1; i <= boat.Length - 2; i++)
                {
                    spriteBatch.Draw(
                        midsection,
                        new Vector2(boat.Position.X * cell.Width + (cell.Width / 2), (boat.Position.Y + i) * cell.Height + (cell.Height / 2)),
                        null,
                        color,
                        boat.Direction,
                        new Vector2(midsection.Width / 2, midsection.Height / 2),
                        new Vector2((float)cell.Height / (float)midsection.Height, (float)cell.Width / (float)midsection.Width),
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }

        public void DrawShips(SpriteBatch spriteBatch)
        {
            foreach (Boat boat in boats)
            {
                DrawBoat(spriteBatch, boat, Color.White);
            }
        }
        
        public void DrawHits(SpriteBatch spriteBatch)
        { 
            foreach (Vector2 loc in misses)
            {
                spriteBatch.Draw(
                        missTexture,
                        new Vector2(cell.Width * loc.X, cell.Height * loc.Y),
                        null,
                        Color.White,
                        0f,
                        new Vector2(0, 0),
                        new Vector2((float)cell.Width / (float)missTexture.Width, (float)cell.Height / (float)missTexture.Height),
                        SpriteEffects.None,
                        0f
                    );
            }

            foreach (Boat boat in boats)
            {
                if (boat.Life == 0)
                    DrawBoat(spriteBatch, boat, Color.White);
            }

            foreach (Vector2 loc in hits)
            {
                spriteBatch.Draw(
                    hitTexture,
                    new Vector2(cell.Width * loc.X, cell.Height * loc.Y),
                    null,
                    Color.White,
                    0f,
                    new Vector2(0, 0),
                    new Vector2((float)cell.Width / (float)hitTexture.Width, (float)cell.Height / (float)hitTexture.Height),
                    SpriteEffects.None,
                    0f
                );
            }
        }

        public void DrawTemp(SpriteBatch spriteBatch)
        {
            if (tempPlace != null)
            {
                Color draw = Color.White;
                if (ExceedsLimit(tempPlace.Length))
                    draw = Color.Red;

                DrawBoat(spriteBatch, tempPlace, draw);
            }
        }

    }
}
