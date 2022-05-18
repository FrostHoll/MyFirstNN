using System;
using UnityEngine;

public static class Program
{
    /// <summary>
    /// Минимальное значение нейрона
    /// </summary>
    public static float MinNeuronValue = 0f;
    /// <summary>
    /// Максимальное значение нейрона
    /// </summary>
    public static float MaxNeuronValue = 1f;

    /// <summary>
    /// Минимальное значение перцептрона
    /// </summary>
    public static float MinWeightValue = -1f;
    /// <summary>
    /// Максимальное значение перцептрона
    /// </summary>
    public static float MaxWeightValue = 1f;

    public static bool UseReLU = false;

    public static float GetRandomWeight()
    {
        return UnityEngine.Random.Range(MinWeightValue, MaxWeightValue);
    }

    public static float ActivationFunc(float value)
    {
        return UseReLU ? ReLU(value) : Sigmoid(value);
    }

    public static float ActivationFuncDeriv(float value)
    {
        return UseReLU ? ReLUDeriv(value) : SigmoidDeriv(value);
    }

    public static float ReLU(float value)
    {
        return Mathf.Max(value, 0f);
    }

    public static float Sigmoid(float value)
    {
        return 1 / (1 + (float)Math.Exp(-value));
    }

    public static float[] Softmax(float[] values)
    {
        float k = 0f;
        foreach (float item in values) k += Mathf.Exp(item);

        float[] res = new float[values.Length];
        for (int i = 0; i < res.Length; i++)
        {
            res[i] = Mathf.Exp(values[i]) / k;
        }
        return res;
    }

    public static int SoftmaxToClassIndex(float[] outs)
    {
        int _max = 0;
        for (int i = 0; i < outs.Length; i++)
        {
            if (outs[i] > outs[_max])
            {
                _max = i;
            }
        }
        return _max;
    }

    public static float CrossEntropy(float[] outs, float[] ans)
    {
        float res = 0f;
        for (int i = 0; i < outs.Length; i++)
        {
            res += (ans[i] * Mathf.Log10(outs[i]) + (1 - ans[i]) * Mathf.Log10(1 - outs[i]));
        }
        return -res;
    }

    public static float MSE(float[] outs, float[] ans, bool root = false)
    {
        float res = 0f;

        for (int i = 0; i < outs.Length; i++)
        {
            res += Mathf.Pow(outs[i] - ans[i], 2);
        }

        return root? Mathf.Sqrt(res / outs.Length) : res / outs.Length;
    }

    public static float SigmoidDeriv(float value)
    {
        return Sigmoid(value) * (1 - Sigmoid(value));
    }

    public static float ReLUDeriv(float value)
    {
        return (value >= 0) ? 1f : 0f;
    }

    /// <summary>
    /// Изменение цвета нейрона в зависимости от его значения
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Color NeuronColorChange(float value)
    {
        float offset = MinNeuronValue < 0f ? Math.Abs(MinNeuronValue) : 0f;
        float percent = (value + offset) / (MaxNeuronValue + offset);
        return new Color(percent, percent, percent);
    }

    /// <summary>
    /// Изменение цвета перцептрона в зависимости от его значения
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Color WeightColorChange(float value)
    {
        float offset = MinWeightValue < 0f ? Math.Abs(MinWeightValue) : 0f;
        float percent = (value + offset) / (MaxWeightValue + offset);
        return Color.Lerp(Color.red, Color.green, percent);
    }
}
