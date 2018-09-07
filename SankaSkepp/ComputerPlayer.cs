using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SankaSkepp
{
    class ComputerPlayer : BoatHandler
    {
        Random rnd;

        public ComputerPlayer(Texture2D cap, Texture2D midsection, Texture2D nil, Texture2D missTexture, Texture2D hitTexture, Rectangle cell) 
            : base(cap, midsection, nil, missTexture, hitTexture, cell)
        {
            rnd = new Random();
        }

        public void RandomLayout(int x, int y)
        {
            float angle;
            Vector2 loc;

            if (LimitOn)
            {
                angle = rnd.Next(100) < 50 ? 0f : (float)Math.PI / 2f;
                if ((int)angle == 0)
                    loc = new Vector2(rnd.Next(x - 2), rnd.Next(y));
                else
                    loc = new Vector2(rnd.Next(x), rnd.Next(y - 2));
                Add(new Boat(new Vector2(loc.X, loc.Y), 2, angle));

                int i = 2;
                while (i > Len && i < 6)
                {
                    angle = rnd.Next(100) < 50 ? 0f : (float)Math.PI / 2f;
                    if ((int)angle == 0)
                        loc = new Vector2(rnd.Next(x - i), rnd.Next(y));
                    else
                        loc = new Vector2(rnd.Next(x), rnd.Next(y - i));
                    Add(new Boat(new Vector2(loc.X, loc.Y), i, angle));
                    if (i == Len)
                        i++;
                }
            }
            else
            {
                int len;
                while(Sum < 16)
                {
                    len = rnd.Next(x);
                    angle = rnd.Next(100) < 50 ? 0f : (float)Math.PI / 2f;
                    if ((int)angle == 0)
                        loc = new Vector2(rnd.Next(x - len), rnd.Next(y));
                    else
                        loc = new Vector2(rnd.Next(x), rnd.Next(y - len));
                    Add(new Boat(new Vector2(loc.X, loc.Y), len, angle));
                }
            }
        }

        public bool Fire(BoatHandler boats, int x, int y)
        {
            /* kollar var träff, finns det en bredvid så flytta höger tills det finns en tom ruta (skjut då och bryt)
             * eller brädet är slut (gör sammma åt vänster då) finns det ingen så kolla om det finns en över eller under,
             * gör det det gör samma sak som tidigare, först ned sedan upp.
             * 
             * finns det ingen kandidat skjut tom ruta oven/höger/under/vänster om träff
             * 
             * om ovanstående inte resulterar till ett skott, innebär det att skeppet eller skeppen ringats in. 
             * skjut då en slumpad position som inte redan prövats. gjör även detta om det inte finns någon träff alls
             */
            Vector2 loc;

            if (boats.Hits.Count > 0)
            {
                foreach (Vector2 hit in boats.Hits)
                {
                    int step;
                    bool direction;
                    if (boats.Hits.Contains(new Vector2(hit.X, hit.Y - 1)) || boats.Hits.Contains(new Vector2(hit.X, hit.Y + 1))) //known vertical
                    {
                        step = 1;
                        direction = true;
                        while (direction)
                        {
                            if (boats.Misses.Contains(new Vector2(hit.X, hit.Y + step)) || hit.Y + step < y)
                                direction = !direction;
                            if (boats.FireAt(new Vector2(hit.X, hit.Y + step)))
                            {
                                System.Diagnostics.Debug.WriteLine("Fired At" + new Vector2(hit.X, hit.Y + step));
                                return true;
                            }
                            step++;
                        }
                        step = 1;
                        while (!direction)
                        {
                            if (boats.Misses.Contains(new Vector2(hit.X, hit.Y - step)) || hit.Y - step >= 0)
                                break;
                            if (boats.FireAt(new Vector2(hit.X, hit.Y - step)))
                            {
                                System.Diagnostics.Debug.WriteLine("Fired At" + new Vector2(hit.X, hit.Y - step) + " hit.Y - step >= 0 ->" + (hit.Y - step >= 0));
                                return true;
                            }

                            step++;
                        }
                    }

                    if (boats.Hits.Contains(new Vector2(hit.X + 1, hit.Y)) || boats.Hits.Contains(new Vector2(hit.X - 1, hit.Y))) //known horizontal
                    {
                        step = 1;
                        direction = true;
                        while (direction)
                        {
                            if (boats.Misses.Contains(new Vector2(hit.X + step, hit.Y)) || hit.X + step < x)
                                direction = !direction;
                            if (boats.FireAt(new Vector2(hit.X + step, hit.Y)))
                            {
                                System.Diagnostics.Debug.WriteLine("Fired At" + new Vector2(hit.X + step, hit.Y));
                                return true;
                            }
                            step++;
                        }
                        step = 1;
                        while (!direction)
                        {
                            if (boats.Misses.Contains(new Vector2(hit.X - step, hit.Y)) || hit.X - step >= 0)
                                break;
                            if (boats.FireAt(new Vector2(hit.X - step, hit.Y)))
                            {
                                System.Diagnostics.Debug.WriteLine("Fired At" + new Vector2(hit.X - step, hit.Y));
                                return true;
                            }
                            step++;
                        }
                    }

                    // try around known hit
                    if (boats.FireAt(new Vector2(hit.X + 1, hit.Y)))
                    {
                        System.Diagnostics.Debug.WriteLine("Fired At" + new Vector2(hit.X + 1, hit.Y));
                        return true;
                    }
                    else if (boats.FireAt(new Vector2(hit.X, hit.Y + 1)))
                    {
                        System.Diagnostics.Debug.WriteLine("Fired At" + new Vector2(hit.X, hit.Y + 1));
                        return true;
                    }
                    else if (boats.FireAt(new Vector2(hit.X - 1, hit.Y)))
                    {
                        System.Diagnostics.Debug.WriteLine("Fired At" + new Vector2(hit.X - 1, hit.Y));
                        return true;
                    }
                    else if (boats.FireAt(new Vector2(hit.X, hit.Y - 1)))
                    {
                        System.Diagnostics.Debug.WriteLine("Fired At" + new Vector2(hit.X, hit.Y - 1));
                        return true;
                    }
                }
            }

            loc = new Vector2(rnd.Next(x), rnd.Next(y));
            while (!boats.FireAt(loc))
            {
                loc = new Vector2(rnd.Next(x), rnd.Next(y));
            }

            System.Diagnostics.Debug.WriteLine("Fired At" + loc);

            return true;
        }
    }
}
