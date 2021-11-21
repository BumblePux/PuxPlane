using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MonoBehaviourExtensions
{
    public static GameManager GetGameManager(this MonoBehaviour monoBehaviour)
    {
        return GameManager.Instance;
    }

    public static GameModeBase GetGameMode(this MonoBehaviour monoBehaviour)
    {
        return GameManager.Instance.GameMode;
    }

    public static GameSettingsData GetGameSettings(this MonoBehaviour monoBehaviour)
    {
        return GameManager.Instance.GameSettings;
    }

    public static GameSessionData GetSessionData(this MonoBehaviour monoBehaviour)
    {
        return GameManager.Instance.GameData;
    }
}
