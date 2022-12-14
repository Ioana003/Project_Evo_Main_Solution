using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TileMap_Y12
{
    class Sprite
    {
        protected Texture2D spriteText;
        protected Vector2 spritePosition;
        protected Rectangle? sourceRectangle;
        protected Color spriteColour;
        

        public Sprite()
        {
        }

        public Sprite(Texture2D inTexture, Vector2 inSpritePosition, Color inColour)
        {
            spriteText = inTexture;
            spritePosition = inSpritePosition;
            spriteColour = inColour;
        }

        public Sprite(Texture2D inTexture, Vector2 inSpritePosition, Rectangle? inSource, Color inColour)
        {
            spriteText = inTexture;
            spritePosition = inSpritePosition;
            sourceRectangle = inSource;
            spriteColour = inColour;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(spriteText, new Vector2(spritePosition.X, spritePosition.Y), sourceRectangle, spriteColour);
        }

    }
}
