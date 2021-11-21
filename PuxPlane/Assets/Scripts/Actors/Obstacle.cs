using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float yOffsetLimit = 2.75f;
    [SerializeField]
    private float offscreenPositionMin = -12f;

    [Header("Sprites")]
    [SerializeField]
    private SpriteRenderer rock;
    [SerializeField]
    private SpriteRenderer rockTop;

    // References
    private Rigidbody2D _rb;

    // Properties
    public float Speed
    {
        get => speed;
        set => speed = value;
    }


    private void Awake()
    {
        // Get references
        _rb = GetComponent<Rigidbody2D>();

        // Set rock sprites from selected GameManager theme
        rock.sprite = this.GetGameManager().MapTheme.rock;
        rockTop.sprite = this.GetGameManager().MapTheme.rockDown;
    }

    private void OnEnable()
    {
        float offsetY = Random.Range(-yOffsetLimit, yOffsetLimit);
        transform.position = new Vector3(offscreenPositionMin * -1f, offsetY, 0f);
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + Vector2.left * speed * Time.fixedDeltaTime);

        // Are we beyond the off-screen limit?
        if (_rb.position.x < offscreenPositionMin)
        {
            this.GetGameMode().OnObstacleOffScreen(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            this.GetGameMode().OnObstaclePassed();
        }
    }
}
