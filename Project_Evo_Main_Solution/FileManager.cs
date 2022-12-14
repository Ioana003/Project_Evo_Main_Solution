using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Project_Evo_Prot_5___Tile_Map
{
    internal class FileManager
    {

        private Random randomNumber = new Random();
        private TextWriter writer;
        private StreamReader reader;
        private NoiseManager noiseManager = new NoiseManager();
        private float[][] perlinNoise;

        public FileManager()
        {

        }

        public void WriteToFile(int tileAmount, int seed)
        {
            writer = File.CreateText("C:\\Users\\10GherasimI.SCRCAT\\Desktop\\TextFiles\\TileMapFile1.txt");

            perlinNoise = noiseManager.GeneratePerlinNoise(noiseManager.GenerateWhiteNoise(tileAmount, tileAmount, seed), 6);

            for (int i = 0; i < tileAmount; i++)
            {
                for(int j = 0; j < tileAmount; j++)
                {
                    if (j != tileAmount - 1)
                    {
                        writer.Write(perlinNoise[i][j] + ",");
                    }
                    else
                    {
                        writer.Write(perlinNoise[i][j]);
                    }
                }
                writer.WriteLine();
            }

            writer.Close();
        }

        public float[,] ReadFile(int tileAmount)
        {
            reader = new StreamReader("C:\\Users\\10GherasimI.SCRCAT\\Desktop\\TextFiles\\TileMapFile1.txt");

            string line = "";
            int counter = 0;
            float[,] tileType;
            
            tileType = new float[tileAmount, tileAmount];

            do
            {
                List<float> tileList = new List<float>();
                line = reader.ReadLine();
                string[] stringArray = line?.Split(',');

                foreach (string s in stringArray)
                {
                    tileList.Add(float.Parse(s));
                }

                float[] fArrary = tileList.ToArray();

                for(int i = 0; i < tileAmount; i++)
                {
                    tileType[counter, i] = fArrary[i];
                }

                if(counter < tileAmount - 1)
                {
                    counter++;
                }

            } while (!reader.EndOfStream);
            reader.Close();

            return tileType;
        }

    }
}
