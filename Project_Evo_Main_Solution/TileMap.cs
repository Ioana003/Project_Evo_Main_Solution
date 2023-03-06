using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Evo_Main_Solution
{
    internal class TileMap
    {
        private Tile[,] tileArray;
        private float[][] tilesArrayFloat;

        public TileMap()
        {

        }

        public TileMap(float[][] map)
        {
            this.tilesArrayFloat = map;
        }

        public Tile[,] CreateMap(int tileSize, int tileAmount, Texture2D texture)
        {
            tileArray = new Tile[tileAmount, tileAmount];

            for(int i = 0; i < tileAmount; i++)
            {
                for(int j = 0; j < tileAmount; j++)
                {
                    if (tilesArrayFloat[i][j] >= 0 && tilesArrayFloat[i][j] < 0.35)
                    {
                        tileArray[i, j] = new Tile(texture, new Rectangle(tileSize * i, tileSize * j, tileSize, tileSize), Color.CadetBlue, "SeaWater");
                        tileArray[i, j].nutrientAmount = 300;
                    }
                    else if (tilesArrayFloat[i][j] >= 0.35 && tilesArrayFloat[i][j] < 0.4)
                    {
                        tileArray[i, j] = new Tile(texture, new Rectangle(tileSize * i, tileSize * j, tileSize, tileSize), Color.SkyBlue, "FreshWater");
                        tileArray[i, j].nutrientAmount = 500;
                    }
                    else if (tilesArrayFloat[i][j] >= 0.4 && tilesArrayFloat[i][j] < 0.45)
                    {
                        tileArray[i, j] = new Tile(texture, new Rectangle(tileSize * i, tileSize * j, tileSize, tileSize), Color.SandyBrown, "Sand");
                        tileArray[i, j].nutrientAmount = 100;
                    }
                    else if (tilesArrayFloat[i][j] >= 0.45 && tilesArrayFloat[i][j] < 0.7)
                    {
                        tileArray[i, j] = new Tile(texture, new Rectangle(tileSize * i, tileSize * j, tileSize, tileSize), Color.SaddleBrown, "Soil");
                        tileArray[i, j].nutrientAmount = 1000;
                    }
                    else if (tilesArrayFloat[i][j] >= 0.7)
                    {
                        tileArray[i, j] = new Tile(texture, new Rectangle(tileSize * i, tileSize * j, tileSize, tileSize), Color.DarkSlateGray, "Rock");
                        tileArray[i, j].nutrientAmount = 0;
                    }
                }
            }

            return tileArray;
        }

    }
}
