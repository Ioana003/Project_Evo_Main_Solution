using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project_Evo_Prot_5___Tile_Map;
using Project_Evo_Prototype_1___Map_Creation;
using System;

namespace Project_Evo_Main_Solution
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont font_def;
        private string inputString = "";
        private Rectangle textBoxRectangle;
        private Texture2D textBoxTexture;
        private MouseManager mouseManager = new MouseManager();
        private TextWriter textWriter = new TextWriter();
        private bool allowText = false;
        private const int SIZE_OF_CELL = 10;
        private bool showTyping = true;
        private Random randomNumber = new Random();
        private bool allowKeyboardTap = true;

        private FileManager fileManager = new FileManager();
        private TileMap tileMap = new TileMap();
        private Tile[,] tileArray;
        private float[][] map;
        const int NUMBER_OF_TILES = 100;

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

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            textBoxTexture = Content.Load<Texture2D>("textRectangle");
            textBoxRectangle = new Rectangle(Window.ClientBounds.Width / 2 - 150 / 2, Window.ClientBounds.Height / 2 - 25 / 2, 150, 25);

            font_def = Content.Load<SpriteFont>("default_text");

            textWriter = new TextWriter(textBoxRectangle);

            tileArray = new Tile[NUMBER_OF_TILES, NUMBER_OF_TILES];

            map = new float[NUMBER_OF_TILES][];
            for (int i = 0; i < NUMBER_OF_TILES; i++)
            {
                map[i] = new float[NUMBER_OF_TILES];
            }

            tileMap = new TileMap(map);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            // This is the text box that lets you input your seed
            if (showTyping == true)
            {
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

                if(Keyboard.GetState().IsKeyDown(Keys.Back))
                {
                    showTyping = true;
                    allowKeyboardTap = true;
                }
            }

            // If you press enter, it creates a perlin noise map and creates how it looks
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && allowKeyboardTap == true)
            {
                allowKeyboardTap = false;

                showTyping = false;

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

                textWriter.SetInputtedString("");

            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);

            // TODO: Add your drawing code here

            _spriteBatch.Begin();

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
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}