using System.Collections;
using UnityEngine;

public class Brick : MonoBehaviour
{
    [Header("Optional coin inside")]
    public bool hasCoin = false;
    public Animator coinAnimator;      // child coin (leave empty for a plain brick)
    public int coinsInside = 1;

    public float bounceHeight = 0.3f;
    public float bounceDuration = 0.15f;

    private Rigidbody2D rb;
    private bool isBouncing = false;
    private bool isEmpty = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (coinAnimator != null)
            coinAnimator.gameObject.SetActive(false);   // hide the coin until needed
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBouncing) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        // only count hits from below
        float marioTop = collision.collider.bounds.max.y;
        float brickBottom = GetComponent<Collider2D>().bounds.min.y;
        if (marioTop > brickBottom + 0.1f) return;

        // stop Mario going up so he doesn't stick to the brick
        Rigidbody2D marioRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (marioRb != null)
            marioRb.linearVelocity = new Vector2(marioRb.linearVelocity.x, 0);

        if (hasCoin && !isEmpty && coinAnimator != null)
        {
            coinAnimator.gameObject.SetActive(true);
            coinAnimator.SetTrigger("spawned");
            coinsInside--;
            if (coinsInside <= 0) isEmpty = true;
        }

        StartCoroutine(Bounce());
    }

    IEnumerator Bounce()
    {
        isBouncing = true;
        Vector2 start = rb.position;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.fixedDeltaTime / bounceDuration;
            float y = Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI) * bounceHeight;
            rb.MovePosition(start + Vector2.up * y);
            yield return new WaitForFixedUpdate();
        }
        rb.position = start;
        isBouncing = false;

        // only a spent coin brick freezes; plain bricks stay bumpable
        if (hasCoin && isEmpty)
            rb.bodyType = RigidbodyType2D.Static;
    }
}