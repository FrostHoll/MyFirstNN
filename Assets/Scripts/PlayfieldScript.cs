using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayfieldScript : MonoBehaviour
{
    [SerializeField] private Image playerImage;

    [SerializeField] private Image aiImage;

    [SerializeField] private AIManager aiManager;

    [SerializeField] private Player player;

    [SerializeField] private Sprite defaultElementSprite;

    [SerializeField] private Text playerText;

    [SerializeField] private Text aiText;

    [SerializeField] private Text scoreText;

    [SerializeField] private int maxScore = 3;

    private Element playerChoice;

    private Element aiChoice;

    private bool CanProcessResult => playerChoice != null && aiChoice != null;

    private bool isPlayerChose = false;
    private bool isAIChose = false;

    public bool IsPlayerChose => isPlayerChose;
    public bool IsAIChoice => isAIChose;

    private int playerScore = 0;
    private int aiScore = 0;

    public delegate void GameResultHandler(GameResult playerResult, Element player, GameResult aiResult, Element ai);
    public GameResultHandler OnGameResult;

    public delegate void MatchResetHandler();
    public MatchResetHandler OnMatchReset;

    public delegate void GameResetHandler(int playerScore, int aiScore);
    public GameResetHandler OnGameReset;

    private void Start()
    {
        if (aiManager != null) aiManager.OnAIDecision += SetAIChoice;
        if (player != null) player.OnPlayerDecision += SetPlayerChoice;
        ResetChoices();
    }

    private void OnDestroy()
    {
        if (aiManager != null) aiManager.OnAIDecision -= SetAIChoice;
        if (player != null) player.OnPlayerDecision -= SetPlayerChoice;
    }

    private void ProcessResult()
    {
        if (!CanProcessResult) return;

        GameResult playerResult = playerChoice.ResultAgainst(aiChoice);
        GameResult aiResult = aiChoice.ResultAgainst(playerChoice);

        if (playerResult == GameResult.Win) playerScore++;
        if (aiResult == GameResult.Win) aiScore++;

        playerText.text = playerResult.ToString() + "!";
        aiText.text = aiResult.ToString() + "!";

        scoreText.text = $"{playerScore} : {aiScore}";

        OnGameResult?.Invoke(playerResult, playerChoice, aiResult, aiChoice);
    }

    public void SetPlayerChoice(Element element)
    {
        if (isPlayerChose) return;

        playerChoice = element;
        playerImage.sprite = playerChoice.sprite;
        isPlayerChose = true;
    }

    public void SetAIChoice(Element element)
    {
        if (isAIChose) return;

        aiChoice = element;
        aiImage.sprite = aiChoice.sprite;
        isAIChose = true;

        ProcessResult();
    }

    public void ResetChoices()
    {
        OnMatchReset?.Invoke();
        playerChoice = null;
        aiChoice = null;
        playerImage.sprite = defaultElementSprite;
        aiImage.sprite = defaultElementSprite;
        isPlayerChose = false;
        isAIChose = false;
        playerText.text = "";
        aiText.text = "";
        if (playerScore >= maxScore || aiScore >= maxScore)
        {
            OnGameReset?.Invoke(playerScore, aiScore);
            playerScore = 0;
            aiScore = 0;
            scoreText.text = $"{playerScore} : {aiScore}";
        }
    }

    private void OnMouseDown()
    {
        if (!CanProcessResult) return;

        ResetChoices();
    }
}
