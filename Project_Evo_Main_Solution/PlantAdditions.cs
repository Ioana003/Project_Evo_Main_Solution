using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Project_Evo_Main_Solution
{
    public class PlantAdditions : Movable
    {
        public float length { get; set; }
        public float width { get; set; }
        public float depth { get; set; }
        private float density = 1; // Leaves have a very complicated density, but using the value of water is a rough estimate that works
        public float totalSurfaceArea { get; set; }
        public float totalVolume { get; set; }
        public float health { get; set; }
        public float individualHardiness { get; set; }
        private Random random = new Random();

        public PlantAdditions(Texture2D inTexture, Vector2 inSpritePosition, Color inColour, Vector2 inSpriteSize) : base(inTexture, inSpritePosition, inColour, inSpriteSize)
        {
            this.length = random.NextFloat(0.1f, 10);
            this.width = random.NextFloat(0.1f, 10);
            this.depth = random.NextFloat(0.1f, 10);

            this.totalSurfaceArea = length * width;
            this.totalVolume = this.totalSurfaceArea * this.depth;
            this.health = this.totalVolume * this.density; // This is more so it resembles the creatures classes
            this.individualHardiness = this.random.NextFloat(0.1f, 5);

            position = new Rectangle((int)inSpritePosition.X, (int)inSpritePosition.Y, (int)width, (int)length);
            spritePosition = new Vector2(inSpritePosition.X, inSpritePosition.Y);
            spriteText = inTexture;
            spriteColour = inColour;
            spriteSize = new Vector2(width, length);
            sourceRectangle = new Rectangle(position.X, position.Y, (int)spriteSize.X, (int)spriteSize.Y);
        }


    }
}
