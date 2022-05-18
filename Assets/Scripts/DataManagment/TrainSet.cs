using UnityEngine;

public class TrainSet : MonoBehaviour
{
    private Set[] trainSets;

    private int index = 0;

    public Set[] TrainSets
    {
        get
        {
            if(IsTrainSetLoaded)
            {
                return trainSets;
            }
            return null;
        }
    }

    public int TrainSetIndex => index;

    public bool IsTrainSetLoaded => trainSets != null && trainSets.Length > 0;

    public bool TryGetNextSet(out Set? set)
    {
        if(!IsTrainSetLoaded)
        {
            set = null;
            return false;
        }

        if (index < trainSets.Length)
        {
            set = trainSets[index];
            index++;
            return true;
        }
        set = null;
        index = 0;
        return false;
    }

    public void ForceLoad()
    {
        DataConstruct dataConstruct = GetComponent<DataConstruct>();
        if (dataConstruct != null)
        {
            trainSets = dataConstruct.GetSets();
        }
    }
}

[System.Serializable]
public struct Set
{
    public float[] input;

    public float[] answer;

    public Set(float[] _input, float[] _answer)
    {
        input = _input;
        answer = _answer;
    }
}
