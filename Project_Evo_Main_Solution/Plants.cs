using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Project_Evo_Main_Solution
{
    public class Plants : Movable
    {
        /*
         * - Plants
         *      - Change hardiness
         *      - Change livable temperature
         *      - Reproduce
         *      - Health
         *      - Energy
         *      - Energy:Health
         *      - Surface Area
         *      - Volume
         *      - [Make leaves]
         */

        public float individualHardiness { get; set; } // How hard the exterior of the plant is
        private float feltTemperature; // What the temperature in the surrounding of the plant is
        private float[] temperatureRange = new float[2]; // The range in which the plant can live in; 0 - minimum, 1 - maximum
        public float maxHealth { get; set; } // The health amount the plant has which can be drained
        private float maxEnergy; // The energy amount the plant has which can be depleted
        public float health;
        private float energy;
        public float totalSurfaceArea { get; set; } // Total surface area of the plant
        public float totalVolume { get; set; } // Total volume of the plant
        private int leafAmount;
        public PlantAdditions[] leaves { get; set; }
        public PlantAdditions stem { get; set; }
        private int height;
        private float ratioEnergyHealth; // The amount, set by the user, by which energy is greater than health
        private int reproductionDistance; // The distance from the parent plant from which the offspring can spread up to
        private Random random = new Random();
        private int chemicalsDistance;
        private int chemicalsStrength;
        private Rectangle chemicalPosition;
        private Plants[] plantThreatLevelID;
        private int[] plantThreatLevelAmount;
        private int plantID;
        private bool releaseChemicals = false;
        private int aggressiveness;



        public int reproductiveDrive { get; private set; }
        private int nutrients;
        private Rectangle rootsArea;
        private int rootsdistance;
        private float photosynthesisEfficiency;
        private float seedSturdiness;
        private float seedNutrients;
        public bool overcrowded = false;
        public float age = 0;
        public float maturity;
        public bool oldAge = false;
        public float seedGerminationTime;
        public bool seedGerminated;
        private float seedAge = 0;

        public bool matured = false;
        public bool countedOnPile = false;

        public Plants(Texture2D inTexture, Vector2 inSpritePosition, Color inColour, Vector2 inSpriteSize, float inFeltTemperature, float inEnergyHealthRatio, int inPlantID, bool inSeedGerminated) : base(inTexture, inSpritePosition, inColour, inSpriteSize)
        {
            feltTemperature = inFeltTemperature; // Based on the map
            ratioEnergyHealth = inEnergyHealthRatio; // Based on the settings

            height = random.Next(1, 5); // Can mutate, is random at first, it's the stem's height
            leafAmount = random.Next(1, 3); // Can mutate, is random at first
            leaves = new PlantAdditions[leafAmount];
            reproductionDistance = random.Next(1, 50); // This is how many CELLS the plant can spread the seeds

            stem = new PlantAdditions(inTexture, inSpritePosition, inColour, inSpriteSize);
            this.totalSurfaceArea = this.totalSurfaceArea + stem.totalSurfaceArea;
            this.totalVolume = this.totalVolume + stem.totalVolume;
            this.maxHealth = this.maxHealth + stem.health;

            spriteSize = new Vector2(stem.width, stem.length);
            sourceRectangle = new Rectangle(position.X, position.Y, (int)spriteSize.X, (int)spriteSize.Y);

            for (int i = 0; i < leaves.Length; i++)
            {
                leaves[i] = new PlantAdditions(inTexture, new Vector2(position.X + random.Next(-5, 5), position.Y + random.Next(-5, 5)), Color.GreenYellow, new Vector2());

                this.totalSurfaceArea = leaves[i].totalSurfaceArea + this.totalSurfaceArea;
                this.totalVolume = leaves[i].totalVolume + this.totalVolume;
                this.maxHealth = this.maxHealth + leaves[i].health;
            }

            maxEnergy = this.maxHealth * ratioEnergyHealth;

            chemicalsDistance = random.Next(0, 50);
            chemicalsStrength = random.Next(0, 200);

            chemicalPosition = new Rectangle(position.X - chemicalsDistance, position.Y - chemicalsDistance, 2 * chemicalsDistance + position.Width, 2 * chemicalsDistance + position.Height);

            plantID = inPlantID;

            aggressiveness = random.Next(0, 3);

            reproductiveDrive = random.Next(-100, 100);

            rootsdistance = random.Next(1, 10);
            rootsArea = new Rectangle(position.X - rootsdistance, position.Y - rootsdistance, 2 * rootsdistance + position.Width, 2 * rootsdistance + position.Height);

            photosynthesisEfficiency = 0.65f;

            seedNutrients = random.NextFloat(0, maxEnergy);
            seedSturdiness = random.NextFloat(0, 2);


            seedGerminationTime = random.NextFloat(10, seedNutrients * 1.2f);

            seedGerminated = inSeedGerminated;

            maturity = random.NextFloat(10, 120);

            health = maxHealth;
            energy = maxEnergy;

            origin = new Vector2(spritePosition.X - spriteSize.X / 2, spritePosition.Y - spriteSize.Y / 2);

        }

        public void LearnIDs(List<Plants> listPlants)
        {
            plantThreatLevelID = listPlants.ToArray();

            plantThreatLevelAmount = new int[plantThreatLevelID.Length];
        }

        public void AssignThreat(int i)
        {
                if (plantThreatLevelID[i].position.Intersects(chemicalPosition))
                {
                    plantThreatLevelAmount[i] = 3 + aggressiveness;
                }

                if (this.position.Intersects(plantThreatLevelID[i].chemicalPosition) && plantThreatLevelID[i].releaseChemicals == true)
                {
                    plantThreatLevelAmount[i] = plantThreatLevelAmount[i] + aggressiveness;
                }

                if(this.overcrowded == true)
                {
                    plantThreatLevelAmount[i] = plantThreatLevelAmount[i] + aggressiveness;
                }

                else if(this.overcrowded == false)
                {
                    plantThreatLevelAmount[i] = plantThreatLevelAmount[i] - aggressiveness;
                }
        }

        // Essential for evolution, contains a lot of different variables that can be changed
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
                this.leafAmount = this.leafAmount + random.Next(-1, 1);

                if(this.leafAmount <= 0)
                {
                    this.leafAmount = 1;
                }

                this.leaves = new PlantAdditions[this.leafAmount];

                for (int i = 0; i < this.leaves.Length; i++)
                {
                    this.leaves[i] = new PlantAdditions(spriteText, new Vector2(position.X + random.Next(-5, 5), position.Y + random.Next(-5, 5)), Color.GreenYellow, new Vector2());

                    this.totalSurfaceArea = this.leaves[i].totalSurfaceArea + this.totalSurfaceArea;
                    this.totalVolume = this.leaves[i].totalVolume + this.totalVolume;
                    this.maxHealth = this.maxHealth + this.leaves[i].health;
                }
            }

            if(random.Next(1, 100) == 3)
            {
                this.reproductionDistance = this.reproductionDistance + random.Next(-5, 5);

                if(this.reproductionDistance <= 0)
                {
                    this.reproductionDistance = 1;
                }
            }

            if(random.Next(1, 100) == 4)
            {
                this.reproductiveDrive = random.Next(-20, 20) + this.reproductiveDrive;
            }

            if(random.Next(1, 100) == 5)
            {

                Color tempColour = leaves[0].spriteColour;
                int holdRandom = random.Next(-50, 50);
                
                if(tempColour.G + holdRandom >= 0 && tempColour.G + holdRandom <= 255)
                {
                    tempColour.G = (byte)(tempColour.G + holdRandom);
                }

                if (tempColour.R + holdRandom >= 0 && tempColour.R + holdRandom <= 255)
                {
                    tempColour.R = (byte)(tempColour.R + holdRandom);
                }

                foreach (PlantAdditions l in leaves)
                {
                    l.spriteColour = tempColour;
                }
            }

            if (random.Next(1, 100) == 6)
            {

                Color tempColour = stem.spriteColour;
                int holdRandom = random.Next(-50, 50);

                if (tempColour.G + holdRandom >= 0 && tempColour.G + holdRandom <= 255)
                {
                    tempColour.G = (byte)(tempColour.G + holdRandom);
                }

                if (tempColour.R + holdRandom >= 0 && tempColour.R + holdRandom <= 255)
                {
                    tempColour.R = (byte)(tempColour.R + holdRandom);
                }

                stem.spriteColour = tempColour;
            }

            if(random.Next(1, 100) == 7)
            {
                this.seedSturdiness = seedSturdiness + random.NextFloat(-5, 5);
            }

            if(random.Next(1, 100) == 8)
            {
                this.seedGerminationTime = seedGerminationTime + random.NextFloat(-5, 5);
            }

            if(random.Next(1, 100) == 9)
            {
                this.aggressiveness = aggressiveness + random.Next(-1, 1);
            }

            if(random.Next(1, 100) == 10)
            {
                this.chemicalsDistance = chemicalsDistance + random.Next(-1, 1);
                chemicalPosition = new Rectangle(position.X - chemicalsDistance, position.Y - chemicalsDistance, 2 * chemicalsDistance + position.Width, 2 * chemicalsDistance + position.Height);
            }

            if(random.Next(1, 100) == 11)
            {
                this.chemicalsStrength = chemicalsStrength + random.Next(-10, 10);
            }

            if(random.Next(1, 100) == 12)
            {
                this.rootsdistance = rootsdistance + random.Next(-2, 2);
                rootsArea = new Rectangle(position.X - rootsdistance, position.Y - rootsdistance, 2 * rootsdistance + position.Width, 2 * rootsdistance + position.Height);
            }

            if(random.Next(1, 100) == 13)
            {
                this.maturity = maturity + random.Next(-10, 10);
            }    

        }

        // Essential for evolution, copies over as many traits as possible from the parent
        public void Reproduce(Plants parent1, Tile[,] tileArray, List<Plants> plantList)
        {

        //public float individualHardiness { get; set; } // How hard the exterior of the plant is
        //private float[] temperatureRange = new float[2]; // The range in which the plant can live in; 0 - minimum, 1 - maximum

            parent1.energy = parent1.energy - parent1.seedNutrients * parent1.seedSturdiness;

            int offsetY = parent1.position.Y + random.Next(-reproductionDistance * 10, reproductionDistance * 10);
            int offsetX = parent1.position.X + random.Next(-reproductionDistance * 10, reproductionDistance * 10);
            Rectangle tempPosition = new Rectangle(offsetX, offsetY, parent1.position.Width, parent1.position.Height);

            foreach(Tile t in tileArray)
            {
                while(t.position.Intersects(tempPosition) && t.tileType == "stone")
                {
                    offsetX = parent1.position.X + random.Next(-reproductionDistance * 10, reproductionDistance * 10);
                    offsetY = parent1.position.Y + random.Next(-reproductionDistance * 10, reproductionDistance * 10);
                }
            }

            foreach(Plants p in plantList)
            {
                for(int i = 0; i < 3; i++)
                {
                    if(p.position.Intersects(tempPosition))
                    {
                        offsetX = offsetX + p.position.X;
                        offsetY = offsetY + p.position.Y;
                    }
                }

                if(parent1.plantID == p.plantID)
                {
                    for(int i = 0; i < plantThreatLevelID.Length; i++)
                    {
                        if(parent1.plantID == plantThreatLevelID[i].plantID)
                        {
                            plantThreatLevelAmount[i] = 1;
                        }
                    }
                }
            }

            this.position = new Rectangle(offsetX, offsetY, parent1.position.Width, parent1.position.Height);
            this.spritePosition = new Vector2(this.position.X, this.position.Y);

            this.height = parent1.height;

            this.reproductionDistance = parent1.reproductionDistance;
            this.reproductiveDrive = parent1.reproductiveDrive;

            stem = new PlantAdditions(spriteText, new Vector2(position.X + random.Next(-5, 5), position.Y + random.Next(-5, 5)), Color.Green, new Vector2());
            this.totalSurfaceArea = this.totalSurfaceArea + this.stem.totalSurfaceArea;
            this.totalVolume = this.totalVolume + this.stem.totalVolume;
            this.maxHealth = this.maxHealth + this.stem.health;

            this.leafAmount = parent1.leafAmount;
            this.leaves = new PlantAdditions[this.leafAmount];

            for (int i = 0; i < this.leaves.Length; i++)
            {
                this.leaves[i] = new PlantAdditions(spriteText, new Vector2((float)(position.X + (parent1.leaves[i]?.position.X - parent1.position.X)), (float)(position.Y + (parent1.leaves[i]?.position.Y - parent1.position.Y))), Color.GreenYellow, new Vector2());

                this.totalSurfaceArea = this.leaves[i].totalSurfaceArea + this.totalSurfaceArea;
                this.totalVolume = this.leaves[i].totalVolume + this.totalVolume;
                this.maxHealth = this.maxHealth + this.leaves[i].health;
            }

            this.chemicalsDistance = parent1.chemicalsDistance;
            this.chemicalsStrength = parent1.chemicalsStrength;
            this.chemicalPosition = new Rectangle(position.X - chemicalsDistance, position.Y - chemicalsDistance, 2 * chemicalsDistance + position.Width, 2 * chemicalsDistance + position.Height);

            this.aggressiveness = parent1.aggressiveness;

            this.rootsdistance = parent1.rootsdistance;
            this.rootsArea = new Rectangle(position.X - rootsdistance, position.Y - rootsdistance, 2 * rootsdistance + position.Width, 2 * rootsdistance + position.Height);

            this.matured = false;
            this.countedOnPile = false;
            this.releaseChemicals = false;
            this.oldAge = false;
            this.overcrowded = false;

            this.seedGerminationTime = parent1.seedGerminationTime;
            this.age = 0;
            this.seedAge = 0;
            this.seedGerminated = true;
            this.maturity = parent1.maturity;

            this.seedSturdiness = parent1.seedSturdiness;
            this.seedNutrients = parent1.seedNutrients;

            maxEnergy = this.maxHealth * ratioEnergyHealth;

            Mutate();

        }

        // The energy cost for living. Older individuals need more energy
        public void BaseMetabolismCost()
        {
            if (oldAge == false)
            {
                energy = energy - age / maturity;
            }
            else
            {
                energy = energy - 100 * age / maturity;
            }
        }

        // A form of competition: it checks if there's any plants within its chemical distance Rectangle
        // If it's aggressive enough towards them, it will release chemicals to kill off competition
        // This CAN include offspring!
        public bool ReleaseChemicals(int i)
        {

            releaseChemicals = false;

                if (plantThreatLevelAmount[i] > 5)
                {
                    if (plantThreatLevelID[i].position.Intersects(chemicalPosition))
                    {
                        releaseChemicals = true;
                        energy = energy - chemicalsStrength;
                    }
                }

                else
                {
                    releaseChemicals = false;
                }

            return releaseChemicals;

        }

        // Damage taken from chemicals released by other plants
        public void Damage_Chemicals(int i)
        {
            if (plantThreatLevelID[i].chemicalPosition.Intersects(this.position) && plantThreatLevelID[i].releaseChemicals == true)
            {
                this.health = this.health - plantThreatLevelID[i].chemicalsStrength;
            }
        }

        public void Damage_Energy()
        {
            if(energy <= 0)
            {
                energy = 0;
                health = health - maxEnergy * age / maturity;
            }
        }

        public void OldAge()
        {
            if(age >= maturity * 2)
            {
                oldAge = true;
            }
        }

        // The amount of energy produced every action, based on how many leaves there are and their colour
        public void Photosynthesis()
        {

            if(nutrients > 0)
            {
                foreach(PlantAdditions l in leaves)
                {
                    energy = energy + l.totalSurfaceArea * photosynthesisEfficiency * l.spriteColour.G / 255;

                    nutrients = nutrients - (int)l.totalSurfaceArea;
                }

                energy = energy + stem.totalSurfaceArea * photosynthesisEfficiency * stem.spriteColour.G / 255;

                nutrients = nutrients - (int)stem.totalSurfaceArea;
            }

            if(energy > maxEnergy)
            {
                energy = maxEnergy;
            }
        }

        // Absorbs nutrients from the tile map made with perlin noise
        public void AbsorbNutrients(Tile[,] tileArray)
        {
            foreach(Tile t in tileArray)
            {
                if(rootsArea.Intersects(t.position))
                {
                    t.nutrientAmount = t.nutrientAmount - rootsArea.Width * rootsArea.Height;
                    nutrients = nutrients + rootsArea.Width * rootsArea.Height;
                }
            }
        }

        public void AgePlant()
        {
            if (seedGerminated == true)
            {
                age = age + 1;
            }

            if(age >= maturity)
            {
                matured = true;
            }
        }

        public void GrowSeed()
        {
            if(seedGerminated == false)
            {
                seedNutrients = seedNutrients - 1;
                seedAge = seedAge + 1;
            }

            if(seedAge >= seedGerminationTime)
            {
                seedGerminated = true;
            }
        }

        public void Heal()
        {
            if(nutrients > 0 && energy >= maxEnergy / 4 && health < maxHealth)
            {
                health = health + 10;
                energy = energy - 1;
            }

            if(health >= maxHealth)
            {
                health = maxHealth;
            }
        }

        // A single method for all (but one) plant action to make it less demanding on the hardware
        public void PlantActions(Tile[,] tileArray)
        {
            Heal();

            GrowSeed();

            AgePlant();

            OldAge();

            BaseMetabolismCost();

            for (int i = 0; i < plantThreatLevelID.Length; i++)
            {
                AssignThreat(i);

                ReleaseChemicals(i);

                Damage_Chemicals(i);
            }

            Damage_Energy();

            AbsorbNutrients(tileArray);

            Photosynthesis();
        }

    }
}
