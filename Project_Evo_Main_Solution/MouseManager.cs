using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Evo_Prototype_1___Map_Creation
{
    class MouseManager
    {
        private bool hasclicked = false;

        public MouseManager()
        {

        }

        public bool CheckIfClicked(Rectangle textBox)
        {
            Rectangle mousePosition = new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);

            if(textBox.Intersects(mousePosition))
            {
                if(Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    hasclicked = true;
                }
            }
            else
            {
                if(Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    hasclicked = false;
                }
            }

            return hasclicked;
        }
        
    }
}
