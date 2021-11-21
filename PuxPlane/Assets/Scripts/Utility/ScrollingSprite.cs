using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingSprite : MonoBehaviour
{
    public enum EScrollDirection
    {
        Left,
        Right
    }

    [Header("Scroll")]
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private EScrollDirection direction = EScrollDirection.Left;

    [Header("Sprite")]
    [SerializeField]
    private int tileCount = 2;
    [SerializeField]
    private Vector2 spriteSize;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLines = false;
    [SerializeField]
    private float debugLineLength = 2f;

    [Header("References")]
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    // Sprite
    private float _spriteRightEdge;
    private float _spriteLeftEdge;
    private Vector2 _distanceBetweenEdges;

    // Properties
    public float Speed
    {
        get => speed;
        set
        {
            speed = value < 0f ? 0f : value;
        }
    }


    private void Awake()
    {
        //spriteRenderer = GetComponent<SpriteRenderer>();
        
        CalculateSpriteEdges();
    }

    private void CalculateSpriteEdges()
    {
        _spriteRightEdge = transform.position.x + (spriteRenderer.bounds.extents.x / tileCount);
        _spriteLeftEdge = transform.position.x - (spriteRenderer.bounds.extents.x / tileCount);
        _distanceBetweenEdges = new Vector2(_spriteRightEdge - _spriteLeftEdge, 0f);
    }

    private void Update()
    {
        if (speed == 0f)
            return;

        if (direction == EScrollDirection.Left)
            transform.localPosition -= speed * Vector3.right * Time.deltaTime;
        else
            transform.localPosition += speed * Vector3.right * Time.deltaTime;

        if (HasPassedEdge())
        {
            RepositionSprite();
        }
    }

    private bool HasPassedEdge()
    {
        bool passedRightEdge = direction == EScrollDirection.Right && transform.position.x > _spriteRightEdge;
        bool passedLeftEdge = direction == EScrollDirection.Left && transform.position.x < _spriteLeftEdge;
        return passedRightEdge || passedLeftEdge;
    }

    private void RepositionSprite()
    {
        if (direction == EScrollDirection.Left)
            transform.localPosition += (Vector3)_distanceBetweenEdges;
        else
            transform.localPosition -= (Vector3)_distanceBetweenEdges;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        if (!showDebugLines)
            return;

        Vector2 rightLineStart = new Vector2(_spriteRightEdge, transform.position.y + debugLineLength);
        Vector2 rightLineEnd = new Vector2(_spriteRightEdge, transform.position.y - debugLineLength);

        Vector2 leftLineStart = new Vector2(_spriteLeftEdge, transform.position.y + debugLineLength);
        Vector2 leftLineEnd = new Vector2(_spriteLeftEdge, transform.position.y - debugLineLength);

        Gizmos.DrawLine(rightLineStart, rightLineEnd);
        Gizmos.DrawLine(leftLineStart, leftLineEnd);
    }

    private void OnValidate()
    {
        if (spriteRenderer == null)
            return;

        // Reclaculate sprite size (as sprite may have been changed)
        spriteSize = spriteRenderer.sprite.rect.size / spriteRenderer.sprite.pixelsPerUnit;
        spriteRenderer.size = new Vector2(spriteSize.x * tileCount, spriteSize.y);
    }
}
