using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLog : MonoBehaviour
{
    [SerializeField] private Text Log;

    [SerializeField] private Slider progress;

    public void LogClear()
    {
        Log.text = "";
    }

    public void LogAny(string mes, bool newLine = true)
    {
        if (newLine)
        {
            Log.text += "\n" + mes;
        }
        else
        {
            Log.text += mes;
        }
    }

    public void SetProgressUI(float percent)
    {
        progress.value = percent;

        if (percent <= 0f)
        {
            progress.fillRect.gameObject.SetActive(false);
        }
        else
        {
            progress.fillRect.gameObject.SetActive(true);
        }
    }

    private void Awake()
    {
        SetProgressUI(0f);
    }
}
