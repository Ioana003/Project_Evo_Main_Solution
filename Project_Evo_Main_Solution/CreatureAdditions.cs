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
    public class CreatureAdditions : Movable
    {
        public float length { get; set; }
        public float width { get; set; }
        public float depth { get; set; }
        private float density = 1; // 1 by default for testing
        /*
         * Different densities:
         * - eyes:
         * - muscle/flesh:
         * - fur:
         * - scales:
         * - skin/leather:
         */

        public float totalSurfaceArea { get; set; }
        public float totalVolume { get; set; }
        public float health { get; set; }
        public float individualHardiness { get; set; }
        private Random random = new Random();
        public string partType;

        public Vector2 distanceFromMain;

        public bool rotateLeft = false;
        public bool rotateRight = false;
        public bool moveForward = false;
        public bool moveBackwards = false;

        public CreatureAdditions(Texture2D inTexture, Vector2 inSpritePosition, Color inColour, Vector2 inSpriteSize, string inPartType, float inLinearVel, float inRotationVel) : base(inTexture, inSpritePosition, inColour, inSpriteSize)
        {
            // Checks which type of body part it is as they all have different functions
            if(inPartType == "body")
            {
                this.length = random.NextFloat(1, 15);
                this.width = random.NextFloat(1, 15);
                this.depth = random.NextFloat(1, 15);
            }    
            else if (inPartType == "main")
            {
                this.length = random.NextFloat(1, 15);
                this.width = random.NextFloat(1, 15);
                this.depth = random.NextFloat(1, 15);

                distanceFromMain = new Vector2(0, 0);
                origin = new Vector2(spritePosition.X + spriteSize.X / 2, spritePosition.Y + spriteSize.Y / 2);
            }
            else if(inPartType == "eyes" || inPartType == "teeth")
            {
                this.length = random.NextFloat(1, 5);
                this.width = random.NextFloat(1, 5);
                this.depth = random.NextFloat(1, 5);

            }
            else if(inPartType == "mouth")
            {
                this.length = random.NextFloat(1, 10);
                this.width = random.NextFloat(1, 10) * 2;
                this.depth = random.NextFloat(1, 10);

            }

            this.totalSurfaceArea = length * width;
            this.totalVolume = this.totalSurfaceArea * this.depth;
            this.health = this.totalVolume * this.density;
            this.individualHardiness = this.random.NextFloat(1, 5);

            position = new Rectangle((int)inSpritePosition.X, (int)inSpritePosition.Y, (int)width, (int)length);
            spritePosition = new Vector2(inSpritePosition.X, inSpritePosition.Y);
            spriteText = inTexture;
            spriteColour = inColour;
            spriteSize = new Vector2(width, length);
            sourceRectangle = new Rectangle(position.X, position.Y, (int)spriteSize.X, (int)spriteSize.Y);

            partType = inPartType;

            rotationVelocity = inRotationVel;
            linearVelocity = inLinearVel;

        }

        // This method ensures that when the part is moved (because of the neural network), it updates its position
        public void UpdatePart()
        {

            if (rotateLeft == true)
            {
                rotation -= MathHelper.ToRadians(rotationVelocity);

                spritePosition = origin + distanceFromMain;
                position = new Rectangle((int)spritePosition.X, (int)spritePosition.Y, (int)width, (int)length);
            }
            else if(rotateRight == true)
            {
                rotation += MathHelper.ToRadians(rotationVelocity);

                spritePosition = origin + distanceFromMain;
                position = new Rectangle((int)spritePosition.X, (int)spritePosition.Y, (int)width, (int)length);
            }

            var direction = new Vector2((float)Math.Cos(rotation), -(float)Math.Sin(rotation));

            if(moveForward == true)
            {
                spritePosition += direction * linearVelocity;
                position = new Rectangle((int)spritePosition.X, (int)spritePosition.Y, (int)width, (int)length);

            }
            else if(moveBackwards == true)
            {
                spritePosition -= direction * linearVelocity;
                position = new Rectangle((int)spritePosition.X, (int)spritePosition.Y, (int)width, (int)length);
            }

        }

    }
}
