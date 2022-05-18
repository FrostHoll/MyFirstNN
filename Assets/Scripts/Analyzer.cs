using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Analyzer : MonoBehaviour
{
    [SerializeField] private PlayfieldScript playfield;

    [SerializeField] private bool isTraining = true;

    private AIManager aiManager;

    private const int c_dataIndexCount = 18;

    private Deque<Match> matches = new Deque<Match>(3);

    private float[] data = new float[c_dataIndexCount];

    private void Update()
    {
        if (playfield.IsPlayerChose && !playfield.IsAIChoice)
        {
            aiManager.MakeDecision();
        }
    }

    public float[] GetAnalyzedData()
    {
        for (int i = 0; i < c_dataIndexCount; i++)
        {
            data[i] = 0f;
        }

        Match[] m = matches.GetDeque();

        for (int i = 0; i < m.Length; i++)
        {
            if (m[i].IsEmpty) continue;
            int j = i * 6 + m[i].element.id;
            data[j] = 1f;
            j = i * 6 + 3 + (int)m[i].gameResult + 1;
            data[j] = 1f;
        }
        return data;
    }



    private struct Match
    {
        public Element element;
        public GameResult gameResult;

        public bool IsEmpty => element == null;
    }

    private class Deque<T>
    {
        private T[] mas;

        private int count = 0;
        private int maxSize = 0;

        public T[] GetDeque() => mas;

        public int Length => maxSize;

        public Deque(int maxSize)
        {
            this.maxSize = maxSize;
            mas = new T[maxSize];
        }

        public void Clear()
        {
            mas = null;
            mas = new T[maxSize];
            count = 0;
        }

        public void Add(T item)
        {
            if (count < maxSize)
            {
                mas[count] = item;
                count++;
            }
            else
            {
                for (int i = 0; i < maxSize - 1; i++)
                {
                    mas[i] = mas[i + 1];
                }
                mas[maxSize - 1] = item;
            }
        }
    }

    private void AnalyzeMatch(GameResult plRes, Element plElem, GameResult aiRes, Element aiElem)
    {
        Match match = new Match() { element = plElem, gameResult = plRes };
        matches.Add(match);
      
        if (isTraining)
        {
            float[] ans = new float[3] { 0, 0, 0 };
            int ind = plElem.loseElement.id;
            ans[ind] = 1f;
            aiManager.Train(ans);
        }
    }

    private void AnalyzeGame(int plScore, int aiScore)
    {
        matches.Clear();
    }

    private void Start()
    {
        if (playfield != null)
        {
            playfield.OnGameReset += AnalyzeGame;
            playfield.OnGameResult += AnalyzeMatch;
        }
    }

    private void OnDestroy()
    {
        if (playfield != null)
        {
            playfield.OnGameReset -= AnalyzeGame;
            playfield.OnGameResult -= AnalyzeMatch;
        }
    }

    private void Awake()
    {
        aiManager = GetComponent<AIManager>();
    }
}
