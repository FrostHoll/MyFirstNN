using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronNetVisual : MonoBehaviour
{
    public NeuronNet NeuronNet => neuronNet;

    private List<NeuronLevelVisual> neuronLevelVisuals;

    private NeuronNet neuronNet;

    private GameObject neuronLevelPrefab;

    [SerializeField] private float nlPosOffset = 3f;

    public void InitNeuronNet(int neuronLevelCount, int[] neuronLevelNeuronsCount)
    {
        neuronLevelVisuals = new List<NeuronLevelVisual>(neuronLevelCount);
        List<NeuronLevel> neuronLevels = new List<NeuronLevel>(neuronLevelCount);

        CreateNeuronLevelVisual(gameObject.transform.position, neuronLevelNeuronsCount[0], 0);
        neuronLevels.Add(neuronLevelVisuals[0].NeuronLevel);


        for (int i = 1; i < neuronLevelCount; i++)
        {
            CreateNeuronLevelVisual(gameObject.transform.position + i * nlPosOffset * Vector3.right,
                neuronLevelNeuronsCount[i], neuronLevelNeuronsCount[i - 1],
                neuronLevelVisuals[i - 1], i == neuronLevelCount - 1);
            neuronLevels.Add(neuronLevelVisuals[i].NeuronLevel);
        }

        neuronNet.Init(neuronLevelCount, neuronLevels);
    }

    private void Awake()
    {
        neuronLevelPrefab = Resources.Load<GameObject>("Prefabs/NeuronLevel");

        neuronNet = GetComponent<NeuronNet>();
    }

    private void CreateNeuronLevelVisual(Vector3 pos, int neuronCount, int inputCount, NeuronLevelVisual prevNL = null, bool lastLevel = false)
    {
        NeuronLevelVisual nl;
        nl = Instantiate(neuronLevelPrefab, pos, Quaternion.identity, gameObject.transform).GetComponent<NeuronLevelVisual>();
        nl.InitNeuronLevel(neuronCount, inputCount, prevNL, lastLevel);
        neuronLevelVisuals.Add(nl);
        nl.gameObject.name += neuronLevelVisuals.Count.ToString();
    }
}
