using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Element", fileName = "NewElement")]
public class Element : ScriptableObject
{
    public int id = 0;

    public string displayName;

    public Sprite sprite;

    public Element loseElement;
    public Element tieElement;
    public Element winElement;

    

    public GameResult ResultAgainst(Element elem)
    {
        if (elem == winElement)
        {
            return GameResult.Win;
        }
        else if (elem == tieElement)
        {
            return GameResult.Tie;
        }
        else
        {
            return GameResult.Lose;
        }
    }

}

public enum GameResult
{
    Win = 1,
    Tie = 0,
    Lose = -1
}
