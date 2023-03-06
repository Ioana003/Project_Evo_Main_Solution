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
        public Movable creatures;
        public Plants plants;
        public List<Movable> listCreatures = new List<Movable>();
        public List<Plants> listPlants = new List<Plants>();
        public int STARTING_CREATURE_NUMBER = 100;
        public int STARTING_PLANT_NUMBER = 100;
        public NNMaker[][] neuralNet;

        public float timer = 0;

        private int plantsIDs = 0;

        private bool plantIDsChanged = true;

        /*
         * List of what works:
         * - Camera
         * - Map Creation (Randomised)
         * - Text Box position
         * - Clicking on the text box
         * - Text displays properly
         * - Camera zoom in and out
         * - Player/Camera only moves whenever you are zoomed into the map
         * 
         * To do:
         * - Plants
         * - Animals
         *      - Move forward-back
         *      - Move left-right
         *      - Rotate
         *      - See
         *      - Reproduce
         *      - Eat
         *      - [More advanced behaviours]
         * - Weather
         *      - Temperature Map using Perlin Noise
         *      - Moisture Map using Perlin Noise
         *      - Wind direction
         *      - Wind strength
         * - Options
         * 
         */

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

            /* There are 3 networks each creatures will automatically start with:
             * 1. Move Forward-Back: negative values move back, positive values move forward
             * 2. Move Left-Right: negative values move left, positive values move right
             * 3. WantToReproduce: positive values increase chance to multiply, negative values decrease chance to multiply
             */

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            timer += delta;

            _camera.Follow(_player, zoomLevel);

            // For the purposes of Debugging
            if(Keyboard.GetState().IsKeyDown(Keys.LeftControl) && Keyboard.GetState().IsKeyDown(Keys.B))
            {
                var debug = 1;
            }

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


            // WHERE SIMULATION CODE STARTS
            else
            {
                allowText = false;

                if (Keyboard.GetState().IsKeyDown(Keys.Back))
                {
                    showTyping = true;
                    allowKeyboardTap = true;
                }

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

                if(timer % 2 >= 0 && timer % 2 <= 0.05)
                {

                    if(listPlants.Count < STARTING_PLANT_NUMBER)
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

                                    plantIDsChanged = true;
                                }
                            }
                        }
                    }

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

                    foreach (Plants p in listPlants)
                    {
                        p.PlantActions(tileArray);

                        if (p.health <= 0)
                        {
                            plantsHaveDied = true;
                        }



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
                                foreach(Tile t in tileArray)
                                {
                                    if (t.position.Intersects(arrayBeforeDeaths[i].position))
                                    {
                                        t.nutrientAmount = t.nutrientAmount + (int)arrayBeforeDeaths[i].totalVolume;
                                        t.plantAmount--;
                                    }
                                }
                            }
                        }

                        listPlants.Clear();

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
                    int length = tempArrayPlants.Length;
                    Plants[] newPlantsArray = new Plants[length];
                    for (int i = 0; i < length; i++)
                    {
                        foreach (Tile t in tileArray)
                        {
                            if (t.position.Intersects(tempArrayPlants[i].position) && tempArrayPlants[i].overcrowded == false)
                            {
                                if (randomNumber.Next(0, 1000) + tempArrayPlants[i].reproductiveDrive >= 900 && tempArrayPlants[i].matured == true)
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
                }
            }

            // If you press enter, it creates a perlin noise map and creates how it looks
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

                for (int i = 0; i < STARTING_CREATURE_NUMBER; i++)
                {
                    int x = randomNumber.Next(0, NUMBER_OF_TILES * SIZE_OF_CELL);
                    int y = randomNumber.Next(0, NUMBER_OF_TILES * SIZE_OF_CELL);
                    Rectangle tempPlace = new Rectangle(x, y, SIZE_OF_CELL, SIZE_OF_CELL);

                    foreach (Tile t in tileArray)
                    {
                        if (tempPlace.Intersects(t.position))
                        {
                            if (t.tileType != "Rock")
                            {
                                creatures = new Movable(textBoxTexture, new Vector2(x, y), Color.Purple, new Vector2(10, 10));
                                listCreatures.Add(creatures);
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
            }

            else
            {
                foreach(Tile t in tileArray)
                {
                    t?.Draw(_spriteBatch);
                }

                foreach(Movable m in listCreatures)
                {
                    m.Draw(_spriteBatch, 1);
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