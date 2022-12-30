using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

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
        const int NUMBER_OF_TILES = 100;
        private int SIZE_OF_CELL = 50;

        // Camera variables
        public static int screenWidth;
        public static int screenHeight;
        public Camera _camera;
        public Player _player;
        public bool allowPlayerMovement = false;
        public bool zoomedIn = true;

        // Plant and Animals variables
        public Movable creatures;
        public Movable plants;
        public List<Movable> listCreatures = new List<Movable>();
        public List<Movable> listPlants = new List<Movable>();
        public int STARTING_CREATURE_NUMBER = 100;
        public int STARTING_PLANT_NUMBER = 100;
        public NNMaker[][] neuralNet;

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

            // This is the text box that lets you input your seed

            _camera.Follow(_player);

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

            else
            {
                allowText = false;

                if (Keyboard.GetState().IsKeyDown(Keys.Back))
                {
                    showTyping = true;
                    allowKeyboardTap = true;
                }

                if(Keyboard.GetState().IsKeyDown(Keys.OemMinus) && zoomedIn == true)
                {
                    zoomedIn = false;
                    SIZE_OF_CELL = 10;
                    _player.spritePosition = new Vector2(Window.ClientBounds.Width / 2 - _player.spriteText.Width / 2, Window.ClientBounds.Height / 2 - _player.spriteText.Height / 2);
                    tileArray = tileMap.CreateMap(SIZE_OF_CELL, NUMBER_OF_TILES, textBoxTexture);
                    allowPlayerMovement = false;

                    List<Movable> tempList = new List<Movable>();
                    int size = 5;

                    foreach(Movable m in listCreatures)
                    {
                        tempList.Add(new Movable(new Rectangle((int)m.spritePosition.X / size, (int)m.spritePosition.Y / size, 2, 2), m.texture, new Vector2(m.spritePosition.X / size, m.spritePosition.Y / size), m.spriteColour));
                    }
                    listCreatures.Clear();
                    foreach(Movable m in tempList)
                    {
                        listCreatures.Add(m);
                    }
                    tempList.Clear();

                    foreach (Movable m in listPlants)
                    {
                        tempList.Add(new Movable(new Rectangle((int)m.spritePosition.X / size, (int)m.spritePosition.Y / size, 2, 2), m.texture, new Vector2(m.spritePosition.X / size, m.spritePosition.Y / size), m.spriteColour));
                    }
                    listPlants.Clear();
                    foreach (Movable m in tempList)
                    {
                        listPlants.Add(m);
                    }
                    tempList.Clear();
                }
                else if(Keyboard.GetState().IsKeyDown(Keys.OemPlus) && zoomedIn == false)
                {
                    zoomedIn = true;
                    SIZE_OF_CELL = 50;
                    _player.spritePosition = new Vector2(Mouse.GetState().X * SIZE_OF_CELL / 10, Mouse.GetState().Y * SIZE_OF_CELL / 10);
                    tileArray = tileMap.CreateMap(SIZE_OF_CELL, NUMBER_OF_TILES, textBoxTexture);
                    allowPlayerMovement = true;

                    List<Movable> tempList = new List<Movable>();
                    int size = 5;

                    foreach (Movable m in listCreatures)
                    {
                        tempList.Add(new Movable(new Rectangle((int)m.spritePosition.X * size, (int)m.spritePosition.Y * size, 10, 10), m.texture, new Vector2(m.spritePosition.X * size, m.spritePosition.Y * size), m.spriteColour));
                    }
                    listCreatures.Clear();
                    foreach (Movable m in tempList)
                    {
                        listCreatures.Add(m);
                    }
                    tempList.Clear();

                    foreach (Movable m in listPlants)
                    {
                        tempList.Add(new Movable(new Rectangle((int)m.spritePosition.X * size, (int)m.spritePosition.Y * size, 10, 10), m.texture, new Vector2(m.spritePosition.X * size, m.spritePosition.Y * size), m.spriteColour));
                    }
                    listPlants.Clear();
                    foreach (Movable m in tempList)
                    {
                        listPlants.Add(m);
                    }
                    tempList.Clear();
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
                                creatures = new Movable(new Rectangle(x, y, 10, 10), textBoxTexture, new Vector2(x, y), Color.MediumVioletRed);
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

                    foreach (Tile t in tileArray)
                    {
                        if (tempPlace.Intersects(t.position))
                        {
                            if (t.tileType != "Rock")
                            {
                                plants = new Movable(new Rectangle(x, y, 10, 10), textBoxTexture, new Vector2(x, y), Color.Green);
                                listPlants.Add(plants);
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

            _spriteBatch.Begin(transformMatrix: _camera.transformMatrix);

            _player.Draw(_spriteBatch);

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
                    m.Draw(_spriteBatch);
                }

                foreach(Movable m in listPlants)
                {
                    m.Draw(_spriteBatch);
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}