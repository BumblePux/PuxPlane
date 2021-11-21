using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : Singleton<MapManager>
{
    [Header("Scroll")]
    [SerializeField]
    private bool scrollOnAwake = true;
    [SerializeField]
    private bool scrollSpeedOverride = false;
    [SerializeField]
    private float foregroundOverrideSpeed = 5f;
    [SerializeField]
    private float backgroundOverrideSpeed = 1.25f;

    [Header("References")]
    [SerializeField]
    private ScrollingSprite ground;
    [SerializeField]
    private ScrollingSprite ceiling;
    [SerializeField]
    private ScrollingSprite background;

    // Scroll speeds
    private float _groundSpeed;
    private float _ceilingSpeed;
    private float _backgroundSpeed;

    // State
    private bool _isScrolling = false;


    protected override void OnSingletonAwake()
    {
        // Register for MapTheme changes
        this.GetGameManager().OnThemeChanged += OnChangeTheme;

        // Force theme change, in case the current theme is different from GameManager selection
        OnChangeTheme(this.GetGameManager().MapTheme);

        // Get default speed from objects.
        _groundSpeed = ground.Speed;
        _ceilingSpeed = ceiling.Speed;
        _backgroundSpeed = background.Speed;

        // Setup startup scrolling
        if (scrollOnAwake)
            StartScrolling();
        else
            StopScrolling();
    }

    private void Update()
    {
        // Set scroll speed overrides
        if (scrollSpeedOverride && _isScrolling)
        {
            ground.Speed = foregroundOverrideSpeed;
            ceiling.Speed = foregroundOverrideSpeed;
            background.Speed = backgroundOverrideSpeed;
        }
    }

    public void StartScrolling()
    {
        ground.Speed = _groundSpeed;
        ceiling.Speed = _ceilingSpeed;
        background.Speed = _backgroundSpeed;

        _isScrolling = true;
    }

    public void StopScrolling()
    {
        ground.Speed = 0f;
        ceiling.Speed = 0f;
        background.Speed = 0f;

        _isScrolling = false;
    }

    private void OnChangeTheme(MapThemeData theme)
    {
        ground.GetComponent<SpriteRenderer>().sprite = theme.ground;
        ceiling.GetComponent<SpriteRenderer>().sprite = theme.ground;
        background.GetComponent<SpriteRenderer>().color = theme.backgroundTint;
    }

    protected override void OnSingletonDestroy()
    {
        // Unregister for MapTheme changes
        if (GameManager.Exists)
            this.GetGameManager().OnThemeChanged -= OnChangeTheme;
    }
}
