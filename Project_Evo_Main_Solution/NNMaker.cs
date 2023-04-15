using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SharpDX;

namespace Project_Evo_Main_Solution
{
    public class NNMaker
    {
        /* This is the class where most, if not all, of the NN functions will be held
         * In this class, I'll be able to make the NN
         * Let's hope for the best
         * 
         * Additionally, the main resource I'm using is making this for Unity, which is, well, not Visual Studio.
         */

        private int[] layers;//layers    
        private float[][] neurons;//neurons    
        private float[][] biases;//biasses    
        private float[][][] weights;//weights    
        private int[] activations;//layers

        public float fitness = 0;//fitness

        private Random randomNumber = new Random();

        public NNMaker()
        {

        }

        public NNMaker(int[] layers)
        {
            this.layers = new int[layers.Length];

            for(int i = 0; i < layers.Length; i++)
            {
                this.layers[i] = layers[i];
            }

            this.layers[this.layers.Length - 1] = 9;


            InitNeurons();
            InitBiases();
            InitWeights();
        }

        /*
         * Layers work like this:
         * L0: [N] [N] [N] [N] [N]
         * L1: [N] [N] [N] [N] [N]
         * ...
         * LN: [N] [N] [N] [N] [N]
         * LN acts as the OUTPUT layer! It goes DOWNWARDS, not sideways
         * L0 would be the input layer
         * I want to be able to add in more nodes on the L1 - LN-1 layers
         */

        private void InitNeurons()
        {
            List<float[]> neuronList = new List<float[]>();

            for (int i = 0; i < layers.Length; i++) // It makes 5 (or any number) "rows" of neurons/nodes
            {
                neuronList.Add(new float[layers[i]]); // Based on the values within the layers array, it adds that many layers/additional nodes onto the row/chain
            }

            neurons = neuronList.ToArray();
        }

        private void InitBiases()
        {
            List<float[]> biasList = new List<float[]>();

            for(int i = 0; i < layers.Length; i++) // It, again, goes through all the rows/chains individually
            {
                float[] bias = new float[layers[i]]; // It holds the "depth" of each chain

                for(int j = 0; j < layers[i]; j++) // It goes through every node within the chain
                {
                    bias[j] = randomNumber.Next(-10, 10); // It creates a random number between -10 and 10 as a bias
                }

                biasList.Add(bias);
            }

            biases = biasList.ToArray();
        }

        private void InitWeights()
        {
            List<float[][]> weightsList = new List<float[][]>();

            for(int i = 1; i < layers.Length; i++) // This looks through each individual row/chain
            {
                List<float[]> layerWeightsList = new List<float[]>();

                int neuronsInPreviousLayer = layers[i - 1]; // It holds the depth of the previous chain

                for(int j = 0; j < neurons[i].Length; j++) // This goes through each neuron within the chain; the i is the chain it's looking at, so length is the length of that chain
                {
                    float[] neuronWeight = new float[neuronsInPreviousLayer]; // Neurons in previous layer are usually the same or higher quantity than the current layer

                    for(int k = 0; k < neuronsInPreviousLayer; k++)
                    {
                        neuronWeight[k] = randomNumber.Next(-10, 10); // You have to add the weights to the previous chain because the output layer/chain cannot have weights, but can have biases
                        // Weights affter what the RESULT of the node comes out as whenever passed to another
                    }

                    layerWeightsList.Add(neuronWeight);
                }

                weightsList.Add(layerWeightsList.ToArray());
            }

            weights = weightsList.ToArray();
        }

        public float Activate(float value)
        {
            return (float)Math.Tanh(value);
        }
        // This returns the tanh of the value given. Glad it exists because I didn't want to write out the formula

        // This function allows to calculate what the output is from a number of inputs
        public float[] FeedForward(float[] inputs)
        {
            for(int i = 0; i < inputs.Length; i++)
            {
                neurons[0][i] = inputs[i]; // The neurons on the first chain act as the inputs
            }

            for(int i = 1; i < neurons.Length; i++) // Starting from the second layer, look through every other layer
            {
                int layer = i - 1; // Look at the previous layer

                for(int j = 0; j < neurons[i - 1].Length; j++) // Look at every neuron inside the previous layer
                {
                    float value = 0f;

                    for(int k = 0; k < neurons[i - 1].Length; k++) // Look through every neuron within the previous layer again
                    {
                        value += weights[i - 1][j][k] * neurons[i - 1][k]; // The weights determine how the value is passed from one neuron to another, and the neuron holds the actual value
                    }

                    neurons[i][j] = Activate(value + biases[i][j]);
                }
            }

            return neurons[neurons.Length - 1];
        }

        public void Mutate(int chance, float val)
        {
            for (int i = 0; i < biases.Length; i++) // Change the value of biases
            {
                for (int j = 0; j < biases[i].Length; j++)
                {
                    biases[i][j] = (randomNumber.Next(0, chance) <= 5) ? biases[i][j] += randomNumber.NextFloat((int)-val, (int)val) : biases[i][j];
                }
            }

            for (int i = 0; i < weights.Length; i++) // change the value of weights
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        weights[i][j][k] = (randomNumber.Next(0, chance) <= 5) ? weights[i][j][k] += randomNumber.NextFloat((int)-val, (int)val) : weights[i][j][k];
                    }
                }
            }

            if(randomNumber.Next(0, chance) <= 5)
            {
                int layerIamLookingAt = randomNumber.Next(0, layers.Length - 1);

                layers[layerIamLookingAt] = layers[layerIamLookingAt] + 1;
                // This looks at a random middle layer, from layer 1 (not input) to the second-to-last layer (so not output either)
                // Then it adds an extra node to it

                InitNeurons(); // It's safe to intialise neurons again as they only store the number of nodes on each layer
                // Biases and weights are a different issue, however, as now I need to initialise the SPECIFIC bias and weight.

                List<float[]> biasList = new List<float[]>();

                for (int i = 0; i < layers.Length; i++) // It, again, goes through all the rows/chains individually
                {
                    float[] bias = new float[layers[i]]; // It holds the "depth" of each chain

                    for (int j = 0; j < layers[i]; j++) // It goes through every node within the chain
                    {
                        if(i == layerIamLookingAt - 1)
                        {
                            if(j == layers[i] - 1)
                            {
                                bias[j] = randomNumber.Next(-10, 10); // It creates a random number between -10 and 10 as a bias
                            }
                        }
                    }

                    biasList.Add(bias);
                }

                biases = biasList.ToArray();


                List<float[][]> weightsList = new List<float[][]>();

                for (int i = 1; i < layers.Length; i++) // This looks through each individual row/chain
                {
                    List<float[]> layerWeightsList = new List<float[]>();

                    int neuronsInPreviousLayer = layers[i - 1]; // It holds the depth of the previous chain

                    for (int j = 0; j < neurons[i].Length; j++) // This goes through each neuron within the chain; the i is the chain it's looking at, so length is the length of that chain
                    {
                        float[] neuronWeight = new float[neuronsInPreviousLayer]; // Neurons in previous layer are usually the same or higher quantity than the current layer

                        for (int k = 0; k < neuronsInPreviousLayer; k++)
                        {
                            if(i == layerIamLookingAt - 1)
                            {
                                if(j == neurons[layerIamLookingAt].Length - 1)
                                {
                                    if(k == neurons[neuronsInPreviousLayer].Length - 1)
                                    {
                                        neuronWeight[k] = randomNumber.Next(-10, 10);
                                    }
                                }
                            }
                        }

                        layerWeightsList.Add(neuronWeight);
                    }

                    weightsList.Add(layerWeightsList.ToArray());
                }

                weights = weightsList.ToArray();

                // These should create new biases and weights based on the layer I edited
            }
        }

        public float[][] GetNeuronsArray()
        {
            return neurons;
        }

        public float[][] GetBiasesArray()
        {
            return biases;
        }

        public float[][][] GetWeightsArray()
        {
            return weights;
        }

    }
}
