using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreHUD : HUD
{
    [Header("References")]
    [SerializeField]
    private TMP_Text scoreText;


    public void OnChangeScore(int newScore)
    {
        scoreText.SetText(newScore.ToString());
    }

    public void OnResetScore()
    {
        scoreText.SetText("0");
    }
}
