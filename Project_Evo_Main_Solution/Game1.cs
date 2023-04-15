using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;

namespace Project_Evo_Main_Solution
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Text Box variables
        private SpriteFont font_def;
        private string inputString = "";
        private Rectangle textBoxRectangle;
        private Texture2D textBoxTexture;
        private MouseManager mouseManager = new MouseManager();
        private TextWriter textWriter = new TextWriter();
        private bool allowText = false;
        private bool showTyping = true;
        private Random randomNumber = new Random();
        private bool allowKeyboardTap = true;

        // Tile Map variables
        private FileManager fileManager = new FileManager();
        private TileMap tileMap = new TileMap();
        private Tile[,] tileArray;
        private float[][] map;
        const int NUMBER_OF_TILES = 50;
        private int SIZE_OF_CELL = 50;

        // Camera variables
        public static int screenWidth;
        public static int screenHeight;
        public Camera _camera;
        public Player _player;
        public bool allowPlayerMovement = false;
        public bool zoomed = true;
        public float zoomLevel = 1;

        // Plant and Animals variables
        public Creatures creatures;
        public Plants plants;
        public List<Creatures> listCreatures = new List<Creatures>();
        public List<Plants> listPlants = new List<Plants>();
        public int STARTING_CREATURE_NUMBER = 100;
        public int STARTING_PLANT_NUMBER = 100;
        public NNMaker[][] neuralNet;

        public List<Food> listFood = new List<Food>();

        public float timer = 0;

        private int plantsIDs = 0;
        private int creaturesIDs = 0;

        private bool plantIDsChanged = true;
        private bool creaturesIDsChanged = true;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.PreferredBackBufferWidth = 1000;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            screenHeight = _graphics.PreferredBackBufferHeight;
            screenWidth = _graphics.PreferredBackBufferWidth;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Method where content is loaded to use later

            _camera = new Camera();

            textBoxTexture = Content.Load<Texture2D>("textRectangle");
            textBoxRectangle = new Rectangle(Window.ClientBounds.Width / 2 - 150 / 2, Window.ClientBounds.Height / 2 - 25 / 2, 150, 25);

            font_def = Content.Load<SpriteFont>("default_text");

            _player = new Player(textBoxTexture);

            textWriter = new TextWriter(textBoxRectangle);

            tileArray = new Tile[NUMBER_OF_TILES, NUMBER_OF_TILES];

            map = new float[NUMBER_OF_TILES][];
            for (int i = 0; i < NUMBER_OF_TILES; i++)
            {
                map[i] = new float[NUMBER_OF_TILES];
            }

            tileMap = new TileMap(map);

            neuralNet = new NNMaker[STARTING_CREATURE_NUMBER][];
            for(int i = 0; i < STARTING_CREATURE_NUMBER; i++)
            {
                neuralNet[i] = new NNMaker[3];
            }

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            // Delta variable used as a timer for actions later in the code
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            timer += delta;

            // Camera constantly follows player (a.k.a. where the user moves the camera itself)
            _camera.Follow(_player, zoomLevel);

            // This if statement checks whether the user is in the main menu, by showing the menu buttons/text boxes. User cannot move the camera
            if (showTyping == true)
            {
                allowPlayerMovement = false;
                _player.spritePosition = new Vector2(Window.ClientBounds.Width / 2 - _player.spriteText.Width / 2, Window.ClientBounds.Height / 2 - _player.spriteText.Height / 2);

                if (mouseManager.CheckIfClicked(textBoxRectangle) == true)
                {
                    allowText = true;
                }
                else
                {
                    allowText = false;
                }
            }


            // If the Main Menu isn't being shown, begin the actual simulation
            else
            {
                allowText = false;

                // This makes it so the user can return to the menu or reset the simulation
                if (Keyboard.GetState().IsKeyDown(Keys.Back))
                {
                    showTyping = true;
                    allowKeyboardTap = true;
                }

                // By pressing - or +, it allows the user to zoom in and out
                // The booleans make it so it only zooms "once at a time"
                if(Keyboard.GetState().IsKeyDown(Keys.OemMinus) && zoomed == false)
                {
                    zoomed = true;
                    zoomLevel = zoomLevel * 0.5f;
                }
                else if(Keyboard.GetState().IsKeyDown(Keys.OemPlus) && zoomed == false)
                {
                    zoomed = true;
                    zoomLevel = zoomLevel * 2;
                }
                else if(Keyboard.GetState().IsKeyUp(Keys.OemPlus) && Keyboard.GetState().IsKeyUp(Keys.OemMinus))
                {
                    zoomed = false;
                }

                // Every 2 seconds (roughly), each organism is able to do an action
                if(timer % 2 >= 0 && timer % 2 <= 0.02)
                {

                    // This deals with decomposition of food items (plant or meat)
                    Food[] arrayAfterDecomposition = new Food[listFood.Count];

                    if(listFood != null)
                    {
                        foreach(Food f in listFood)
                        {
                            // For every food item that exists, it decomposes
                            f?.Decompose(tileArray);

                            if (f?.foodAmount > 0)
                            {
                                // If it has no more food/fully decomposed, do not add it to the temporary array
                                arrayAfterDecomposition[listFood.IndexOf(f)] = f;
                            }
                        }
                    }

                    listFood.Clear();

                    // Add every item in the array to the list
                    foreach(Food f in arrayAfterDecomposition)
                    {
                        listFood.Add(f);
                    }

                    // This ensures that new, unique plants spawn whenever there are fewer than the minimum starting number; this way, the simulation can continue until a species makes a foothold eventually
                    if(listPlants.Count < STARTING_PLANT_NUMBER)
                    {
                        int x = randomNumber.Next(0, NUMBER_OF_TILES * SIZE_OF_CELL);
                        int y = randomNumber.Next(0, NUMBER_OF_TILES * SIZE_OF_CELL);
                        Rectangle tempPlace = new Rectangle(x, y, SIZE_OF_CELL, SIZE_OF_CELL);
                        bool placed = false;

                        // The boolean makes sure no more than 1 plant is placed down at a time
                        foreach (Tile t in tileArray)
                        {
                            if (tempPlace.Intersects(t.position) && placed == false)
                            {
                                // Can only place on non-rock surfaces
                                if (t.tileType != "Rock")
                                {
                                    plants = new Plants(textBoxTexture, new Vector2(x, y), Color.Green, new Vector2(10, 10), 20, 2, plantsIDs, true);
                                    listPlants.Add(plants);
                                    placed = true;

                                    plantsIDs++;

                                    plantIDsChanged = true;
                                }
                            }
                        }
                    }

                    // Exactly the same as the plants, except with creatures
                    if (listCreatures.Count < STARTING_CREATURE_NUMBER)
                    {
                        int x = randomNumber.Next(0, NUMBER_OF_TILES * SIZE_OF_CELL);
                        int y = randomNumber.Next(0, NUMBER_OF_TILES * SIZE_OF_CELL);
                        Rectangle tempPlace = new Rectangle(x, y, SIZE_OF_CELL, SIZE_OF_CELL);
                        bool placed = false;

                        foreach (Tile t in tileArray)
                        {
                            if (tempPlace.Intersects(t.position) && placed == false)
                            {
                                if (t.tileType != "Rock")
                                {
                                    creatures = new Creatures(textBoxTexture, new Vector2(x, y), Color.Purple, new Vector2(10, 10), 20, 2, plantsIDs, true);
                                    listCreatures.Add(creatures);
                                    placed = true;

                                    creaturesIDs++;

                                    creaturesIDsChanged = true;
                                }
                            }
                        }
                    }

                    // If there are new plants, their IDs are learned again by the plants; helps in competition
                    if (plantIDsChanged == true)
                    {
                        foreach (Plants p in listPlants)
                        {
                            p.LearnIDs(listPlants);
                        }

                        plantIDsChanged = false;
                    }

                    // Plant Death
                    Plants[] arrayBeforeDeaths = new Plants[listPlants.Count];
                    arrayBeforeDeaths = listPlants.ToArray();
                    Plants[] arrayAfterDeaths = new Plants[listPlants.Count];

                    bool plantsHaveDied = false;

                    // Each plant has a number of actions they can do. If their health reaches 0 or below, they become dead
                    foreach (Plants p in listPlants)
                    {
                        p.PlantActions(tileArray);

                        if (p.health <= 0)
                        {
                            plantsHaveDied = true;
                        }


                        // Overcrowding increases aggression, hence why it has to be changed
                        for (int i = 0; i < tileArray.GetUpperBound(0); i++)
                        {
                            for (int j = 0; j < tileArray.GetUpperBound(1); j++)
                            {
                                if (p.position.Intersects(tileArray[i, j].position) && p.countedOnPile == false)
                                {
                                    tileArray[i, j].plantAmount++;
                                    p.countedOnPile = true;
                                }

                                if (tileArray[i, j].plantAmount >= 5)
                                {
                                    p.overcrowded = true;
                                }
                            }
                        }
                    }

                    // If plants have been tagged as one or more dying, two arrays are made
                    // The beforeDeath array holds every single plant
                    // The afterDeath array holds only plants that are still ALIVE
                    // Food is made on the spot where the plants died
                    if (plantsHaveDied == true)
                    {
                        plantIDsChanged = true;

                        arrayBeforeDeaths = listPlants.ToArray();

                        for (int i = 0; i < arrayBeforeDeaths.Length; i++)
                        {
                            if (arrayBeforeDeaths[i].health >= 0)
                            {
                                arrayAfterDeaths[i] = arrayBeforeDeaths[i];
                            }
                            else
                            {
                                foreach (PlantAdditions p in arrayBeforeDeaths[i].leaves)
                                {
                                    if (p.health > 0)
                                    {
                                        listFood.Add(new Food(p, textBoxTexture, p.spritePosition, Color.Gold, p.spriteSize));
                                    }
                                }

                                if (arrayBeforeDeaths[i].stem.health > 0)
                                {
                                    listFood.Add(new Food(arrayBeforeDeaths[i].stem, textBoxTexture, arrayBeforeDeaths[i].stem.spritePosition, Color.Gold, arrayBeforeDeaths[i].stem.spriteSize));
                                }
                            }
                        }

                        listPlants.Clear();

                        // All of the living plants are added to the list after it is emptied so there are no duplicates
                        foreach (Plants p in arrayAfterDeaths)
                        {
                            if (p != null)
                            {
                                listPlants.Add(p);
                            }
                        }

                        plantsHaveDied = false;
                    }


                    Plants[] tempArrayPlants = new Plants[listPlants.Count];
                    tempArrayPlants = listPlants.ToArray();
                    listPlants.Clear();


                    // Plant Reproduction
                    // Each plant can only make 1 other plant at a time, hence the number of plants after reproducing being the same as the number of plants currently existing
                    // The random number is there to ensure it doesn't happen every 2 seconds and lead to an overflow
                    // Plants that are more likely to reproduce have a higher chance of producing offspring
                    int length = tempArrayPlants.Length;
                    Plants[] newPlantsArray = new Plants[length];
                    for (int i = 0; i < length; i++)
                    {
                        foreach (Tile t in tileArray)
                        {
                            if (t.position.Intersects(tempArrayPlants[i].position) && tempArrayPlants[i].overcrowded == false)
                            {
                                if (randomNumber.Next(0, 1000) + tempArrayPlants[i].reproductiveDrive >= 800 && tempArrayPlants[i].matured == true)
                                {
                                    plantIDsChanged = true;

                                    plantsIDs++;

                                    newPlantsArray[i] = new Plants(textBoxTexture, new Vector2(), Color.Green, new Vector2(), 20, 2, plantsIDs, false);

                                    newPlantsArray[i].Reproduce(tempArrayPlants[i], tileArray, listPlants);
                                }
                            }
                        }
                    }

                    foreach(Plants p in tempArrayPlants)
                    {
                        if (p != null)
                        {
                            listPlants.Add(p);
                        }
                    }

                    foreach(Plants p in newPlantsArray)
                    {
                        if (p != null)
                        {
                            listPlants.Add(p);
                        }
                    }
                    // Both arrays are added to the list after being cleared


                    // Creatures
                    Creatures[] arrayBeforeDeathsC = new Creatures[listCreatures.Count];
                    arrayBeforeDeaths = listPlants.ToArray();
                    Creatures[] arrayAfterDeathsC = new Creatures[listCreatures.Count];

                    bool creaturesHaveDied = false;

                    // First, is death and other activities
                    // Similar to Plants
                    foreach(Creatures c in listCreatures)
                    {
                        c.DecideInputs(listPlants.ToArray(), listCreatures.ToArray(), tileArray);

                        c.NeuralNet(tileArray, listFood);

                        c.LearnIDs(listCreatures);

                        c.AgeCreature();

                        c.GrowEgg();

                        for (int i = 0; i < listCreatures.Count; i++)
                        {
                            c.AssignThreat(i);
                        }

                        c.BaseMetabolismCost();

                        c.Damage_Bite();

                        c.Damage_Energy();

                        if(c.health <= 0)
                        {
                            creaturesHaveDied = true;
                        }

                    }

                    // Then, it creates a separate array to edit, empties the list, kills off any creatures with health bellow 0, and regroups the array and the list
                    if (creaturesHaveDied == true)
                    {
                        creaturesIDsChanged = true;

                        arrayBeforeDeathsC = listCreatures.ToArray();

                        for (int i = 0; i < arrayBeforeDeathsC.Length; i++)
                        {
                            if (arrayBeforeDeathsC[i].health >= 0)
                            {
                                arrayAfterDeathsC[i] = arrayBeforeDeathsC[i];
                            }
                            else
                            {
                                foreach (CreatureAdditions p in arrayBeforeDeathsC[i].bodyParts)
                                {
                                    if (p.health > 0)
                                    {
                                        listFood.Add(new Food(p, textBoxTexture, p.spritePosition, Color.IndianRed, p.spriteSize));
                                    }
                                }
                            }
                        }

                        listCreatures.Clear();

                        foreach (Creatures p in arrayAfterDeathsC)
                        {
                            if (p != null)
                            {
                                listCreatures.Add(p);
                            }
                        }

                        creaturesHaveDied = false;
                    }

                    // Creature Reproduction - makes arrays to hold the old and new creatures, mutates the new ones based on the parent, and then groups the two in the main list
                    Creatures[] tempArrayCreatures = new Creatures[listCreatures.Count];
                    tempArrayCreatures = listCreatures.ToArray();
                    listCreatures.Clear();

                    int lengthC = tempArrayCreatures.Length;
                    Creatures[] newCreaturesArray = new Creatures[lengthC];
                    for (int i = 0; i < lengthC; i++)
                    {
                        if (tempArrayCreatures[i].allowReproduction == true && tempArrayCreatures[i].matured == true && randomNumber.Next(1, 1000) + tempArrayCreatures[i].reproductiveDrive >= 880)
                        {
                            creaturesIDsChanged = true;

                            creaturesIDs++;

                            newCreaturesArray[i] = new Creatures(textBoxTexture, new Vector2(), Color.Purple, new Vector2(), 20, 2, creaturesIDs, false);

                            newCreaturesArray[i].Reproduce(tempArrayCreatures[i], tileArray, listCreatures);
                        }
                    }

                    foreach (Creatures p in tempArrayCreatures)
                    {
                        if (p != null)
                        {
                            listCreatures.Add(p);
                        }
                    }

                    foreach (Creatures p in newCreaturesArray)
                    {
                        if (p != null)
                        {
                            listCreatures.Add(p);
                        }
                    }

                }
            }

            // While in the menu, if you press Enter, with or without a seed, it creates a perlin noise map
            // This is then used for the actual map the organisms live in
            // No seed leads to random seed
            // Each map is saved on a file if the user wants to load it as a save
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && allowKeyboardTap == true)
            {
                allowKeyboardTap = false;

                showTyping = false;

                allowPlayerMovement = true;

                if (textWriter.GetInputtedString() != "")
                {
                    fileManager.WriteToFile(NUMBER_OF_TILES, int.Parse(textWriter.GetInputtedString()));
                }

                else
                {
                    fileManager.WriteToFile(NUMBER_OF_TILES, randomNumber.Next(0, 99999999));
                }

                float[,] temp = fileManager.ReadFile(NUMBER_OF_TILES);

                for (int i = 0; i < NUMBER_OF_TILES; i++)
                {
                    for (int j = 0; j < NUMBER_OF_TILES; j++)
                    {
                        map[i][j] = temp[i, j];
                    }
                }
                tileMap = new TileMap(map);

                tileArray = tileMap.CreateMap(SIZE_OF_CELL, NUMBER_OF_TILES, textBoxTexture);

                listCreatures.Clear();

                // These are the initial spawns of the creatures, which happen whenever the user starts the simulation
                for (int i = 0; i < STARTING_CREATURE_NUMBER; i++)
                {
                    int x = randomNumber.Next(0, NUMBER_OF_TILES * SIZE_OF_CELL);
                    int y = randomNumber.Next(0, NUMBER_OF_TILES * SIZE_OF_CELL);
                    Rectangle tempPlace = new Rectangle(x, y, SIZE_OF_CELL, SIZE_OF_CELL);
                    bool placed = false;

                    foreach (Tile t in tileArray)
                    {
                        if (tempPlace.Intersects(t.position) && placed == false)
                        {
                            if (t.tileType != "Rock")
                            {
                                creatures = new Creatures(textBoxTexture, new Vector2(x, y), Color.Purple, new Vector2(10, 10), 20, 2, creaturesIDs, false);
                                listCreatures.Add(creatures);
                                placed = true;

                                creaturesIDs++;
                            }
                        }
                    }
                }

                listPlants.Clear();

                for (int i = 0; i < STARTING_PLANT_NUMBER; i++)
                {
                    int x = randomNumber.Next(0, NUMBER_OF_TILES * SIZE_OF_CELL);
                    int y = randomNumber.Next(0, NUMBER_OF_TILES * SIZE_OF_CELL);
                    Rectangle tempPlace = new Rectangle(x, y, SIZE_OF_CELL, SIZE_OF_CELL);
                    bool placed = false;

                    foreach (Tile t in tileArray)
                    {
                        if (tempPlace.Intersects(t.position) && placed == false)
                        {
                            if (t.tileType != "Rock")
                            {
                                plants = new Plants(textBoxTexture, new Vector2(x, y), Color.Green, new Vector2(10, 10), 20, 2, plantsIDs, true);
                                listPlants.Add(plants);
                                placed = true;

                                plantsIDs++;
                            }
                        }
                    }
                }

                textWriter.SetInputtedString("");
                textWriter.SetCharacterArray(new string[8]);

            }

            if (allowPlayerMovement == true)
            {
                _player.MovePlayer();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            // TODO: Add your drawing code here

            float timer = (float)gameTime.TotalGameTime.TotalSeconds;

            _spriteBatch.Begin(transformMatrix: _camera.transformMatrix);

            _player.Draw(_spriteBatch, 0);

            if (showTyping == true)
            {
                _spriteBatch.Draw(textBoxTexture, textBoxRectangle, Color.Gray);

                if (allowText == true)
                {
                    textWriter.WriteText(_spriteBatch, font_def, inputString);
                }

                _spriteBatch.DrawString(font_def, "Instructions:", new Vector2(10, Window.ClientBounds.Height / 4 * 3), Color.Black);
                _spriteBatch.DrawString(font_def, "W A S D - Move Camera", new Vector2(10, Window.ClientBounds.Height / 4 * 3 + 15), Color.Black);
                _spriteBatch.DrawString(font_def, "-/+ - Zoom Out/In", new Vector2(10, Window.ClientBounds.Height / 4 * 3 + 30), Color.Black);
                _spriteBatch.DrawString(font_def, "Click on the box to enter a seed or leave empty for a random seed.", new Vector2(10, Window.ClientBounds.Height / 4 * 3 + 45), Color.Black);
                _spriteBatch.DrawString(font_def, "Press ENTER to start the simulation and enjoy!", new Vector2(10, Window.ClientBounds.Height / 4 * 3 + 60), Color.Black);
                _spriteBatch.DrawString(font_def, "Alternatively, click BACKSPACE to come back to this menu.", new Vector2(10, Window.ClientBounds.Height / 4 * 3 + 75), Color.Black);

            }

            else
            {
                foreach(Tile t in tileArray)
                {
                    t?.Draw(_spriteBatch);
                }

                if(listFood != null)
                {
                    foreach(Food f in listFood)
                    {
                        if (f?.foodAmount > 0 && f?.totalFood > 0)
                        {
                            f.Draw(_spriteBatch, f.foodAmount / f.totalFood);
                        }
                    }
                }

                foreach(Creatures c in listCreatures)
                {
                    foreach(CreatureAdditions b in c.bodyParts)
                    {
                        b.Draw(_spriteBatch, c.age / c.maturity);
                    }

                    foreach (CreatureAdditions m in c.mouth)
                    {
                        m.Draw(_spriteBatch, c.age / c.maturity);
                    }

                    foreach (CreatureAdditions t in c.teeth)
                    {
                        t.Draw(_spriteBatch, c.age / c.maturity);
                    }

                    foreach (CreatureAdditions e in c.eyes)
                    {
                        e.Draw(_spriteBatch, c.age / c.maturity);
                    }
                }

                foreach(Plants m in listPlants)
                {
                    m.stem.Draw(_spriteBatch, m.age / m.maturity);

                    for(int i = 0; i < m.leaves.Length; i++)
                    {
                        m.leaves[i]?.Draw(_spriteBatch, m.age / m.maturity);
                    }
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}