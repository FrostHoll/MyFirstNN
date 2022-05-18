using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataConstruct : MonoBehaviour
{
    [SerializeField] private string dataFileName = "Data";
    [SerializeField] private string fileExtension = ".csv";

    private string DataPath
    {
        get
        {
            return FileManager.SavPath + "Resources/Data/" + dataFileName + fileExtension;
        }
    }


    [SerializeField] List<string> dataList = new List<string>();

    public Set[] GetSets()
    {
        if (dataList.Count < 1)
        {
            LoadDataSet();
        }
        return FormatCSVToSet();
    }

    private void LoadDataSet()
    {
        dataList = FileManager.LoadFile(DataPath);
    }

    private Set[] FormatCSVToSet()
    {
        int inputCount = 0;
        int outputCount = 0;
        string[] header = dataList[0].Split(',');
        bool isInputReading = false;
        for (int i = 0; i < header.Length; i++)
        {
            switch (header[i])
            {
                case "Input":
                    inputCount++;
                    isInputReading = true;
                    break;
                case "Output":
                    outputCount++;
                    isInputReading = false;
                    break;
                default:
                    if (isInputReading)
                    {
                        inputCount++;
                    }
                    else
                    {
                        outputCount++;
                    }
                    break;
            }
        }

        Set[] res = new Set[dataList.Count - 1];

        for (int i = 0; i < res.Length; i++)
        {
            res[i] = new Set(new float[inputCount], new float[outputCount]);
            string[] s = dataList[i + 1].Split(',');
            for (int k = 0; k < s.Length; k++)
            {
                if (k < inputCount)
                {
                    res[i].input[k] = Convert.ToSingle(s[k]);
                }
                else
                {
                    res[i].answer[k - inputCount] = Convert.ToSingle(s[k]);
                }
            }
        }

        return res;

    }
}
