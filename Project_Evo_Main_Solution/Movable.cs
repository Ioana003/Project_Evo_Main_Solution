using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Evo_Main_Solution
{
    public class Movable : Sprite
    {
        public Rectangle position { get; set; }

        public Movable(Texture2D inTexture, Vector2 inSpritePosition, Color inColour, Vector2 inSpriteSize) : base(inTexture, inSpritePosition, inColour, inSpriteSize)
        {
            position = new Rectangle((int)inSpritePosition.X, (int)inSpritePosition.Y, (int)inSpriteSize.X, (int)inSpriteSize.Y);
            sourceRectangle = position;
        }

        public void MoveRight(int cellSize)
        {
            position = new Rectangle(position.X + cellSize, position.Y, position.Width, position.Height);
        }

        public void MoveLeft(int cellSize)
        {
            position = new Rectangle(position.X - cellSize, position.Y, position.Width, position.Height);
        }

        public void MoveUp(int cellSize)
        {
            position = new Rectangle(position.X, position.Y - cellSize, position.Width, position.Height);
        }

        public void MoveDown(int cellSize)
        {
            position = new Rectangle(position.X, position.Y + cellSize, position.Width, position.Height);
        }

    }
}
