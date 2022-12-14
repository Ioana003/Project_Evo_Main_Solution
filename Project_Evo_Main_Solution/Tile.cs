using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Evo_Prot_5___Tile_Map
{
    internal class Tile
    {
        private Texture2D texture;
        private Rectangle position;
        private Color colour;
        private string tileType;

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
