using UnityEngine;

using UnityEngine;
using System.Collections;

public class BrickHit : MonoBehaviour
{
    public GameObject coin;
    public AudioClip coinSound;
    public GameObject mushroom;          // unused for now
    public AudioClip mushroomSound;      // unused for now
    public Animator brickAnimator;

  

    private AudioSource audioSource;
    private Rigidbody2D rb;
    private SpringJoint2D spring;
    private float restY;                 // the brick's resting height (the floor)
    private bool hasCoin = true;
    private bool isUsed = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // get the Rigidbody2D component attached to the brick
        spring = GetComponent<SpringJoint2D>();
        audioSource = GetComponent<AudioSource>();
        restY = rb.position.y;

        // check if the brick has been hit before
        brickAnimator.SetBool("isHit", false);
        if (coin != null) coin.SetActive(false);
    }

    void FixedUpdate()
    {
        if (isUsed) return;

        // Floor: the brick may go up, but never below its rest position
        if (rb.position.y < restY)
        {
            rb.position = new Vector2(rb.position.x, restY);
            if (rb.linearVelocity.y < 0)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isUsed) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Mario must be below the brick (head hit), not standing on top.
            // the contact is on a top or bottom face, not the side & Mario should be below the brick, not above it
            if (Mathf.Abs(contact.normal.y) > 0.5f &&
                collision.transform.position.y < transform.position.y)
            {
                OnHitFromBelow();
                break;
            }
        }
    }

    void OnHitFromBelow()
    {
     

        if (hasCoin && coin != null)
        {
            hasCoin = false;
            isUsed = true;
            brickAnimator.SetBool("isHit", true);

            if (audioSource != null && coinSound != null)
                audioSource.PlayOneShot(coinSound);

            coin.transform.SetParent(null);
            coin.transform.position = new Vector3(transform.position.x, restY + 1.2f, transform.position.z);
            coin.SetActive(true);

            StartCoroutine(LockBrick());
        }
    }

    // Let the brick finish its one bounce, then freeze it as an empty block
    IEnumerator LockBrick()
    {
        yield return new WaitForSeconds(0.3f);

        spring.enabled = false;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.position = new Vector2(rb.position.x, restY);
    }
}