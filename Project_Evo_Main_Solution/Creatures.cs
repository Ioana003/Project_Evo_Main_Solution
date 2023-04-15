using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Project_Evo_Main_Solution
{
    public class Creatures : Movable
    {
        private float feltTemperature; // What the temperature in the surrounding of the plant is
        private float[] temperatureRange = new float[2]; // The range in which the plant can live in; 0 - minimum, 1 - maximum
        public float maxHealth { get; set; } // The health amount the plant has which can be drained
        private float maxEnergy; // The energy amount the plant has which can be depleted
        public float health;
        private float energy;
        public float totalSurfaceArea { get; set; } // Total surface area of the plant
        public float totalVolume { get; set; } // Total volume of the plant
        private int legAmount;
        public CreatureAdditions[] legs;
        private int bodyPartsAmount;
        public CreatureAdditions[] bodyParts { get; set; }
        public CreatureAdditions[] mouth { get; set; }
        private int teethAmount;
        public CreatureAdditions[] teeth { get; set; }
        private int height;
        private float ratioEnergyHealth; // The amount, set by the user, by which energy is greater than health
        private Random random = new Random();
        private Creatures[] creatureThreatLevelID;
        private int[] creatureThreatLevelAmount;
        private int creatureID;
        private int aggressiveness;
        public int reproductiveDrive { get; private set; }
        private int nutrients;
        private int gutLength;
        private int gutDiameter;
        private int gutArea;
        private float digestionEfficiency;
        private string reproductiveType;
        private int eggSturdiness;
        private int eggNutrients;
        public float age = 0;
        public float maturity;
        public bool oldAge = false;
        private int eggIncubation;
        public float gestationTime;
        private bool eggHatched;
        private int eggAge;
        private float dietType;
        private int eyeAmount;
        public CreatureAdditions[] eyes { get; set; }
        public bool matured = false;
        private float speed;
        public bool allowReproduction = false;
        public bool biting = false;

        public NNMaker neuralNet;
        private float[] inputs;

        public Creatures(Texture2D inTexture, Vector2 inSpritePosition, Color inColour, Vector2 inSpriteSize, float inFeltTemperature, float inEnergyHealthRatio, int inCreatureID, bool inAbleToReproduce) : base(inTexture, inSpritePosition, inColour, inSpriteSize)
        {
            origin = inSpritePosition;

            rotationVelocity = random.NextFloat(0, 4);
            linearVelocity = random.NextFloat(0, 3);

            age = 0;
            aggressiveness = random.Next(0, 5);
            reproductiveDrive = random.Next(0, 100);
            gutLength = random.Next(1, 5);
            gutDiameter = random.Next(1, 5);
            gutArea = gutDiameter * gutLength;
            digestionEfficiency = random.NextFloat(0, 1);
            eggSturdiness = random.Next(0, 10);
            maturity = random.NextFloat(0, 30);
            gestationTime = random.Next(0, 120);
            eggIncubation = random.Next(0, 120);
            eggAge = 0;

            dietType = random.NextFloat(0, 1);

            // Creates the body parts; bodyParts[0] is the "main" part that everything else is glued onto
            // There are: eyes, mouth, teeth, and body parts
            bodyPartsAmount = random.Next(1, 4);
            bodyParts = new CreatureAdditions[bodyPartsAmount];
            bodyParts[0] = new CreatureAdditions(inTexture, inSpritePosition, Color.DarkRed, inSpriteSize, "main", linearVelocity, rotationVelocity);
            bodyParts[0].origin = new Vector2(inSpritePosition.X + bodyParts[0].spriteSize.X / 2, inSpritePosition.Y + bodyParts[0].spriteSize.Y / 2);
            totalSurfaceArea = totalSurfaceArea + bodyParts[0].totalSurfaceArea;
            totalVolume = totalVolume + bodyParts[0].totalVolume;
            maxHealth = maxHealth + bodyParts[0].health;

            for (int i = 1; i < bodyParts.Length; i++)
            {
                bodyParts[i] = new CreatureAdditions(inTexture, new Vector2(bodyParts[0].spritePosition.X + random.Next(-10, 10), bodyParts[0].spritePosition.Y + random.Next(-10, 10)), inColour, inSpriteSize, "body", linearVelocity, rotationVelocity);
                bodyParts[i].origin = bodyParts[0].origin;
                bodyParts[i].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - bodyParts[i].spritePosition.X, bodyParts[0].spritePosition.Y - bodyParts[i].spritePosition.Y);

                totalSurfaceArea = totalSurfaceArea + bodyParts[i].totalSurfaceArea;
                totalVolume = totalVolume + bodyParts[i].totalVolume;
                maxHealth = maxHealth + bodyParts[i].health;
            }

            eyeAmount = random.Next(1, 5);
            eyes = new CreatureAdditions[eyeAmount];
            for(int i = 0; i < eyes.Length; i++)
            {
                eyes[i] = new CreatureAdditions(inTexture, new Vector2(random.Next((int)bodyParts[0].spritePosition.X, (int)bodyParts[0].spritePosition.X + (int)bodyParts[0].spriteSize.X), random.Next((int)bodyParts[0].spritePosition.Y, (int)bodyParts[0].spritePosition.Y + (int)bodyParts[0].spriteSize.Y)), Color.White, inSpriteSize, "eyes", linearVelocity, rotationVelocity);
                eyes[i].origin = bodyParts[0].origin;
                eyes[i].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - eyes[i].spritePosition.X, bodyParts[0].spritePosition.Y - eyes[i].spritePosition.Y);

                totalSurfaceArea = totalSurfaceArea + eyes[i].totalSurfaceArea;
                totalVolume = totalVolume + eyes[i].totalVolume;
                maxHealth = maxHealth + eyes[i].health;
            }    

            legAmount = random.Next(0, 4);
            speed = random.Next(0, 10) * legAmount;

            mouth = new CreatureAdditions[2];
            mouth[0] = new CreatureAdditions(inTexture, new Vector2(bodyParts[0].spritePosition.X + bodyParts[0].spriteSize.X, bodyParts[0].spritePosition.Y), Color.DarkSlateGray, inSpriteSize, "mouth", linearVelocity, rotationVelocity);
            mouth[0].origin = bodyParts[0].origin;
            mouth[0].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - mouth[0].spritePosition.X, bodyParts[0].spritePosition.Y - mouth[0].spritePosition.Y);
            totalSurfaceArea = totalSurfaceArea + mouth[0].totalSurfaceArea;
            totalVolume = totalVolume + mouth[0].totalVolume;
            maxHealth = maxHealth + mouth[0].health;

            mouth[1] = new CreatureAdditions(inTexture, new Vector2(bodyParts[0].spritePosition.X + bodyParts[0].spriteSize.X, bodyParts[0].spritePosition.Y + bodyParts[0].spriteSize.Y), Color.DarkSlateGray, inSpriteSize, "mouth", linearVelocity, rotationVelocity);
            mouth[1].origin = bodyParts[0].origin;
            mouth[1].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - mouth[1].spritePosition.X, bodyParts[0].spritePosition.Y - mouth[1].spritePosition.Y);
            totalSurfaceArea = totalSurfaceArea + mouth[1].totalSurfaceArea;
            totalVolume = totalVolume + mouth[1].totalVolume;
            maxHealth = maxHealth + mouth[1].health;

            teethAmount = random.Next(0, 6);
            teeth = new CreatureAdditions[teethAmount];
            for(int i = 0; i < teeth.Length / 2; i++)
            {
                teeth[i] = new CreatureAdditions(inTexture, new Vector2(random.Next((int)mouth[0].spritePosition.X, (int)mouth[0].spritePosition.X + (int)mouth[0].spriteSize.X), mouth[0].spritePosition.Y + random.Next(0, 5)), Color.Black, inSpriteSize, "teeth", linearVelocity, rotationVelocity);
                teeth[i].origin = bodyParts[0].origin;
                teeth[i].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - teeth[i].spritePosition.X, bodyParts[0].spritePosition.Y - teeth[i].spritePosition.Y);

                totalSurfaceArea = totalSurfaceArea + teeth[i].totalSurfaceArea;
                totalVolume = totalVolume + teeth[i].totalVolume;
                maxHealth = maxHealth + teeth[i].health;
            }
            for (int i = teeth.Length / 2; i < teeth.Length; i++)
            {
                teeth[i] = new CreatureAdditions(inTexture, new Vector2(random.Next((int)mouth[1].spritePosition.X, (int)mouth[1].spritePosition.X + (int)mouth[1].spriteSize.X), mouth[1].spritePosition.Y + random.Next(-5, 0)), Color.Black, inSpriteSize, "teeth", linearVelocity, rotationVelocity);
                teeth[i].origin = bodyParts[0].origin;
                teeth[i].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - teeth[i].spritePosition.X, bodyParts[0].spritePosition.Y - teeth[i].spritePosition.Y);

                totalSurfaceArea = totalSurfaceArea + teeth[i].totalSurfaceArea;
                totalVolume = totalVolume + teeth[i].totalVolume;
                maxHealth = maxHealth + teeth[i].health;
            }

            maxEnergy = maxHealth * inEnergyHealthRatio;

            energy = maxEnergy;
            health = maxHealth;

            int[] layers = new int[5];
            for(int i = 0; i < layers.Length; i++)
            {
                layers[i] = eyeAmount + 1;
            }

            neuralNet = new NNMaker(layers);
            inputs = new float[eyeAmount + 1];

            /*
             * Input are:
             * - The number of eyes, which can see stuff independently
             * - Temperature
             * - Creatures it sees
             * - Plants it sees
             */
        }

        public void NeuralNet(Tile[,] tileArray, List<Food> listFood)
        {

            /*
             * Outputs are:
             * - [0] Move forward
             * - [1] Move backwards
             * - [2] Turn left
             * - [3] Turn right
             * - [4] Reproduce
             * - [5] Digestion
             * - [6] Absorb Nutrients
             * - [7] Bite
             * - [8] Heal
             */

            // Each body part needs a move output individually
            float[] outputs = neuralNet.FeedForward(inputs);

            foreach (Tile t in tileArray)
            {
                foreach (CreatureAdditions a in bodyParts)
                {
                    if (outputs[0] >= 0.5)
                    {
                        a.moveForward = true;
                        energy -= 10;
                    }
                    else
                    {
                        a.moveForward = false;
                    }

                    if (outputs[1] >= 0.5)
                    {
                        a.moveBackwards = true;
                        energy -= 10;
                    }
                    else
                    {
                        a.moveBackwards = false;
                    }

                    if (outputs[2] >= 0.5)
                    {
                        a.rotateLeft = true;
                        energy -= 10;
                    }
                    else
                    {
                        a.rotateLeft = false;
                    }

                    if (outputs[3] >= 0.5)
                    {
                        a.rotateRight = true;
                        energy -= 10;
                    }
                    else
                    {
                        a.rotateRight = false;
                    }

                    if (t.position.Intersects(bodyParts[0].position) && t.tileType == "Rock")
                    {
                        a.moveForward = false;
                        a.moveBackwards = false;
                    }


                    a.UpdatePart();
                }

                foreach (CreatureAdditions a in mouth)
                {
                    if (outputs[0] >= 0.5)
                    {
                        a.moveForward = true;
                    }
                    else
                    {
                        a.moveForward = false;
                    }

                    if (outputs[1] >= 0.5)
                    {
                        a.moveBackwards = true;
                    }
                    else
                    {
                        a.moveBackwards = false;
                    }

                    if (outputs[2] >= 0.5)
                    {
                        a.rotateLeft = true;
                    }
                    else
                    {
                        a.rotateLeft = false;
                    }

                    if (outputs[3] >= 0.5)
                    {
                        a.rotateRight = true;
                    }
                    else
                    {
                        a.rotateRight = false;
                    }

                    if (t.position.Intersects(bodyParts[0].position) && t.tileType == "Rock")
                    {
                        a.moveForward = false;
                        a.moveBackwards = false;
                    }


                    a.UpdatePart();
                }

                foreach (CreatureAdditions a in eyes)
                {
                    if (outputs[0] >= 0.5)
                    {
                        a.moveForward = true;
                    }
                    else
                    {
                        a.moveForward = false;
                    }

                    if (outputs[1] >= 0.5)
                    {
                        a.moveBackwards = true;
                    }
                    else
                    {
                        a.moveBackwards = false;
                    }

                    if (outputs[2] >= 0.5)
                    {
                        a.rotateLeft = true;
                    }
                    else
                    {
                        a.rotateLeft = false;
                    }

                    if (outputs[3] >= 0.5)
                    {
                        a.rotateRight = true;
                    }
                    else
                    {
                        a.rotateRight = false;
                    }

                    if (t.position.Intersects(bodyParts[0].position) && t.tileType == "Rock")
                    {
                        a.moveForward = false;
                        a.moveBackwards = false;
                    }


                    a.UpdatePart();
                }

                foreach (CreatureAdditions a in teeth)
                {
                    if (outputs[0] >= 0.5)
                    {
                        a.moveForward = true;
                    }
                    else
                    {
                        a.moveForward = false;
                    }

                    if (outputs[1] >= 0.5)
                    {
                        a.moveBackwards = true;
                    }
                    else
                    {
                        a.moveBackwards = false;
                    }

                    if (outputs[2] >= 0.5)
                    {
                        a.rotateLeft = true;
                    }
                    else
                    {
                        a.rotateLeft = false;
                    }

                    if (outputs[3] >= 0.5)
                    {
                        a.rotateRight = true;
                    }
                    else
                    {
                        a.rotateRight = false;
                    }

                    if (t.position.Intersects(bodyParts[0].position) && t.tileType == "Rock")
                    {
                        a.moveForward = false;
                        a.moveBackwards = false;
                    }


                    a.UpdatePart();
                }

            }

            // Rest of the outputs
            if (outputs[4] >= 0.5)
            {
                allowReproduction = true;
                energy -= 10;
            }
            else
            {
                allowReproduction = false;
                energy -= 10;
            }

            if (outputs[5] >= 0.5)
            {
                Digestion();
                energy -= 10;
            }

            if (outputs[6] >= 0.5)
            {
                AbsorbNutrients(listFood);
                energy -= 10;
            }

            if (outputs[7] >= 0.5)
            {
                Bite(20);
                energy -= 10;
            }

            if (outputs[8] >= 0.5)
            {
                Heal();
                energy -= 10;
            }

            /*
             * Outputs are (so far):
             * - [0] Move forward
             * - [1] Move backwards
             * - [2] Turn left
             * - [3] Turn right
             * - [4] Reproduce
             * - [5] Digestion
             * - [6] Absorb Nutrients
             * - [7] Bite
             * - [8] Heal
             */

            // All outputs use some energy
        }

        // Decides what the inputs are and the number associated with them
        public void DecideInputs(Plants[] plantArray, Creatures[] creaturesArray, Tile[,] tileArray)
        {
            for(int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = 0;
            }

            foreach (Plants p in plantArray)
            {
                for(int e = 0; e < eyes.Length; e++)
                {
                    if(p.position.Intersects(new Rectangle(eyes[e].position.X - 20, eyes[e].position.Y - 20, eyes[e].position.X + 20, eyes[e].position.Y + 20)))
                    {
                        inputs[e] += 1;
                    }
                }
            }

            if(feltTemperature <= temperatureRange[0])
            {
                inputs[eyeAmount + 1 - 1] -= 1;
            }
            else if(feltTemperature >= temperatureRange[1])
            {
                inputs[eyeAmount + 1 - 1] += 1;
            }
            else if(feltTemperature >= temperatureRange[0] && feltTemperature <= temperatureRange[1])
            {
                inputs[eyeAmount + 1 - 1] = 0;
            }

            // These calculations (eyeAmount + 1 - 1) are redundant; However, I wrote them like this because it's easier to read
            // Essentially, I want to look at all of the inputs after the eyes have been taken care of
            // However, adding 1 to the eye amount skips one of the inputs, hence I must always take one away too
            // If I left it as inputs[eyeAmount] alone, I would easily get confused, so I chose to leave it as inputs[eyeAmount + 1 - 1]

            foreach (Creatures c in creaturesArray)
            {
                for (int e = 0; e < eyes.Length; e++)
                {
                    if (c.position.Intersects(new Rectangle(eyes[e].position.X - 20, eyes[e].position.Y - 20, eyes[e].position.X + 20, eyes[e].position.Y + 20)))
                    {
                        inputs[e] += 1;
                    }
                }
            }

        }


        public void LearnIDs(List<Creatures> listCreatures)
        {
            creatureThreatLevelID = listCreatures.ToArray();

            creatureThreatLevelAmount = new int[creatureThreatLevelID.Length];
        }

        public void AssignThreat(int i)
        {
            foreach(CreatureAdditions e in eyes)
            {

                if (creatureThreatLevelID[i].position.Intersects(new Rectangle(e.position.X - 20, e.position.Y - 20, e.position.X + 20, e.position.Y + 20)))
                {
                    creatureThreatLevelAmount[i] += aggressiveness; // Increase aggressiveness if it sees a stranger

                    if (creatureThreatLevelID[i].biting == true)
                    {
                        creatureThreatLevelAmount[i] += aggressiveness; // Increases even more if it's biting/attacking
                    }

                    if (creatureThreatLevelID[i].dietType > 0.5f)
                    {
                        creatureThreatLevelAmount[i] += aggressiveness; // Increases again if it's a carnivore/predator
                    }
                }

            }

        }

        // The main part where evolution occurs. There are many variables that can be changed
        public void Mutate()
        {
            if (random.Next(1, 100) == 1)
            {
                this.height = this.height + random.Next(-2, 2);

                if (height <= 0)
                {
                    this.height = 1;
                }
            }

            if (random.Next(1, 100) == 2)
            {
                bodyPartsAmount = random.Next(-1, 1) + bodyPartsAmount;

                if (this.bodyPartsAmount <= 0)
                {
                    this.bodyPartsAmount = 1;
                }

                this.bodyParts = new CreatureAdditions[this.bodyPartsAmount];
                bodyParts[0] = new CreatureAdditions(spriteText, spritePosition, Color.DarkRed, spriteSize, "main", linearVelocity, rotationVelocity);
                bodyParts[0].origin = new Vector2(spritePosition.X + bodyParts[0].spriteSize.X / 2, spritePosition.Y + bodyParts[0].spriteSize.Y / 2);
                totalSurfaceArea = totalSurfaceArea + bodyParts[0].totalSurfaceArea;
                totalVolume = totalVolume + bodyParts[0].totalVolume;
                maxHealth = maxHealth + bodyParts[0].health;

                for (int i = 0; i < this.bodyParts.Length; i++)
                {
                    this.bodyParts[i] = new CreatureAdditions(spriteText, new Vector2(position.X + random.Next(-5, 5), position.Y + random.Next(-5, 5)), Color.Purple, new Vector2(), "body", linearVelocity, rotationVelocity);
                    bodyParts[i].origin = bodyParts[0].origin;
                    bodyParts[i].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - bodyParts[i].spritePosition.X, bodyParts[0].spritePosition.Y - bodyParts[i].spritePosition.Y);

                    this.totalSurfaceArea = this.bodyParts[i].totalSurfaceArea + this.totalSurfaceArea;
                    this.totalVolume = this.bodyParts[i].totalVolume + this.totalVolume;
                    this.maxHealth = this.maxHealth + this.bodyParts[i].health;
                }
            }

            // Eyes
            if (random.Next(1, 100) == 3)
            {
                eyeAmount = random.Next(-1, 1) + eyeAmount;

                if (this.eyeAmount <= 0)
                {
                    this.eyeAmount = 1;
                }

                eyes = new CreatureAdditions[eyeAmount];
                for (int i = 0; i < eyes.Length; i++)
                {
                    eyes[i] = new CreatureAdditions(spriteText, new Vector2(random.Next((int)bodyParts[0].spritePosition.X, (int)bodyParts[0].spritePosition.X + (int)bodyParts[0].spriteSize.X), random.Next((int)bodyParts[0].spritePosition.Y, (int)bodyParts[0].spritePosition.Y + (int)bodyParts[0].spriteSize.Y)), Color.White, spriteSize, "eyes", linearVelocity, rotationVelocity);
                    eyes[i].origin = bodyParts[0].origin;
                    eyes[i].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - eyes[i].spritePosition.X, bodyParts[0].spritePosition.Y - eyes[i].spritePosition.Y);

                    totalSurfaceArea = totalSurfaceArea + eyes[i].totalSurfaceArea;
                    totalVolume = totalVolume + eyes[i].totalVolume;
                    maxHealth = maxHealth + eyes[i].health;
                    neuralNet.Mutate(5, 1);
                }

                inputs = new float[eyeAmount + 1];

            }

            // Mouth
            if(random.Next(1, 100) == 4)
            {
                mouth = new CreatureAdditions[2];
                mouth[0] = new CreatureAdditions(spriteText, new Vector2(bodyParts[0].spritePosition.X + bodyParts[0].spriteSize.X, bodyParts[0].spritePosition.Y), Color.DarkSlateGray, spriteSize, "mouth", linearVelocity, rotationVelocity);
                mouth[0].origin = bodyParts[0].origin;
                mouth[0].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - mouth[0].spritePosition.X, bodyParts[0].spritePosition.Y - mouth[0].spritePosition.Y);
                totalSurfaceArea = totalSurfaceArea + mouth[0].totalSurfaceArea;
                totalVolume = totalVolume + mouth[0].totalVolume;
                maxHealth = maxHealth + mouth[0].health;

                mouth[1] = new CreatureAdditions(spriteText, new Vector2(bodyParts[0].spritePosition.X + bodyParts[0].spriteSize.X, bodyParts[0].spritePosition.Y + bodyParts[0].spriteSize.Y), Color.DarkSlateGray, spriteSize, "mouth", linearVelocity, rotationVelocity);
                mouth[1].origin = bodyParts[0].origin;
                mouth[1].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - mouth[1].spritePosition.X, bodyParts[0].spritePosition.Y - mouth[1].spritePosition.Y);
                totalSurfaceArea = totalSurfaceArea + mouth[1].totalSurfaceArea;
                totalVolume = totalVolume + mouth[1].totalVolume;
                maxHealth = maxHealth + mouth[1].health;
            }

            // Teeth
            if (random.Next(1, 100) == 2)
            {
                teethAmount = random.Next(-1, 1) + teethAmount;

                if (this.teethAmount <= 1)
                {
                    this.teethAmount = 2;
                }

                for (int i = 0; i < teeth.Length / 2; i++)
                {
                    teeth[i] = new CreatureAdditions(spriteText, new Vector2(random.Next((int)mouth[0].spritePosition.X, (int)mouth[0].spritePosition.X + (int)mouth[0].spriteSize.X), mouth[0].spritePosition.Y + random.Next(0, 5)), Color.Black, spriteSize, "teeth", linearVelocity, rotationVelocity);
                    teeth[i].origin = bodyParts[0].origin;
                    teeth[i].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - teeth[i].spritePosition.X, bodyParts[0].spritePosition.Y - teeth[i].spritePosition.Y);

                    totalSurfaceArea = totalSurfaceArea + teeth[i].totalSurfaceArea;
                    totalVolume = totalVolume + teeth[i].totalVolume;
                    maxHealth = maxHealth + teeth[i].health;
                }
                for (int i = teeth.Length / 2; i < teeth.Length; i++)
                {
                    teeth[i] = new CreatureAdditions(spriteText, new Vector2(random.Next((int)mouth[1].spritePosition.X, (int)mouth[1].spritePosition.X + (int)mouth[1].spriteSize.X), mouth[1].spritePosition.Y + random.Next(-5, 0)), Color.Black, spriteSize, "teeth", linearVelocity, rotationVelocity);
                    teeth[i].origin = bodyParts[0].origin;
                    teeth[i].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - teeth[i].spritePosition.X, bodyParts[0].spritePosition.Y - teeth[i].spritePosition.Y);

                    totalSurfaceArea = totalSurfaceArea + teeth[i].totalSurfaceArea;
                    totalVolume = totalVolume + teeth[i].totalVolume;
                    maxHealth = maxHealth + teeth[i].health;
                }

            }

            if (random.Next(1, 100) == 3)
            {
                this.reproductiveDrive = random.Next(-20, 20) + this.reproductiveDrive;
            }

            if (random.Next(1, 100) == 4)
            {

                Color tempColour = bodyParts[0].spriteColour;
                int holdRandom = random.Next(-50, 50);

                if (tempColour.G + holdRandom >= 0 && tempColour.G + holdRandom <= 255 && random.Next(0, 3) == 1)
                {
                    tempColour.G = (byte)(tempColour.G + holdRandom);
                }

                if (tempColour.R + holdRandom >= 0 && tempColour.R + holdRandom <= 255 && random.Next(0, 3) == 2)
                {
                    tempColour.R = (byte)(tempColour.R + holdRandom);
                }

                if(tempColour.B + holdRandom >= 0 && tempColour.B + holdRandom <= 255 && random.Next(0, 3) == 3)
                {
                    tempColour.B = (byte)(tempColour.B + holdRandom);
                }

                foreach (CreatureAdditions a in bodyParts)
                {
                    a.spriteColour = tempColour;
                }
            }

            if (random.Next(1, 100) == 5)
            {
                this.eggSturdiness = eggSturdiness + random.Next(-5, 5);
            }

            if (random.Next(1, 100) == 6)
            {
                this.eggIncubation = eggIncubation + random.Next(-5, 5);
            }

            if (random.Next(1, 100) == 7)
            {
                this.aggressiveness = aggressiveness + random.Next(-1, 1);
            }

            if (random.Next(1, 100) == 8)
            {
                this.maturity = maturity + random.Next(-10, 10);
            }

            if(random.Next(1, 100) == 9)
            {
                neuralNet.Mutate(100, 0.1f);
            }

            if (random.Next(1, 100) == 10)
            {
                reproductiveDrive += random.Next(-5, 5);
            }

            if (random.Next(1, 100) == 11)
            {
                gutLength += random.Next(-5, 5);
            }

            if (random.Next(1, 100) == 12)
            {
                gutDiameter += random.Next(-5, 5);
            }

            gutArea = gutDiameter * gutLength;

            if (random.Next(1, 100) == 13)
            {
                digestionEfficiency += random.NextFloat(-1, 1);
            }

            if (random.Next(1, 100) == 14)
            {
                maturity += random.Next(-5, 5);
            }

            if (random.Next(1, 100) == 15)
            {
                gestationTime += random.Next(-5, 5);
            }

        }

        // Another essential part of evolution, it makes sure as many factors are copied over before everything is mutated
        public void Reproduce(Creatures parent1, Tile[,] tileArray, List<Creatures> listCreatures)
        {

            //public float individualHardiness { get; set; } // How hard the exterior of the plant is
            //private float[] temperatureRange = new float[2]; // The range in which the plant can live in; 0 - minimum, 1 - maximum

            parent1.energy = parent1.energy - parent1.eggNutrients * parent1.eggSturdiness;

            this.position = new Rectangle(parent1.position.X - parent1.position.Width, parent1.position.Y, parent1.position.Width, parent1.position.Height);
            this.spritePosition = new Vector2(this.position.X, this.position.Y);

            this.height = parent1.height;

            this.reproductiveType = parent1.reproductiveType;
            this.reproductiveDrive = parent1.reproductiveDrive;

            bodyPartsAmount = parent1.bodyPartsAmount;
            bodyParts = new CreatureAdditions[bodyPartsAmount];
            bodyParts[0] = new CreatureAdditions(spriteText, spritePosition, Color.DarkRed, spriteSize, "main", linearVelocity, rotationVelocity);
            bodyParts[0].origin = new Vector2(spritePosition.X + bodyParts[0].spriteSize.X / 2, spritePosition.Y + bodyParts[0].spriteSize.Y / 2);
            totalSurfaceArea = totalSurfaceArea + bodyParts[0].totalSurfaceArea;
            totalVolume = totalVolume + bodyParts[0].totalVolume;
            maxHealth = maxHealth + bodyParts[0].health;

            for (int i = 1; i < bodyParts.Length; i++)
            {
                bodyParts[i] = new CreatureAdditions(spriteText, new Vector2(bodyParts[0].spritePosition.X + random.Next(-10, 10), bodyParts[0].spritePosition.Y + random.Next(-10, 10)), spriteColour, spriteSize, "body", linearVelocity, rotationVelocity);
                bodyParts[i].origin = bodyParts[0].origin;
                bodyParts[i].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - bodyParts[i].spritePosition.X, bodyParts[0].spritePosition.Y - bodyParts[i].spritePosition.Y);

                totalSurfaceArea = totalSurfaceArea + bodyParts[i].totalSurfaceArea;
                totalVolume = totalVolume + bodyParts[i].totalVolume;
                maxHealth = maxHealth + bodyParts[i].health;
            }

            eyeAmount = parent1.eyeAmount;
            eyes = new CreatureAdditions[eyeAmount];
            for (int i = 0; i < eyes.Length; i++)
            {
                eyes[i] = new CreatureAdditions(spriteText, new Vector2(random.Next((int)bodyParts[0].spritePosition.X, (int)bodyParts[0].spritePosition.X + (int)bodyParts[0].spriteSize.X), random.Next((int)bodyParts[0].spritePosition.Y, (int)bodyParts[0].spritePosition.Y + (int)bodyParts[0].spriteSize.Y)), Color.White, spriteSize, "eyes", linearVelocity, rotationVelocity);
                eyes[i].origin = bodyParts[0].origin;
                eyes[i].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - eyes[i].spritePosition.X, bodyParts[0].spritePosition.Y - eyes[i].spritePosition.Y);

                totalSurfaceArea = totalSurfaceArea + eyes[i].totalSurfaceArea;
                totalVolume = totalVolume + eyes[i].totalVolume;
                maxHealth = maxHealth + eyes[i].health;
            }

            legAmount = random.Next(0, 4);
            speed = random.Next(0, 10) * legAmount;

            mouth = new CreatureAdditions[2];
            mouth[0] = new CreatureAdditions(spriteText, new Vector2(bodyParts[0].spritePosition.X + bodyParts[0].spriteSize.X, bodyParts[0].spritePosition.Y), Color.DarkSlateGray, spriteSize, "mouth", linearVelocity, rotationVelocity);
            mouth[0].origin = bodyParts[0].origin;
            mouth[0].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - mouth[0].spritePosition.X, bodyParts[0].spritePosition.Y - mouth[0].spritePosition.Y);
            totalSurfaceArea = totalSurfaceArea + mouth[0].totalSurfaceArea;
            totalVolume = totalVolume + mouth[0].totalVolume;
            maxHealth = maxHealth + mouth[0].health;

            mouth[1] = new CreatureAdditions(spriteText, new Vector2(bodyParts[0].spritePosition.X + bodyParts[0].spriteSize.X, bodyParts[0].spritePosition.Y + bodyParts[0].spriteSize.Y), Color.DarkSlateGray, spriteSize, "mouth", linearVelocity, rotationVelocity);
            mouth[1].origin = bodyParts[0].origin;
            mouth[1].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - mouth[1].spritePosition.X, bodyParts[0].spritePosition.Y - mouth[1].spritePosition.Y);
            totalSurfaceArea = totalSurfaceArea + mouth[1].totalSurfaceArea;
            totalVolume = totalVolume + mouth[1].totalVolume;
            maxHealth = maxHealth + mouth[1].health;

            teethAmount = parent1.teethAmount;
            teeth = new CreatureAdditions[teethAmount];
            for (int i = 0; i < teeth.Length / 2; i++)
            {
                teeth[i] = new CreatureAdditions(spriteText, new Vector2(random.Next((int)mouth[0].spritePosition.X, (int)mouth[0].spritePosition.X + (int)mouth[0].spriteSize.X), mouth[0].spritePosition.Y + random.Next(0, 5)), Color.Black, spriteSize, "teeth", linearVelocity, rotationVelocity);
                teeth[i].origin = bodyParts[0].origin;
                teeth[i].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - teeth[i].spritePosition.X, bodyParts[0].spritePosition.Y - teeth[i].spritePosition.Y);

                totalSurfaceArea = totalSurfaceArea + teeth[i].totalSurfaceArea;
                totalVolume = totalVolume + teeth[i].totalVolume;
                maxHealth = maxHealth + teeth[i].health;
            }
            for (int i = teeth.Length / 2; i < teeth.Length; i++)
            {
                teeth[i] = new CreatureAdditions(spriteText, new Vector2(random.Next((int)mouth[1].spritePosition.X, (int)mouth[1].spritePosition.X + (int)mouth[1].spriteSize.X), mouth[1].spritePosition.Y + random.Next(-5, 0)), Color.Black, spriteSize, "teeth", linearVelocity, rotationVelocity);
                teeth[i].origin = bodyParts[0].origin;
                teeth[i].distanceFromMain = new Vector2(bodyParts[0].spritePosition.X - teeth[i].spritePosition.X, bodyParts[0].spritePosition.Y - teeth[i].spritePosition.Y);

                totalSurfaceArea = totalSurfaceArea + teeth[i].totalSurfaceArea;
                totalVolume = totalVolume + teeth[i].totalVolume;
                maxHealth = maxHealth + teeth[i].health;
            }

            this.aggressiveness = parent1.aggressiveness;
            reproductiveDrive = parent1.reproductiveDrive;
            gutLength = parent1.gutLength;
            gutDiameter = parent1.gutDiameter;
            gutArea = gutDiameter * gutLength;
            digestionEfficiency = parent1.digestionEfficiency;
            maturity = parent1.maturity;
            gestationTime = parent1.gestationTime;

            this.matured = false;
            this.oldAge = false;

            this.eggIncubation = parent1.eggIncubation;
            this.age = 0;
            this.eggAge = 0;
            this.eggHatched = false;
            this.maturity = parent1.maturity;

            this.eggSturdiness = parent1.eggSturdiness;
            this.eggNutrients = parent1.eggNutrients;

            maxEnergy = this.maxHealth * ratioEnergyHealth;

            inputs = new float[eyeAmount + 1];
            this.neuralNet = parent1.neuralNet;

            Mutate();

        }
        
        // The energy cost associated with living. Older organisms use more energy
        public void BaseMetabolismCost()
        {
            if (oldAge == false)
            {
                energy = energy - 100 * age / maturity;
            }
            else
            {
                energy = energy - 1000 * age / maturity;
            }
        }

        public void Damage_Energy()
        {
            if (energy <= 0)
            {
                energy = 0;
                health = health - maxEnergy * age / maturity;
            }
        }

        public void OldAge()
        {
            if (age >= maturity * 2)
            {
                oldAge = true;
            }
        }

        // Decides how much of the stored nutrients can be converted into energy
        public void Digestion()
        {

            if (nutrients > 0)
            {
                energy = energy + (nutrients / gutArea);
            }

            if (energy > maxEnergy)
            {
                energy = maxEnergy;
            }
        }

        // Whenever creatures are able to "eat" food
        public void AbsorbNutrients(List<Food> listFood)
        {
            foreach(Food f in listFood)
            {
                if (bodyParts[0].position.Intersects(new Rectangle((int)f.spritePosition.X, (int)f.spritePosition.Y, (int)f.spriteSize.X, (int)f.spriteSize.Y)))
                {
                    nutrients = (nutrients + f.foodAmount);
                }
            }
        }

        public void AgeCreature()
        {
            if (eggHatched == true)
            {
                age = age + 1;
            }

            if (age >= maturity)
            {
                matured = true;
            }
        }

        public void GrowEgg()
        {
            if (eggHatched == false)
            {
                eggNutrients = eggNutrients - 1;
                eggAge = eggAge + 1;
            }

            if (eggAge >= eggIncubation)
            {
                eggHatched = true;
            }
        }

        public void Heal()
        {
            if (nutrients > 0 && energy >= maxEnergy / 4 && health < maxHealth)
            {
                health = health + 10;
                energy = energy - 1;
            }

            if (health >= maxHealth)
            {
                health = maxHealth;
            }
        }

        public void Bite(int threatAmount)
        {
            if(threatAmount > 10)
            {
                biting = true;
            }
            else
            {
                biting = false;
            }
        }

        public void Damage_Bite()
        {
            foreach(Creatures c in creatureThreatLevelID)
            {
                if(c.biting == true && position.Intersects(c.position))
                {
                    health -= c.teethAmount * 10;
                }
            }
        }

    }
}
