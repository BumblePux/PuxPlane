using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverMenu : HUD
{
    [Header("References")]
    [SerializeField]
    private TMP_Text scoreText;


    public void OnChangeScore(int newScore)
    {
        scoreText.SetText("SCORE: " + newScore.ToString());
    }
}
