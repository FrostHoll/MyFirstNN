using System;
using System.Collections.Generic;
using UnityEngine;

public class NeuronLevel : MonoBehaviour
{
    private int neuronsCount;

    private bool isLastLvl = false;
    
    private List<Neuron> neurons;

    private NeuronLevel prevNL;   

    public void Init(int neuronsCount, List<Neuron> neuronsList, NeuronLevel previousNeuronLevel, bool lastLevel = true)
    {
        this.neuronsCount = neuronsCount;
        prevNL = previousNeuronLevel;
        isLastLvl = lastLevel;

        //neurons = new List<Neuron>(neuronsCount);
        neurons = neuronsList;
    }

    public float[] NeuronValues
    {
        get
        {
            float[] values = new float[neurons.Count];
            for (int i = 0; i < neurons.Count; i++)
            {
                values[i] = neurons[i].Weight;
            }
            return values;
        }
        set
        {
            if (value.Length != neuronsCount)
            {
                Debug.LogError("Error: wrong input count");
                return;
            }
            for (int i = 0; i < neurons.Count; i++)
            {
                neurons[i].Weight = value[i];
            }
        }
    }

    public float[] NeuronValuesRaw
    {
        get
        {
            float[] values = new float[neurons.Count];
            for (int i = 0; i < neurons.Count; i++)
            {
                values[i] = neurons[i].WeightRaw;
            }
            return values;
        }
    }

    public Neuron GetNeuronByIndex(int index)
    {
        return neurons[index];
    }

    public List<Neuron> NeuronList
    {
        get { return neurons; }
    }

    public bool IsLastLevel => isLastLvl;

    public void Action(float[] inputs)
    {
        for (int i = 0; i < neurons.Count; i++)
        {
            neurons[i].Action(inputs);
        }
    }

    #region BackProp
    // dE / dh
    [SerializeField] private float[] outsDeltas = null;
    // dE / dz
    [SerializeField] private float[] valuesRawDeltas = null;

    public float[] OutsDeltas => outsDeltas;
    public float[] ValuesRawDeltas => valuesRawDeltas;

    public void CalculateDeltas(NeuronLevel nextNL)
    {

        // outs deltas
        outsDeltas = new float[neuronsCount];

        for (int i = 0; i < neuronsCount; i++)
        {
            outsDeltas[i] = nextNL.CalculateOutDelta(i);
        }

        // raw deltas
        valuesRawDeltas = new float[neuronsCount];

        for (int i = 0; i < neuronsCount; i++)
        {
            valuesRawDeltas[i] = outsDeltas[i] * Program.ActivationFuncDeriv(NeuronValuesRaw[i]);
        }

        NeuronsCalculateDeltas();
    }

    public void CalculateDeltasForLastLevel(float[] ans, float err = -1f, bool useSoftmax = false)
    {
        valuesRawDeltas = new float[neuronsCount];

        if (!useSoftmax)
        {
            for (int i = 0; i < neuronsCount; i++)
            {
                valuesRawDeltas[i] = (2 * (NeuronValues[i] - ans[i]) / neuronsCount) * Program.SigmoidDeriv(NeuronValues[i]);
            }
        }
        else
        {
            float[] t = Program.Softmax(NeuronValuesRaw);
            for (int i = 0; i < neuronsCount; i++)
            {
                valuesRawDeltas[i] = t[i] - ans[i];
            }
        }

        

        NeuronsCalculateDeltas();
    }

    public float CalculateOutDelta(int neuronIndex)
    {
        float res = 0f;

        for (int i = 0; i < neuronsCount; i++)
        {
            res += valuesRawDeltas[i] * neurons[i].GetWeightByIndex(neuronIndex);
        }
        return res;
    }

    public void UpdateWeights(float alpha)
    {
        if (valuesRawDeltas == null)
        {
            return;
        }

        foreach (Neuron neuron in neurons)
        {
            neuron.UpdateWeights(alpha);
        }

        outsDeltas = null;
        valuesRawDeltas = null;
    }

    private void NeuronsCalculateDeltas()
    {
        for (int i = 0; i < neuronsCount; i++)
        {
            neurons[i].CalculateDeltas(valuesRawDeltas[i], prevNL);
        }
    }
    #endregion
}