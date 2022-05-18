using UnityEngine;

public class Neuron : MonoBehaviour
{
    private float currWeight = 0f;

    private float weightRaw = 0f;

    private float[] weights;

    private float bias = 0f;

    private NeuronLevel parent;

    public void Init(int weightsCount)
    {
        OnValueChanged?.Invoke(currWeight);
        if (weightsCount == 0) return;

        weights = new float[weightsCount];
        
        for (int i = 0; i < weightsCount; i++)
        {
            weights[i] = Program.GetRandomWeight();           
        }
        bias = Program.GetRandomWeight();
        OnWeightChanged?.Invoke(weights, bias);
    }

    public float Weight
    {
        get => currWeight;
        set
        {
            currWeight = value;
            OnValueChanged?.Invoke(currWeight);
        }
    }

    public float WeightRaw
    {
        get => weightRaw;
    }

    public float Bias
    {
        get => bias;
        set
        {
            bias = value;
        }
    }

    public float GetWeightByIndex(int index)
    {
        return weights[index];
    }

    public delegate void WeightChangeHandler(float[] weights, float bias);
    public delegate void ValueChangeHandler(float value);
    public WeightChangeHandler OnWeightChanged;
    public ValueChangeHandler OnValueChanged;

    public void Action(float[] input)
    {
        weightRaw = 0f;

        if (weights.Length != input.Length)
        {
            Debug.LogError("Error: wrong input count");
            return;
        }

        for (int i = 0; i < weights.Length; i++)
        {
            weightRaw += input[i] * weights[i];
        }

        weightRaw += bias;

        currWeight = parent.IsLastLevel ? Program.Sigmoid(weightRaw) : Program.ActivationFunc(weightRaw);

        OnValueChanged?.Invoke(currWeight);
    }

    #region BackProp

    [SerializeField] private float[] weightDeltas = null;
    [SerializeField] private float biasDelta;

    public void CalculateDeltas(float valueRawDelta, NeuronLevel prevNL)
    {
        if (weights == null) return;

        biasDelta = valueRawDelta;
        weightDeltas = new float[weights.Length];

        for (int i = 0; i < weights.Length; i++)
        {
            weightDeltas[i] = prevNL.NeuronValues[i] * valueRawDelta;
        }
    }

    public void UpdateWeights(float alpha)
    {
        if (weights == null) return;

        if (weightDeltas == null)
        {
            Debug.Log("warn", this);
            return;
        }

        //string s = bias + " " + weights[0];

        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] -= alpha * weightDeltas[i]; 
        }
       
        bias -= alpha * biasDelta;

        //s += " = " + bias + " " + weights[0];

        //Debug.Log(s, this);

        OnWeightChanged?.Invoke(weights, bias);

        weightDeltas = null;
        biasDelta = 0f;
    }

    #endregion

    private void Awake()
    {        
        parent = transform.parent.gameObject.GetComponent<NeuronLevel>();
    }
}
