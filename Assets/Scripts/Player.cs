using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public delegate void PlayerDecisionHandler(Element element);
    public PlayerDecisionHandler OnPlayerDecision;

    private bool uiToogled = true;

    public bool IsUIToogled => uiToogled;

    private GameObject[] uiElements;

    [SerializeField] PlayfieldScript playfield;

    public void SetChoice(Element element)
    {
        ToogleUI();
        OnPlayerDecision?.Invoke(element);
    }

    public void ToogleUI()
    {
        uiToogled = !uiToogled;
        foreach (var obj in uiElements)
        {
            obj.SetActive(uiToogled);
        }
    }

    private void ResetMatch()
    {
        ToogleUI();
    }

    private void ProcessResult(GameResult plRes, Element plElem, GameResult aiRes, Element aiElem)
    {
        
    }

    private void Start()
    {
        if (playfield != null)
        {
            playfield.OnMatchReset += ResetMatch;
            playfield.OnGameResult += ProcessResult;
        }

        uiElements = new GameObject[transform.childCount];
        for (int i = 0; i < uiElements.Length; i++)
            uiElements[i] = transform.GetChild(i).gameObject;
    }

    private void OnDestroy()
    {
        if (playfield != null)
        {
            playfield.OnMatchReset -= ResetMatch;
            playfield.OnGameResult -= ProcessResult;
        }
    }
}
