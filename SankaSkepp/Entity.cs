using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SankaSkepp
{
    class Entity
    {
        Texture2D texture;
        Vector2 position;
        float speed;
        float movementAngle;
        float viewAngle;

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public float Height
        {
            get { return this.texture.Height; }
        }

        public float Width
        {
            get { return this.texture.Width; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float X
        {
            get { return position.X; }
            set { position.X = value; }
        }

        public float Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public float MovementAngle
        {
            get { return movementAngle; }
            set { movementAngle = value; }
        }

        public float ViewAngle
        {
            get { return viewAngle; }
            set { viewAngle = value; }
        }
    }
}
