using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : HUD
{
    [Header("References")]
    [SerializeField]
    private TMP_Text versionText;
    [SerializeField]
    private TMP_Text themeText;
    [SerializeField]
    private TMP_Text highscoreText;


    private void Awake()
    {
        // Set version text
        versionText.SetText("v" + Application.version);

        // Set current theme text
        themeText.SetText(this.GetGameManager().MapTheme.name);

        // Set highscore text
        if (this.GetGameManager().GameData.classicModeHighscore > 0)
            highscoreText.SetText("Highscore: " + this.GetGameManager().GameData.classicModeHighscore.ToString());
        else
            highscoreText.SetText(string.Empty);
    }

    public void OnPlay()
    {
        // Show game mode select screen

        // For now, just load Classic Mode
        SceneManager.Instance.LoadScene("ClassicGameMode");
    }

    public void OnChangePlaneColour(int direction)
    {
        // GameManager is loaded at runtime, hence why this wrapper exists.
        this.GetGameManager().OnChangePlaneColour(direction);
    }

    public void OnChangeTheme(int direction)
    {
        // GameManager is loaded at runtime, hence why this wrapper exists.
        this.GetGameManager().OnChangeTheme(direction);
        themeText.SetText(this.GetGameManager().MapTheme.name);
    }
}
