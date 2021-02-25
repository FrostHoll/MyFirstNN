using System;

namespace MyFirstNN
{
    class Program
    {
        /// <summary>
        /// Количество входных значений для нейронной сети.
        /// </summary>
        const int INPUTS = 2;
        /// <summary>
        /// Общее количество нейронных уровней(входной + скрытые + выходной).
        /// </summary>
        const int LEVEL_COUNT = 3;
        /// <summary>
        /// В качестве активационной функции будет использоваться функция гиперболического тангенса.
        /// </summary>
        static public bool USE_TAN = true;
        /// <summary>
        /// Использовать нейрон смещения в нейронной сети(создается на каждом нейронном уровне).
        /// </summary>
        static public int BIAS = 0;
        /// <summary>
        /// Количество нейронов на нейронных уровнях(Кол-во входных знач., скрытые уровни, кол-во выходных знач.). 
        /// </summary>
        private static readonly int[] NEURON_LEVELS_COUNT = { 2, 3, 1 };
        /// <summary>
        /// Максимальное количество эпох(число прогонов тренировочных данных).
        /// </summary>
        private static readonly int maxEpoch = 1;
        /// <summary>
        /// Тренировочные данные(последние элементы каждого одномерного массива - идеальный ответ).
        /// </summary>
        private static readonly int[,] TrainSets = { { 0, 0, 0 }, { 0, 1, 1 }, { 1, 0, 1 }, { 1, 1, 0 } };

        static public float[] Input = new float[INPUTS];

        static public NeuronNet neuronNet;

        static public Random Random = new Random();

        static void Main(string[] args)
        {
            neuronNet = new NeuronNet(LEVEL_COUNT, NEURON_LEVELS_COUNT);

            //Train(TrainSets, maxEpoch, 0.3f, 0.1f);
            //Selection(TrainSets, 1000);


            //for (int i = 0; i < neuronNet.neuronsCount; i++)
            //{
            //    for (int j = 0; j < neuronNet.neuronsCount; j++)
            //    {
            //        Console.Write($"{neuronNet.weights[i, j]} ");
            //    }
            //    Console.Write("\n");
            //}

            //TODO weights graph system

            while (true)
            {
                Console.WriteLine("Enter " + INPUTS + " inputs:");
                float temp = 0f;
                string[] s = Console.ReadLine().Split(' ');
                if (!float.TryParse(s[0], out temp)) break;
                for (int i = 0; i < INPUTS; i++)
                {
                    Input[i] = float.Parse(s[i]);
                }

                neuronNet.Action(Input);
                Console.WriteLine("Answer: " + neuronNet.GetAnswer()[0].ToString() + "\n");
            }



        }

        /// <summary>
        /// Метод отбора нейронной сети с наименьшей ошибкой среди случайно сгенерированных.
        /// </summary>
        /// <param name="sets">Тренировочные данные, для которых будет подсчитываться ошибка сети.</param>
        /// <param name="epochs">Количество случайно сгенерированных нейронных сетей для отбора.</param>
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
                nn2 = new NeuronNet(LEVEL_COUNT, NEURON_LEVELS_COUNT);
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

        /// <summary>
        /// Метод обучения нейронной сети алгоритмом обратного распространения ошибки.
        /// </summary>
        /// <param name="sets">Тренировочные данные, по которым будет обучаться нейросеть.</param>
        /// <param name="epochs">Количество прогонов тренировочных данных.</param>
        /// <param name="epsilon">Коэффициент обучения нейросети(-!задавать с осторожностью!-).</param>
        /// <param name="alph">Коэффициент момента(-!задавать с осторожностью!-).</param>
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

        /// <summary>
        /// Вывод одномерного массива на экран.
        /// </summary>
        /// <param name="arr">Целочисленный массив для вывода на экран.</param>
        public static void PrintArr(int[] arr)
        {
            foreach (var i in arr)
            {
                Console.Write(i.ToString() + " ");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Вывод одномерного массива на экран.
        /// </summary>
        /// <param name="arr">Массив для чисел с плавающей точкой для вывода на экран.</param>
        public static void PrintArr(float[] arr)
        {
            foreach (var i in arr)
            {
                Console.Write(i.ToString() + " ");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Метод инициализации значений весов случайным образом. 
        /// </summary>
        /// <returns>На выходе число с плавающей точкой в диапазоне [-1.00000;1.00000]</returns>
        public static float SetRandWeight()
        {
            return ((float)Random.Next(0, 200000) / 100000) - 1f;
        }

        /// <summary>
        /// Активационная функция(Сигмоидная функция по умолчанию).
        /// </summary>
        /// <param name="x">Аргумент активационной функции.</param>
        /// <param name="tan">Вместо сигмоидной функции использовать функцию с гиперболическим тангенсом.</param>
        /// <returns>Реакция нейрона на сигнал. ([0;1] для сигмоидной, [-1;1] для tanh)</returns>
        public static float Activation(float x, bool tan = false)
        {
            return !tan ? 1 / (1 + (float)Math.Exp(-x)) : ((float)Math.Exp(2 * x) - 1) / ((float)Math.Exp(2 * x) + 1);
        }
    }

    /// <summary>
    /// Класс, реализирующий нейронную сеть и включающий нейронные уровни.
    /// </summary>
    class NeuronNet
    {

        NeuronLevel[] neuronLevels;
        /// <summary>
        /// Количество нейронных уровней.
        /// </summary>
        int k;

        //TODO убрать public после доработки системы подсчета весов.
        public int neuronsCount = 0;
        public float[,] weights;

        float[] answer;


        /// <summary>
        /// Инициализация нейронной сети.
        /// </summary>
        /// <param name="neuronLevelCount">Количество нейронных уровней.</param>
        /// <param name="neuronLevelNeuronsCount">Количество нейронов на каждом нейронном уровне.</param>
        public NeuronNet(int neuronLevelCount, int[] neuronLevelNeuronsCount)
        {
            k = neuronLevelCount;
            
            foreach (int n in neuronLevelNeuronsCount) neuronsCount += n;
            weights = new float[neuronsCount, neuronsCount];
            for (int i = 0; i < neuronsCount; i++) for (int j = 0; j < neuronsCount; j++) weights[i, j] = -1f;

            neuronLevels = new NeuronLevel[k];
            neuronLevels[0] = new NeuronLevel(neuronLevelNeuronsCount[0], 0);
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

        /// <summary>
        /// Получение выходных значений нейросети.
        /// </summary>
        /// <returns>На выходе массив из значений нейронов выходного слоя.</returns>
        public float[] GetAnswer() => answer;

        /// <summary>
        /// Метод, отдающий команду подсчитать нейронной сети выходные значения по входным значениям.
        /// </summary>
        /// <param name="input">Массив с входными значениями.</param>
        public void Action(float[] input)
        {
            neuronLevels[0].SetValues(input);
            neuronLevels[1].Action(neuronLevels[0].GetNeuronValues());
            answer = neuronLevels[k - 1].GetNeuronValues();
        }

        
    }

    /// <summary>
    /// Класс, реализующий нейронные уровни и включающий нейроны.
    /// </summary>
    class NeuronLevel
    {
        NeuronLevel prevNL = null;
        NeuronLevel nextNL = null;

        Neuron[] neurons;
        float[] neuronValues;

        /// <summary>
        /// Инициализация нейронного уровня.
        /// </summary>
        /// <param name="neuronsCount">Количество нейронов.</param>
        /// <param name="inputCount">Количество входных связей для нейроннов этого уровня.</param>
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

        /// <summary>
        /// Получение значений нейронов.
        /// </summary>
        /// <returns>На выходе массив выходных значений нейронов этого уровня.</returns>
        public float[] GetNeuronValues() => neuronValues;

        /// <summary>
        /// Установить выходные значения нейронов(-!не использовать кроме инициализации первого слоя!-).
        /// </summary>
        /// <param name="values">Необходимые значения для нейронов.</param>
        public void SetValues(float[] values)
        {
            if (values.Length != neuronValues.Length)
            {
                Console.WriteLine("ERROR SETTING NEURON LEVEL VALUES");
                return;
            }

            for (int i = 0; i < values.Length; i++)
            {
                neurons[i].SetValue(values[i]);
                neuronValues[i] = values[i];
            }
        }

        /// <summary>
        /// Установка связей между соседними нейронными уровнями.
        /// </summary>
        /// <param name="prev">Ссылка на предшествующий уровень(для первого уровня - null).</param>
        /// <param name="next">Ссылка на следующий уровень(для последнего уровня - null).</param>
        public void SetRelate(NeuronLevel prev, NeuronLevel next)
        {
            prevNL = prev;
            nextNL = next;
        }

        /// <summary>
        /// Метод, отдающий команду нейронному уровню подсчитать значения выходных нейронов по входным данным.
        /// </summary>
        /// <param name="input">Входные данные для уровня(для первого - входные значения, для следующих - выходные значения нейронов предыдущего уровня).</param>
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

    /// <summary>
    /// Класс, реализующий нейрон и включающий все веса связей с предыдущими нейронами.
    /// </summary>
    class Neuron
    {
        float currWeight = 0f;

        float[] weights;

        /// <summary>
        /// Инициализация нейрона.
        /// </summary>
        /// <param name="weightsCount">Количество входных связей(0 для нейронов входного уровня).</param>
        public Neuron(int weightsCount)
        {
            if (weightsCount == 0) return;
            weights = new float[weightsCount];
            for (int i = 0; i < weightsCount; i++)
            {
                weights[i] = Program.SetRandWeight();
            }
            //Console.Write("\tNeuron: ");
            //Program.PrintArr(weights);
        }

        /// <summary>
        /// Получение значений нейрона.
        /// </summary>
        /// <returns>На выходе выходное значение нейрона.</returns>
        public float GetWeight() => currWeight;

        /// <summary>
        /// Установка выходного значения нейрона(-!не использовать кроме нейронов входного уровня!-).
        /// </summary>
        /// <param name="value">Необходимое значение нейрона.</param>
        public void SetValue(float value)
        {
            currWeight = value;
        }

        /// <summary>
        /// Метод, отдающий команду подсчитать нейрону выходное значение по входным значениям.
        /// </summary>
        /// <param name="inp">Входные значения.</param>
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
