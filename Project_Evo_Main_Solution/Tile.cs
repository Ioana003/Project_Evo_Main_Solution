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
    internal class Tile
    {
        private Texture2D texture;
        public Rectangle position { get; set; }
        private Color colour;
        public string tileType { get; set; }

        public Tile()
        {

        }

        public Tile(Texture2D inTexture, Rectangle inPosition, Color inColour, string inType)
        {
            texture = inTexture;
            position = inPosition;
            colour = inColour;
            tileType = inType;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Draw(texture, position, colour);
        }

    }
}
