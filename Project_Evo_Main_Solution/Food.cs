using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Project_Evo_Main_Solution
{
    public class Food : Sprite
    {

        public int foodType = -1;
        public int foodAmount = -1;
        public int totalFood = -1;
        private int timeAlive = 0;
        private int decomposeTime = 120;

        /* Food types:
         * 0 - Plant
         * 1 - Meat
         */

        // Two constructors based on the type of food
        public Food(PlantAdditions plantFoodType, Texture2D inTexture, Vector2 inSpritePosition, Color inColour, Vector2 inSpriteSize) : base (inTexture, inSpritePosition, inColour, inSpriteSize)
        {
            spritePosition = plantFoodType.spritePosition;
            spriteSize = plantFoodType.spriteSize;
            sourceRectangle = new Rectangle((int)spritePosition.X, (int)spritePosition.Y, (int)spriteSize.X, (int)spriteSize.Y);
            spriteText = inTexture;
            spriteColour = inColour;
            foodType = 0;
            totalFood = (int)plantFoodType.health;
            foodAmount = totalFood;
        }

        public Food(CreatureAdditions creatureFoodType, Texture2D inTexture, Vector2 inSpritePosition, Color inColour, Vector2 inSpriteSize) : base(inTexture, inSpritePosition, inColour, inSpriteSize)
        {
            spritePosition = creatureFoodType.spritePosition;
            spriteSize = creatureFoodType.spriteSize;
            sourceRectangle = new Rectangle((int)spritePosition.X, (int)spritePosition.Y, (int)spriteSize.X, (int)spriteSize.Y);
            spriteText = inTexture;
            spriteColour = inColour;
            foodType = 0;
            totalFood = (int)creatureFoodType.health;
            foodAmount = totalFood;
        }

        // Every time an action can be taken, some food is reduced until it is empty
        // That food is converted in soil nutrients
        public void Decompose(Tile[,] tileArray)
        {
            timeAlive++;

            if(timeAlive <= decomposeTime)
            {
                foodAmount = foodAmount - (totalFood / decomposeTime);
            }
            else
            {
                foreach(Tile t in tileArray)
                {
                    if(t.position.Intersects(new Rectangle((int)spritePosition.X, (int)spritePosition.Y, (int)spriteSize.X, (int)spriteSize.Y)))
                    {
                        t.nutrientAmount = t.nutrientAmount + totalFood;
                    }
                }
            }
        }

    }
}
