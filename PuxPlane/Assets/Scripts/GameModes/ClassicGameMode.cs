using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicGameMode : GameModeBase
{
    public enum EGameModeState
    {
        Setup,
        Playing,
        GameOver
    }

    [Header("References")]
    [SerializeField]
    private PlaneController plane;
    [SerializeField]
    private MapManager map;
    [SerializeField]
    private ScoreHUD scoreUI;
    [SerializeField]
    private GameOverMenu gameOverUI;
    [SerializeField]
    private GameObject howToTextUI;
    [SerializeField]
    private AudioSource scoreAudio;

    [Header("Game Mode Settings")]
    [SerializeField]
    private float gameStartDelay = 2f;
    [SerializeField]
    private float gameOverDelay = 2f;

    [Header("Obstacle Settings")]
    [SerializeField]
    private Obstacle obstaclePrefab;
    [SerializeField]
    private int obstaclePoolSize = 10;
    [SerializeField]
    private float obstacleSpawnDelay = 1f;

    [Header("State")]
    [SerializeField]
    private EGameModeState gameState;

    // Pools
    private ObjectPool<Obstacle> _obstaclePool;

    // Obstacle
    private float _nextObstacleSpawnTime;
    private bool _spawnObstacles;

    // Game mode data
    private int _score = 0;


    private IEnumerator Start()
    {
        // Create Pool
        _obstaclePool = new ObjectPool<Obstacle>(() =>
        {
            return Instantiate(obstaclePrefab);
        },
        (obstacle) =>
        {
            obstacle.gameObject.SetActive(true);
        },
        (obstacle) =>
        {
            obstacle.gameObject.SetActive(false);
        },
        (obstacle) =>
        {
            Destroy(obstacle.gameObject);
        },
        obstaclePoolSize
        );


        // Set starting params
        gameState = EGameModeState.Setup;
        _score = 0;

        // Reset score UI
        scoreUI.OnResetScore();

        // Hide GameOver menu
        gameOverUI.Hide();

        // Wait for user input to begin game
        while (!plane.GetInput())
        {
            yield return null;
        }

        // Game begun! Set params
        gameState = EGameModeState.Playing;

        // Hide How To text
        howToTextUI.SetActive(false);

        // Set plane state
        plane.SetPlaneState(PlaneController.EPlaneState.Playing);

        // Start spawning obstacles
        StartSpawningObstacles();
    }

    public override void OnMainMenu()
    {
        // Load MainMenu
        SceneManager.Instance.LoadScene("MainMenu");
    }

    public override void OnRestart()
    {
        // There's no transient data in a game mode, so just reload the current scene
        SceneManager.Instance.ReloadCurrentScene();
    }

    public override void OnPlaneHit()
    {
        // Don't do anything if already game over
        if (gameState == EGameModeState.GameOver)
            return;

        GameOver();
    }

    private void GameOver()
    {
        // Plane has hit something! Game Over!
        gameState = EGameModeState.GameOver;

        // Set plane state
        plane.SetPlaneState(PlaneController.EPlaneState.Crashed);
        plane.SetEngineAudioActive(false);

        // Stop scrolling
        map.StopScrolling();
        StopSpawningObstacles();
        StopAllObstacles();

        // Set score on GameOver menu
        gameOverUI.OnChangeScore(_score);

        // Show GameOver menu
        gameOverUI.Show();

        // Save score if new best
        if (this.GetGameManager().GameData.classicModeHighscore < _score)
            this.GetGameManager().GameData.classicModeHighscore = _score;
    }

    public override void OnObstaclePassed()
    {
        // Plane may have hit trigger when already crashed.
        if (gameState == EGameModeState.GameOver)
            return;

        // Passed an obstacle! Get a point!
        _score++;

        // Update Score UI
        scoreUI.OnChangeScore(_score);

        // Play sound
        scoreAudio.Play();
    }

    public override void OnObstacleOffScreen(Obstacle obstacle)
    {
        _obstaclePool.Release(obstacle);
    }

    private void Update()
    {
        // Spawn obstacles from pool
        if (gameState == EGameModeState.Playing && _spawnObstacles && Time.time > _nextObstacleSpawnTime)
        {
            _nextObstacleSpawnTime = Time.time + obstacleSpawnDelay;
            _obstaclePool.Get();
        }
    }

    private void StartSpawningObstacles()
    {
        _spawnObstacles = true;
        _nextObstacleSpawnTime = Time.time + gameStartDelay;
    }

    private void StopSpawningObstacles()
    {
        _spawnObstacles = false;
    }

    private void StopAllObstacles()
    {
        var activeObstacles = _obstaclePool.GetAllActive();
        foreach (var obstacle in activeObstacles)
        {
            obstacle.Speed = 0f;
        }
    }
}
