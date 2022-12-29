using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project_Evo_Main_Solution
{
    public class Camera
    {
        public Matrix transformMatrix { get; private set; }
        public Matrix position { get; private set; }
        public Matrix offset { get; private set; }

        public void Follow(Player target)
        {
            position = Matrix.CreateTranslation(-target.spritePosition.X - (target.spriteText.Width / 2), -target.spritePosition.Y - (target.spriteText.Height / 2), 0);

            offset = Matrix.CreateTranslation(Game1.screenWidth / 2, Game1.screenHeight / 2, 0);

            transformMatrix = position * offset;
        }

    }
}
