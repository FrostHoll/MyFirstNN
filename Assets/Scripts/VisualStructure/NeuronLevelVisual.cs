using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronLevelVisual : MonoBehaviour
{
    public NeuronLevel NeuronLevel => neuronLevel;

    private List<NeuronVisual> neuronVisuals;

    [SerializeField] private float neuronPosOffset = 1.5f;

    private GameObject neuronPrefab;

    private GameObject biasPrefab;

    private GameObject biasObj;

    private NeuronLevelVisual prevNL;

    private NeuronLevel neuronLevel;


    public void InitNeuronLevel(int neuronsCount, int inputCount, NeuronLevelVisual previousNeuronLevelVisual, bool isLastLevel = true)
    {
        prevNL = previousNeuronLevelVisual;
        neuronVisuals = new List<NeuronVisual>(neuronsCount);

        List<Neuron> neurons = new List<Neuron>(neuronsCount);

        if (!isLastLevel)
        {
            biasObj = Instantiate(biasPrefab, transform.position + neuronPosOffset * Vector3.up, Quaternion.identity, transform);
        }

        for (int i = 0; i < neuronsCount; i++)
        {
            CreateNeuron(transform.position + i * neuronPosOffset * Vector3.down, inputCount);
            neurons.Add(neuronVisuals[i].Neuron);
        }

        neuronLevel.Init(neuronsCount, neurons, prevNL == null ? null : prevNL.neuronLevel, isLastLevel);
    }

    public NeuronVisual GetNeuronVisualByIndex(int index)
    {
        return neuronVisuals[index];
    }

    public Transform GetBiasObj()
    {
        return biasObj.transform;
    }

    private void CreateNeuron(Vector3 position, int inputCount)
    {
        NeuronVisual neuron;
        neuron = Instantiate(neuronPrefab, position, Quaternion.identity, transform).GetComponent<NeuronVisual>();
        neuron.InitNeuron(inputCount, prevNL);
        neuronVisuals.Add(neuron);
        neuron.gameObject.name += neuronVisuals.Count.ToString();
    }

    private void Awake()
    {
        neuronPrefab = Resources.Load<GameObject>("Prefabs/Neuron");
        biasPrefab = Resources.Load<GameObject>("Prefabs/Bias");

        neuronLevel = GetComponent<NeuronLevel>();
    }

}
