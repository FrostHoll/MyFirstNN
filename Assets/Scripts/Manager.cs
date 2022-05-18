using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [SerializeField] Text nnText;

    [SerializeField] Text testButtonText;

    [SerializeField] Vector3 NeuronNetPos;

    [SerializeField] private int[] NLNeuronsCount;

    [SerializeField] private float[] input;

    [SerializeField] private CarVisual carVisual;

    private NeuronNetVisual neuronNetObj;
    private GameObject NNPrefab;

    private TrainSet trainSet;

    private TextLog logger;

    private Trainer trainer;

    private int trainSetsCount = 0;

    public void SetText(float[] input, float[] output, float[] correct = null)
    {
        nnText.text = "Input:\n";
        string s = "";
        for (int i = 0; i < input.Length; i++)
        {
            s += input[i] + " ";
        }
        nnText.text += s + "\nOutput:\n";
        s = "";
        for (int i = 0; i < output.Length; i++)
        {
            s += output[i].ToString("F2") + " ";
        }
        nnText.text += s;
        if (correct != null)
        {
            s = "\nCorrect:\n";
            for (int i = 0; i < correct.Length; i++)
            {
                s += correct[i] + " ";
            }
            nnText.text += s;


        }
    }

    public void Test()
    {
        Set? set;
        if (trainSet.TryGetNextSet(out set))
        {
            CalculateNeuronNet(neuronNetObj.NeuronNet, set.Value.input, set.Value.answer);

            testButtonText.text = $"Test({trainSet.TrainSetIndex}/{trainSetsCount})";
        }
        else
        {
            testButtonText.text = $"Test(0/{trainSetsCount})";
            nnText.text = "";

        }
    }

    public void NextWindow()
    {
        SceneManager.LoadScene(1);
    }

    private void Awake()
    {
        NNPrefab = Resources.Load<GameObject>("Prefabs/NeuronNet");
        neuronNetObj = Instantiate(NNPrefab, NeuronNetPos, Quaternion.identity).GetComponent<NeuronNetVisual>();
        neuronNetObj.InitNeuronNet(NLNeuronsCount.Length, NLNeuronsCount);
        logger = GetComponent<TextLog>();

        trainer = GetComponent<Trainer>();
        trainer.Init(neuronNetObj.NeuronNet);

        trainSet = GetComponent<TrainSet>();
        if (trainSet == null) Debug.LogError("TrainSet not found");

    }

    private void Start()
    {
        trainSetsCount = trainSet.TrainSets.Length;
        testButtonText.text = $"Test(0/{trainSetsCount})";
    }

    private void Update()
    {

        ///Вычислить ответ на конкретный вход
        if (Input.GetKeyDown(KeyCode.N))
        {
            CalculateNeuronNet(neuronNetObj.NeuronNet, input);
        }
        ///Пробежать трейнсет + логи
        if (Input.GetKeyDown(KeyCode.W))
        {
            float err = 0f;
            AutoCalculateTrainSet(neuronNetObj.NeuronNet, trainSet, out err);
        }
    }

    private void CalculateNeuronNet(NeuronNet nn, float[] input, float[] correct = null)
    {
        nn.Action(input);
        SetText(input, nn.GetAnswer(), correct);
        carVisual.UpdateSigns(input);
        //string y = "";
        //float[] t = Program.Softmax(nn.GetRawAnswers());
        //foreach (var i in t) y += i.ToString("F4") + " ";
        //Debug.Log(y);
    }

    public void AutoCalculateTrainSet(NeuronNet nn, TrainSet trainSet, out float error)
    {
        logger.LogClear();
        error = 0f;

        if (!trainSet.IsTrainSetLoaded) return;

        float count = 0f;
        for (int i = 0; i < trainSet.TrainSets.Length; i++)
        {
            Set set = trainSet.TrainSets[i];
            nn.Action(set.input);
            float[] ans = nn.GetAnswer();
            for (int j = 0; j < ans.Length; j++)
            {
                count++;
                error += Mathf.Pow(ans[j] - set.answer[j], 2);
            }
            logger.LogAny($"[{i}] input:[");
            foreach (float item in set.input) logger.LogAny($" {item} ", false);
            logger.LogAny("] \noutput:[", false);
            foreach (float item in ans) logger.LogAny($" {item} ", false);
            logger.LogAny("] \ncorrect:[", false);
            foreach (float item in set.answer) logger.LogAny($" {item} ", false);
            logger.LogAny($"]\nCrossEntropyError: {Program.CrossEntropy(ans, set.answer)}", false);
        }
        error = Mathf.Sqrt(error / count);
        logger.LogAny($"Overall error: {error}");
    }
}
