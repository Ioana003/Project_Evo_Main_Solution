using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Evo_Main_Solution
{
    public class Player : Sprite
    {

        public Player(Texture2D inText)
        {
            spriteText= inText;
        }

        public void MovePlayer()
        {
            // Left
            if(Keyboard.GetState().IsKeyDown(Keys.A))
            {
                spritePosition = new Vector2(spritePosition.X - 10, spritePosition.Y);
            }

            // Right
            if(Keyboard.GetState().IsKeyDown(Keys.D))
            {
                spritePosition = new Vector2(spritePosition.X + 10, spritePosition.Y);
            }

            // Up
            if(Keyboard.GetState().IsKeyDown(Keys.W))
            {
                spritePosition = new Vector2(spritePosition.X, spritePosition.Y - 10);
            }

            // Down
            if(Keyboard.GetState().IsKeyDown(Keys.S))
            {
                spritePosition = new Vector2(spritePosition.X, spritePosition.Y + 10);
            }
        }

    }
}
