using UnityEngine;
using System.Collections;

public class BrickHit : MonoBehaviour
{
    public GameObject coin;
    public AudioClip coinSound; // <-- Drag your coin sound here in the Inspector
    private AudioSource audioSource; // <-- Holds the audio component

    public GameObject mushroom;
    public AudioClip mushroomSound; // <-- Drag your mushroom sound here in the Inspector

    public Animator brickAnimator; // Animator for the brick


    private Vector3 originalPosition;
    private bool isBouncing = false;
    private bool hasCoin = true;

    void Start()
    {
        originalPosition = transform.position;
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource on the brick
        brickAnimator.SetBool("isHit", false); // Ensure the brick starts in the "not hit" state)

        if (coin != null)
        {
            coin.SetActive(false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isBouncing) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (Mathf.Abs(contact.normal.y) > 0.5f &&
                collision.transform.position.y < transform.position.y)
            {
                StartCoroutine(BounceAndReset());
                break;
            }
        }
    }

    IEnumerator BounceAndReset()
    {
        isBouncing = true;

        float bounceHeight = 0.25f;
        float duration = 0.08f;
        Vector3 targetPos = originalPosition + new Vector3(0, bounceHeight, 0);

        // 1. Bounce the block up
        float elapsed = 0;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(originalPosition, targetPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        brickAnimator.SetBool("isHit", true); // Ensure the brick starts in the "not hit" state)

        // 2. Pop the coin (first hit only)
        if (hasCoin && coin != null)
        {
            hasCoin = false;

            // Play the coin sound effect!
            if (audioSource != null && coinSound != null)
            {
                audioSource.PlayOneShot(coinSound);
            }

            // Unparent so it stays floating outside the block
            coin.transform.SetParent(null);
            coin.transform.position = originalPosition + new Vector3(0, 1.2f, 0);
            coin.SetActive(true);
        }

        // 3. Bounce the block back down
        elapsed = 0;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(targetPos, originalPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPosition;

        // Reset animator state so it goes back to normal
        brickAnimator.SetBool("isHit", true);

        isBouncing = false;
    }
}