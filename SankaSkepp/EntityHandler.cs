using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SankaSkepp
{
    class EntityHandler
    {
        List<Entity> entities;

        public EntityHandler()
        {
            entities = new List<Entity>();
        }

        public void Add(Entity entity)
        {
            entities.Add(entity);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Entity entity in entities)
            {
                spriteBatch.Draw(
                    entity.Texture,
                    entity.Position,
                    null,
                    Color.White,
                    entity.ViewAngle,
                    new Vector2(entity.Width / 2, entity.Height / 2),
                    Vector2.One,
                    SpriteEffects.None,
                    0f
                );
            }

        }
    }
}
