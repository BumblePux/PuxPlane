using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : MonoBehaviour
{
    private const string ANIM_CRASHED = "Crashed";

    public enum EPlaneState
    {
        Idle,
        Playing,
        Crashed
    }

    [Header("Input")]
    [SerializeField]
    private KeyCode[] ascendKeys = { KeyCode.Space };

    [Header("Movement")]
    [SerializeField]
    private float gravity = -30f;
    [SerializeField]
    private float ascendHeight = 1.5f;
    [SerializeField]
    private float rotateModifier = 3f;
    [SerializeField]
    private float rotateAngleLimit = 60f;

    [Header("Idle Bobbing")]
    [SerializeField]
    private float bobSpeed = 5f;        // Frequency
    [SerializeField]
    private float bobHeight = 0.15f;    // Magnitude

    [Header("Audio")]
    [SerializeField]
    private float enginePitchLimitLow = 0.8f;
    [SerializeField]
    private float enginePitchLimitHigh = 1.2f;

    [Header("Debug")]
    [SerializeField]
    private EPlaneState planeState;

    // References
    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Animator _anim;
    private AudioSource _audio;

    // Movement
    private Vector2 _startPosition;
    private Vector2 _velocity;
    private float _spriteRotation;

    // Animation
    private bool _animStopped = false;

    // Input
    private bool _ascend = false;


    public void SetPlaneState(EPlaneState state)
    {
        planeState = state;
    }

    public void SetEngineAudioActive(bool enable)
    {
        if (enable)
            _audio.Play();
        else
            _audio.Stop();
    }

    private void Awake()
    {
        // Register for PlaneColour changes. This is mainly for MainMenu previewing.
        this.GetGameManager().OnPlaneColourChanged += OnChangeColour;        

        // Get references
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponentInChildren<SpriteRenderer>();
        _anim = GetComponentInChildren<Animator>();
        _audio = GetComponent<AudioSource>();

        // Force colour change from current setting
        OnChangeColour(this.GetGameManager().PlaneColour);

        // Setup Rigidbody
        _rb.freezeRotation = true;
        _rb.gravityScale = 0f;

        // Set start position for Idle Bobbing
        _startPosition = _rb.position;

        // Set default plane state
        planeState = EPlaneState.Idle;
    }

    private void Update()
    {
        // Get input
        _ascend |= GetInput();
    }

    public bool GetInput()
    {
        foreach (var key in ascendKeys)
        {
            if (Input.GetKeyDown(key))
                return true;
        }

        return false;
    }

    private void FixedUpdate()
    {
        if (planeState == EPlaneState.Idle)
        {
            IdleBob();
        }
        else
        {
            Fly();

            if (_audio.isPlaying)
            {
                ModifyEngineAudio();
            }
        }
    }

    private void IdleBob()
    {
        // Bob up and down
        _rb.MovePosition(_startPosition + Vector2.up * (Mathf.Sin(Time.time * bobSpeed) * bobHeight));
    }

    private void Fly()
    {
        // Get current velocity
        _velocity = _rb.velocity;

        // Modify velocity
        ApplyGravity();

        // Only ascend (lift plane) if not Crashed (i.e. GameOver)
        if (planeState != EPlaneState.Crashed && _ascend)
        {
            _ascend = false;
            _velocity.y = Mathf.Sqrt(-2f * ascendHeight * gravity);
        }

        // Apply modified velocity
        _rb.velocity = _velocity;

        // Rotate sprite based on velocity
        float rotateAngle = _velocity.y * rotateModifier;
        _spriteRotation = Mathf.Clamp(rotateAngle, -rotateAngleLimit, rotateAngleLimit); // Value stored so it can be used by ModifyEngineAudio().
        _sr.transform.rotation = Quaternion.Euler(0f, 0f, _spriteRotation);

        // Handle animations
        if (planeState == EPlaneState.Crashed && !_animStopped)
        {
            _animStopped = true;
            _anim.SetTrigger(ANIM_CRASHED);
        }
    }

    private void ModifyEngineAudio()
    {
        float pitch = Utils.Map(_spriteRotation, -rotateAngleLimit, rotateAngleLimit, enginePitchLimitLow, enginePitchLimitHigh);
        _audio.pitch = pitch;
    }

    private void ApplyGravity()
    {
        _velocity.y += gravity * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        this.GetGameMode().OnPlaneHit();
    }

    private void OnChangeColour(PlaneData colour)
    {
        _anim.runtimeAnimatorController = colour.planeAnimColour;
    }

    private void OnDestroy()
    {
        // Unregister to PlaneColour changes.
        if (GameManager.Exists)
            this.GetGameManager().OnPlaneColourChanged -= OnChangeColour;
    }
}
