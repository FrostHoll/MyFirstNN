using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] Transform neuronNetPos;

    [SerializeField] private int[] NLNeuronsCount;

    [SerializeField] float alpha = 0.1f;

    private NeuronNetVisual neuronNetObj;
    private GameObject NNPrefab;

    public Element[] allElements;

    private Analyzer analyzer;

    public delegate void AIDecisionHandler(Element element);
    public AIDecisionHandler OnAIDecision;

    [ContextMenu("Make Decision")]
    public void MakeDecision()
    {
        Element decision;
        neuronNetObj.NeuronNet.Action(analyzer.GetAnalyzedData());
        int index = Program.SoftmaxToClassIndex(Program.Softmax(neuronNetObj.NeuronNet.GetRawAnswers()));
        index = Mathf.Clamp(index, 0, allElements.Length);
        decision = allElements[index];
        //decision = RandomElement();
        OnAIDecision?.Invoke(decision);
    }

    public void Train(float[] ans)
    {
        NeuronNet net = neuronNetObj.NeuronNet;

        float error = Program.CrossEntropy(Program.Softmax(net.GetRawAnswers()), ans);

        for (int i = net.NeuronLevels.Count - 1; i >= 1; i--)
        {
            if (net.NeuronLevels[i].IsLastLevel)
            {
                net.NeuronLevels[i].CalculateDeltasForLastLevel(ans, error, true);
            }
            else
            {
                net.NeuronLevels[i].CalculateDeltas(net.NeuronLevels[i + 1]);
            }
        }
        foreach (NeuronLevel level in net.NeuronLevels)
        {
            level.UpdateWeights(alpha);
        }
    }

    private Element RandomElement()
    {
        int i = UnityEngine.Random.Range(0, allElements.Length);
        return allElements[i];
    }

    private void Start()
    {
        neuronNetObj = Instantiate(NNPrefab, neuronNetPos.position, Quaternion.identity).GetComponent<NeuronNetVisual>();
        neuronNetObj.InitNeuronNet(NLNeuronsCount.Length, NLNeuronsCount);
    }

    private void Awake()
    {
        NNPrefab = Resources.Load<GameObject>("Prefabs/NeuronNet");

        analyzer = GetComponent<Analyzer>();
    }
}
