using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarVisual : MonoBehaviour
{
    [SerializeField] Image[] signs;
    
    public void UpdateSigns(float[] input)
    {
        if (input.Length != signs.Length) return;

        for (int i = 0; i < signs.Length; i++)
        {
            Color c = Color.white;
            c.a = input[i] == 1f ? 1f : 0.2f;
            signs[i].color = c;
        }
    }

}
