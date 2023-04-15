using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Evo_Main_Solution
{
    public class Sprite
    {
        public Texture2D spriteText { get; set; }
        public Vector2 spritePosition { get; set; }
        public Rectangle? sourceRectangle { get; set; }
        public Color spriteColour { get; set; }
        public Vector2 spriteSize { get; set; }

        public float rotation;
        public Vector2 origin;

        public float rotationVelocity = 0.5f;
        public float linearVelocity = 0.5f;


        public Sprite()
        {
        }

        public Sprite(Texture2D inTexture, Vector2 inSpritePosition, Color inColour, Vector2 inSpriteSize)
        {
            spriteText = inTexture;
            spritePosition = inSpritePosition;
            spriteColour = inColour;
            spriteSize = inSpriteSize;
            origin = inSpritePosition + inSpriteSize / 2;

            rotation = 0f;
        }

        public Sprite(Texture2D inTexture, Vector2 inSpritePosition, Rectangle? inSource, Vector2 inSpriteSize, Color inColour)
        {
            spriteText = inTexture;
            spritePosition = inSpritePosition;
            sourceRectangle = inSource;
            spriteColour = inColour;
            spriteSize = inSpriteSize;
            origin = inSpritePosition + inSpriteSize / 2;

            rotation = 0f;
        }


        public void Draw(SpriteBatch spriteBatch, float scale)
        {
            spriteBatch.Draw(spriteText, spritePosition, sourceRectangle, spriteColour, rotation, new Vector2(0, 0), scale, SpriteEffects.None, 0);
            
        }

    }
}
