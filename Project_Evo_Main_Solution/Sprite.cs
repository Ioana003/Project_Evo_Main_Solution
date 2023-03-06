using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Evo_Main_Solution
{
    public class Sprite
    {
        public Texture2D spriteText { get; set; }
        public Vector2 spritePosition { get; set; }
        protected Rectangle? sourceRectangle;
        public Color spriteColour { get; set; }
        public Vector2 spriteSize { get; set; }


        public Sprite()
        {
        }

        public Sprite(Texture2D inTexture, Vector2 inSpritePosition, Color inColour, Vector2 inSpriteSize)
        {
            spriteText = inTexture;
            spritePosition = inSpritePosition;
            spriteColour = inColour;
            spriteSize = inSpriteSize;
        }

        public Sprite(Texture2D inTexture, Vector2 inSpritePosition, Rectangle? inSource, Vector2 inSpriteSize, Color inColour)
        {
            spriteText = inTexture;
            spritePosition = inSpritePosition;
            sourceRectangle = inSource;
            spriteColour = inColour;
            spriteSize = inSpriteSize;
        }

        public void Draw(SpriteBatch spriteBatch, float scale)
        {
            spriteBatch.Draw(spriteText, spritePosition, sourceRectangle, spriteColour, 0, new Vector2(spriteSize.X / 2, spriteSize.Y / 2), scale, SpriteEffects.None, 0);
        }

    }
}
