using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    private NeuronNet net;
    private TrainSet tS;
    private TextLog log;
    private Set[] trainSets;

    [SerializeField] int epochs = 0;

    [SerializeField] float alpha = 0f;

    public void Init(NeuronNet net)
    {
        this.net = net;
    }

    private void Awake()
    {
        tS = GetComponent<TrainSet>();
        if (!tS.IsTrainSetLoaded)
        {
            tS.ForceLoad();
        }
        trainSets = tS.TrainSets;
        log = GetComponent<TextLog>();
    }

    [ContextMenu("Test")]
    public void Test()
    {
        StartTrain(epochs, alpha);
    }

    public void StartTrain(int epochs, float alpha)
    {
        int k = 0;
        int kmax = trainSets.Length * epochs;

        float startError = 0f;
        float endError = 0f;

        log.LogClear();
        log.LogAny($"Started training with {epochs} epochs and with {alpha} learning rate.");
        for (int ep = 0; ep < epochs; ep++)
        {
            float epochError = 0f;

            //log.LogAny($"Epoch {ep}");

            for (int setIndex = 0; setIndex < trainSets.Length; setIndex++)
            {
                Set set = trainSets[setIndex];

                net.Action(set.input);

                float error = Program.MSE(net.GetAnswer(), set.answer);
                epochError += error;

                //log.LogAny($"set{setIndex + 1} error: {error}");

                for (int i = net.NeuronLevels.Count - 1; i >= 1; i--)
                {
                    if (net.NeuronLevels[i].IsLastLevel)
                    {
                        net.NeuronLevels[i].CalculateDeltasForLastLevel(set.answer, error);
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
                k++;
                log.SetProgressUI(k / kmax);
            }
            //log.LogAny($"Overall epoch error: {epochError / trainSets.Length}");

            if(ep == 0)
            {
                startError = epochError / trainSets.Length;
                log.LogAny($"Start error: {startError}");
            }
            if (ep == epochs - 1)
            {
                endError = epochError / trainSets.Length;
                log.LogAny("Training finished.");
                log.LogAny($"Finish error: {endError}");
            }

        }
        log.LogAny($"Error change: {(endError / startError - 1f) * 100}%");
    }
}


