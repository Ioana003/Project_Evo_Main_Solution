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
