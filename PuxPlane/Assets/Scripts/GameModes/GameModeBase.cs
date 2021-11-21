using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameModeBase : MonoBehaviour
{
    // Generic game functions
    public virtual void OnMainMenu() { }
    public virtual void OnRestart() { }

    // Game specific functions
    public virtual void OnPlaneHit() { }
    public virtual void OnObstaclePassed() { }
    public virtual void OnObstacleOffScreen(Obstacle obstacle) { }
    public virtual void OnStarCollected() { }


    // GameModeBase functions
    protected virtual void Awake()
    {
        this.GetGameManager().SetGameMode(this);
    }

    protected virtual void OnDestroy()
    {
        if (GameManager.Exists)
            this.GetGameManager().SetGameMode(null);
    }
}
