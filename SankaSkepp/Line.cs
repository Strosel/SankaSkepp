using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SankaSkepp
{
    class Line
    {
        Texture2D line;
        int height, width;
        Color[] color;
        Vector2 position;
        Vector2 scale;

        public Line(GraphicsDevice graphicsDevice, int height, int width, Color color, Vector2 position)
        {
            this.height = height;
            this.width = width;
            line = new Texture2D(graphicsDevice, width, height);

            this.color = new Color[width * height];
            for (int i = 0; i < this.color.Length; ++i)
            {
                this.color[i] = color;
                line.SetData(this.color);
            }

            this.position = position;
            scale = Vector2.One;
        }

        public Texture2D Texture
        {
            get { return line; }
            set { line = value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public Vector2 Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        public int Height
        {
            get { return height; }
        }

        public int Width
        {
            get { return width; }
        }
        
    }
}
