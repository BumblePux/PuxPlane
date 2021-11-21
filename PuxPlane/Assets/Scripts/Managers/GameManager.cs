using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public event Action<MapThemeData> OnThemeChanged;
    public event Action<PlaneData> OnPlaneColourChanged;

    [Header("Themes")]
    [SerializeField]
    private MapThemeData[] mapThemes;
    [SerializeField]
    private PlaneData[] planeColours;

    //--------------------------------------------------
    // Properties
    //--------------------------------------------------
    
    // Data
    public GameSessionData GameData { get; private set; }
    public GameSettingsData GameSettings { get; private set; }
    public GameModeBase GameMode { get; private set; }

    // Themes
    public MapThemeData MapTheme { get; private set; }
    public PlaneData PlaneColour { get; private set; }

    //--------------------------------------------------

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void OnInit()
    {
        if (!Exists)
        {
            Instantiate(Resources.Load("Managers/GameManager", typeof(GameManager))).name = "GameManager";
        }
    }

    protected override void OnSingletonAwake()
    {
        // GameManager should exist for lifetime of the game.
        isPersistent = true;

        // Game is initializing. Create data instances
        GameData = new GameSessionData();
        GameSettings = new GameSettingsData();

        // Default to 1st theme
        MapTheme = mapThemes[0];
        PlaneColour = planeColours[0];

        // TODO: Load/override data from save file here.
    }

    public void SetGameMode(GameModeBase gameMode)
    {
        if (GameMode != null)
        {
            Destroy(GameMode.gameObject);
        }

        GameMode = gameMode;
    }

    public void OnChangePlaneColour(int direction)
    {
        var currentColour = PlaneColour;
        Utils.SetToNext(planeColours, ref currentColour, direction);
        PlaneColour = currentColour;

        OnPlaneColourChanged?.Invoke(PlaneColour);
    }

    public void OnChangeTheme(int direction)
    {
        var currentTheme = MapTheme;
        Utils.SetToNext(mapThemes, ref currentTheme, direction);
        MapTheme = currentTheme;

        OnThemeChanged?.Invoke(MapTheme);
    }
}
