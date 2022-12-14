using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Evo_Neural_Network_Prototype_1
{
    class Movable
    {
        public Rectangle position { get; set; }
        public Texture2D texture { get; set; }

        public Movable(Rectangle inPosition, Texture2D inTexture)
        {
            position = inPosition;
            texture = inTexture;
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
