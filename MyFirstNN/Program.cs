using System;

namespace MyFirstNN
{
    class Program
    {
        const int INPUTS = 2;
        const int LEVEL_COUNT = 2;
        static public bool USE_TAN = true; //use tan activation func
        static public int BIAS = 1; //1 - use BIAS neuron
        private static readonly int[] NEURON_LEVELS_COUNT = { 3, 1 };

        static readonly int maxEpoch = 1;
        static readonly int[,] TrainSets = { { 0, 0, 0 }, { 0, 1, 1 }, { 1, 0, 1 }, { 1, 1, 0 } };

        static public float[] Input = new float[INPUTS];

        static public NeuronNet neuronNet;

        static public Random Random = new Random();

        static void Main(string[] args)
        {
            neuronNet = new NeuronNet(LEVEL_COUNT, NEURON_LEVELS_COUNT, INPUTS);

            //Train(TrainSets, maxEpoch, 0.3f, 0.1f);
            //Selection(TrainSets, 1000);

            for (int i = 0; i < neuronNet.neuronsCount; i++)
            {
                for (int j = 0; j < neuronNet.neuronsCount; j++)
                {
                    Console.Write($"{neuronNet.weights[i, j]} ");
                }
                Console.Write("\n");
            }

            //while (true)
            //{
            //    Console.WriteLine("Enter " + INPUTS + " inputs:");
            //    float temp = 0f;
            //    string[] s = Console.ReadLine().Split(' ');
            //    if (!float.TryParse(s[0], out temp)) break;
            //    for (int i = 0; i < INPUTS; i++)
            //    {
            //        Input[i] = float.Parse(s[i]);
            //    }

            //    neuronNet.Action(Input);
            //    Console.WriteLine("Answer: " + neuronNet.GetAnswer()[0].ToString() + "\n");
            //}



        }

        static public void Selection(int[,] sets, int epochs)
        {
            Console.WriteLine("\nSelection started.\n");

            float error1 = 0f;

            for (int set = 0; set < sets.GetLength(0); set++)
            {
                for (int i = 0; i < INPUTS; i++)
                {
                    Input[i] = sets[set, i];
                }
                neuronNet.Action(Input);
                error1 += (neuronNet.GetAnswer()[0] - (float)sets[set, INPUTS]) * (neuronNet.GetAnswer()[0] - (float)sets[set, INPUTS]);
                Console.WriteLine("\n" + set + "- Answer: " + neuronNet.GetAnswer()[0].ToString() + " Correct: " + sets[set, INPUTS]);
            }
            error1 /= sets.GetLength(0);
            Console.WriteLine("initial NN error: " + error1 * 100);

            NeuronNet nn2 = null;
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                float error2 = 0f;
                nn2 = new NeuronNet(LEVEL_COUNT, NEURON_LEVELS_COUNT, INPUTS);
                //Console.WriteLine("contender " + epoch);
                for (int set = 0; set < sets.GetLength(0); set++)
                {
                    for (int i = 0; i < INPUTS; i++)
                    {
                        Input[i] = sets[set, i];
                    }
                    nn2.Action(Input);
                    error2 += (nn2.GetAnswer()[0] - (float)sets[set, INPUTS]) * (nn2.GetAnswer()[0] - (float)sets[set, INPUTS]);
                    //Console.WriteLine("\n" + set + "- Answer: " + nn2.GetAnswer()[0].ToString() + " Correct: " + sets[set, INPUTS]);
                }
                error2 /= sets.GetLength(0);
                //Console.WriteLine("contender error: " + error2 * 100);

                if (error2 < error1)
                {
                    //Console.WriteLine("result - BETTER");
                    neuronNet = nn2;
                    error1 = error2;
                    nn2 = null;
                }
                else
                {
                    //Console.WriteLine("result - WORSE");
                    nn2 = null;
                }

            }

            Console.WriteLine("\nSelection ended. Best error - " + error1 * 100);

        }

        static public void Train(int[,] sets, int epochs, float epsilon, float alph)
        {
            Console.WriteLine("\nTraining started.\n");

            for (int epoch = 0; epoch < epochs; epoch++)
            {
                float error = 0f;
                Console.WriteLine("epoch " + epoch);
                for (int set = 0; set < sets.GetLength(0); set++)
                {
                    for (int i = 0; i < INPUTS; i++)
                    {
                        Input[i] = sets[set, i];                        
                    }
                    neuronNet.Action(Input);
                    error += (neuronNet.GetAnswer()[0] - (float)sets[set, INPUTS]) * (neuronNet.GetAnswer()[0] - (float)sets[set, INPUTS]);
                    Console.WriteLine("\n" + set + "- Answer: " + neuronNet.GetAnswer()[0].ToString() + " Correct: " + sets[set, INPUTS]);
                }
                error /= sets.GetLength(0);
                Console.WriteLine("epoch error: " + error * 100);
            }

            Console.WriteLine("\nTraining ended.");
        }

        public static void PrintArr(int[] arr)
        {
            foreach (var i in arr)
            {
                Console.Write(i.ToString() + " ");
            }
            Console.WriteLine();
        }

        public static void PrintArr(float[] arr)
        {
            foreach (var i in arr)
            {
                Console.Write(i.ToString() + " ");
            }
            Console.WriteLine();
        }

        public static float SetRandWeight()
        {
            return ((float)Random.Next(0, 200000) / 100000) - 1f;
        }

        public static float Activation(float x, bool tan = false)
        {
            return !tan ? 1 / (1 + (float)Math.Exp(-x)) : ((float)Math.Exp(2 * x) - 1) / ((float)Math.Exp(2 * x) + 1);
        }
    }

    class NeuronNet
    {

        NeuronLevel[] neuronLevels;
        /// <summary>
        /// NeuronLevelsCount
        /// </summary>
        int k;

        public int neuronsCount = 0;
        public float[,] weights;
        float[] answer;

        /// <summary>
        /// инициализация нейронной сети
        /// </summary>
        /// <param name="neuronLevelCount">количество нейронных уровней</param>
        /// <param name="neuronLevelNeuronsCount">количество нейронов на каждом нейронном уровне</param>
        /// <param name="inputCount">количество входных данных</param>
        public NeuronNet(int neuronLevelCount, int[] neuronLevelNeuronsCount, int inputCount)
        {
            k = neuronLevelCount;
            neuronLevels = new NeuronLevel[k];
            foreach (int n in neuronLevelNeuronsCount) neuronsCount += n;
            weights = new float[neuronsCount, neuronsCount];
            neuronLevels[0] = new NeuronLevel(neuronLevelNeuronsCount[0], inputCount);
            for (int i = 1; i < k; i++)
            {
                neuronLevels[i] = new NeuronLevel(neuronLevelNeuronsCount[i], neuronLevelNeuronsCount[i - 1]);
            }
            neuronLevels[0].SetRelate(null, neuronLevels[1]);
            neuronLevels[k - 1].SetRelate(neuronLevels[k - 2], null);
            for (int i = 1; i <= k - 2; i++)
            {
                neuronLevels[i].SetRelate(neuronLevels[i - 1], neuronLevels[i + 1]);
            }
            
        }

        public float[] GetAnswer() => answer;

        public void Action(float[] input)
        {
            neuronLevels[0].Action(input);
            answer = neuronLevels[k - 1].GetNeuronValues();
        }

        
    }

    class NeuronLevel
    {
        NeuronLevel prevNL = null;
        NeuronLevel nextNL = null;

        Neuron[] neurons;
        float[] neuronValues;

        public NeuronLevel(int neuronsCount, int inputCount)
        {
            //Console.WriteLine("NeuronLevel:");
            neurons = new Neuron[neuronsCount];
            neuronValues = new float[neuronsCount];
            for (int i = 0; i < neuronsCount; i++)
            {
                neurons[i] = new Neuron(inputCount + Program.BIAS);
            }
        }

        public float[] GetNeuronValues() => neuronValues;

        public void SetRelate(NeuronLevel prev, NeuronLevel next)
        {
            prevNL = prev;
            nextNL = next;
        }

        public void Action(float[] input)
        {

            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i].Action(input);
                neuronValues[i] = neurons[i].GetWeight();
            }

            if (nextNL != null)
            {
                nextNL.Action(neuronValues);
            }
        }

    }

    class Neuron
    {
        float currWeight = 0f;

        float[] weights;

        public Neuron(int weightsCount)
        {
            weights = new float[weightsCount];
            for (int i = 0; i < weightsCount; i++)
            {
                weights[i] = Program.SetRandWeight();
            }
            //Console.Write("\tNeuron: ");
            //Program.PrintArr(weights);
        }

        public float GetWeight() => currWeight;

        public void Action(float[] inp)
        {
            currWeight = 0f;

            if (Program.BIAS == 0 && weights.Length != inp.Length)
            {
                Console.WriteLine("ERROR: WRONG INPUT COUNT");
                return;
            }

            for (int i = 0; i < weights.Length - Program.BIAS; i++)
            {
                currWeight += inp[i] * weights[i];
            }

            currWeight = Program.Activation(currWeight + weights[^1] * Program.BIAS, Program.USE_TAN);
        }
    }
}
