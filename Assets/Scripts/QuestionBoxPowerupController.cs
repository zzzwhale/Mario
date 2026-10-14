using System.Collections;
using UnityEngine;

public class QuestionBoxPowerupController : MonoBehaviour, IPowerupController
{
    public Animator questionBoxAnimator;
    public Animator powerupAnimator;
    public BasePowerup powerup;

    public float bounceHeight = 0.3f;
    public float bounceDuration = 0.15f;

    private Rigidbody2D rb;
    private bool isUsed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        questionBoxAnimator = GetComponent<Animator>();
        powerupAnimator.gameObject.SetActive(false);   // hide the mushroom
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isUsed) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        // only count hits from below: Mario's head must be under the block's bottom edge
        float marioTop = collision.collider.bounds.max.y;
        float boxBottom = GetComponent<Collider2D>().bounds.min.y;
        if (marioTop > boxBottom + 0.1f) return;

        isUsed = true;

        // stop Mario going up so he doesn't stick to the block
        Rigidbody2D marioRb = collision.gameObject.GetComponent<Rigidbody2D>();
        if (marioRb != null)
            marioRb.linearVelocity = new Vector2(marioRb.linearVelocity.x, 0);

        questionBoxAnimator.SetBool("isHit", true);
        powerupAnimator.gameObject.SetActive(true);
        powerupAnimator.SetTrigger("spawned");

        StartCoroutine(Bounce());
    }

    // nudge up, come back down, then freeze
    IEnumerator Bounce()
    {
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
        Disable();
    }

    public void Disable()
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
    }
}