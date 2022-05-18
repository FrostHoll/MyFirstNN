using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuronVisual : MonoBehaviour
{
    public Neuron Neuron => neuron;

    private Neuron neuron;

    private SpriteRenderer spriteRenderer;

    private Line[] lines;

    private Line biasLine;

    private GameObject linePrefab;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        linePrefab = Resources.Load<GameObject>("Prefabs/Line");

        neuron = GetComponent<Neuron>();
        if (neuron != null)
        {
            neuron.OnValueChanged += OnNeuronValueChanged;
            neuron.OnWeightChanged += OnNeuronWeightChanged;
        }
    }

    private void OnDestroy()
    {
        neuron.OnValueChanged -= OnNeuronValueChanged;
        neuron.OnWeightChanged -= OnNeuronWeightChanged;
    }

    public void InitNeuron(int weightsCount, NeuronLevelVisual prevNL)
    {
        if (weightsCount == 0) return;

        lines = new Line[weightsCount];
        for (int i = 0; i < weightsCount; i++)
        {
            lines[i] = Instantiate(linePrefab).GetComponent<Line>();
            lines[i].SetLine(transform.position, prevNL.GetNeuronVisualByIndex(i).transform.position);
        }
        biasLine = Instantiate(linePrefab).GetComponent<Line>();
        biasLine.SetLine(transform.position, prevNL.GetBiasObj().position);
        neuron.Init(weightsCount);
    }

    private void OnNeuronWeightChanged(float[] weights, float bias)
    {

        for (int i = 0; i < lines.Length; i++)
        {
            lines[i].ChangeWeightColorByValue(weights[i]);
        }
        biasLine.ChangeWeightColorByValue(bias);
    }

    private void OnNeuronValueChanged(float value)
    {
        spriteRenderer.color = Program.NeuronColorChange(value);
    }
}
