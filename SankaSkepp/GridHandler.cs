using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace SankaSkepp
{
    class GridHandler
    {
        GraphicsDevice graphics;
        List<Line> horizontal, vertical;
        Rectangle screen, cell;
        int numHorizontal, numVertical, windowWidth, windowHeight;

        public GridHandler(GraphicsDeviceManager graphics, int horizontal, int vertical, int windowWidth, int windowHeight)
        {
            this.graphics = graphics.GraphicsDevice;
            this.horizontal = new List<Line>();
            this.vertical = new List<Line>();
            numHorizontal = horizontal;
            numVertical = vertical;
            this.windowWidth = windowWidth;
            this.windowHeight = windowHeight;
            float x, y;

            for (int i = 0; i <= horizontal; i++)
            {
                y = i * windowHeight / vertical;
                this.horizontal.Add(new Line(this.graphics, 1, windowWidth, Color.White, new Vector2(0, y)));
            }

            for (int i = 0; i <= vertical; i++)
            {
                x = i * windowWidth / horizontal;
                this.vertical.Add(new Line(this.graphics, windowHeight, 1, Color.White, new Vector2(x, 0)));
            }

            screen = new Rectangle(0, 0, windowWidth, windowHeight);
            cell = new Rectangle(0, 0, windowWidth / horizontal, windowHeight / vertical);
        }

        public Rectangle Cell
        {
            get { return cell; }
        }

        public Rectangle Screen
        {
            get { return screen; }
        }

        public int NumHorizontal
        {
            get { return numHorizontal; }
            set
            {
                if (value > 25)
                    numHorizontal = 25;
                else if (value < 5)
                    numHorizontal = 5;
                else
                    numHorizontal = value;
            }
        }

        public int NumVertical
        {
            get { return numVertical; }
            set
            {
                if (value > 25)
                    numVertical = 25;
                else if (value < 5)
                    numVertical = 5;
                else
                    numVertical = value;
            }
        }

        public void UpdateGrid()
        {
            this.horizontal.Clear();
            this.vertical.Clear();
            float x, y;

            for (int i = 0; i <= numHorizontal; i++)
            {
                y = i * windowHeight / numHorizontal;
                this.horizontal.Add(new Line(this.graphics, 1, windowWidth, Color.White, new Vector2(0, y)));
            }

            for (int i = 0; i <= numVertical; i++)
            {
                x = i * windowWidth / numVertical;
                this.vertical.Add(new Line(this.graphics, windowHeight, 1, Color.White, new Vector2(x, 0)));
            }
            
            cell = new Rectangle(0, 0, windowWidth / numHorizontal, windowHeight / numVertical);
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Line line in horizontal)
            {
                //spriteBatch.Draw(line.Texture, line.Position, Color.White);
                spriteBatch.Draw(
                    line.Texture,
                    line.Position,
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    line.Scale,
                    SpriteEffects.None,
                    0f
                );
            }
            foreach (Line line in vertical)
            {
                spriteBatch.Draw(line.Texture, line.Position, Color.White);
            }
        }

        public Vector2 WichCell(int x, int y)
        {
            if (screen.Contains(x, y))
                return new Vector2(x / cell.Width, y / cell.Height);

            return new Vector2(-1, -1);
        }
    }
}
