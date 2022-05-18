using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronNet : MonoBehaviour
{  
    private List<NeuronLevel> neuronLevels; 

    private int neuronLevelCount;

    private float[] answer;

    private float[] rawAnswers;

    public void Init(int neuronLevelCount, List<NeuronLevel> neuronLevels)
    {
        this.neuronLevelCount = neuronLevelCount;

        this.neuronLevels = neuronLevels;

    }  

    public float[] GetAnswer() => answer;

    public float[] GetRawAnswers() => rawAnswers;

    public List<NeuronLevel> NeuronLevels
    {
        get { return neuronLevels; }
    }

    public void Action(float[] input)
    {
        neuronLevels[0].NeuronValues = input;
        for (int i = 1; i < neuronLevelCount; i++)
        {
            neuronLevels[i].Action(neuronLevels[i - 1].NeuronValues);
        }        
        answer = neuronLevels[neuronLevelCount - 1].NeuronValues;
        rawAnswers = neuronLevels[neuronLevelCount - 1].NeuronValuesRaw;
    }
}
