using System.Collections;
using UnityEngine;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class QuestionBoxPowerupController : MonoBehaviour, IPowerupController
{

    public Animator questionBoxAnimator;
    public Animator powerupAnimator;
    public BasePowerup powerup;


   

    private Rigidbody2D powerUpRb;
    private float restY;
    

    private bool isUsed = false; // check if the question box has been hit

    void Start()
    {
        powerUpRb = GetComponent<Rigidbody2D>();
        questionBoxAnimator = GetComponent<Animator>();
        restY = powerUpRb.position.y;

        powerupAnimator.gameObject.SetActive(false);   // hide the mushroom, like the coin
    }

    // the block can go up, but never below its resting height
    void FixedUpdate()
    {
        if (isUsed) return;
        if (powerUpRb.position.y < restY)
        {
            powerUpRb.position = new Vector2(powerUpRb.position.x, restY);
            if (powerUpRb.linearVelocity.y < 0)
                powerUpRb.linearVelocity = new Vector2(powerUpRb.linearVelocity.x, 0);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isUsed) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        if (Mathf.Abs(collision.contacts[0].normal.y) > 0.5f &&
            collision.transform.position.y < transform.position.y)
        {
            isUsed = true;

            // stop Mario's upward motion so he doesn't stick to the block
            Rigidbody2D marioRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (marioRb != null)
                marioRb.linearVelocity = new Vector2(marioRb.linearVelocity.x, 0);

            questionBoxAnimator.SetBool("isHit", true);
            powerupAnimator.gameObject.SetActive(true);
            powerupAnimator.SetTrigger("spawned");
            StartCoroutine(LockBlock());
        }
    }

    // let the block finish one bounce, then freeze it
    IEnumerator LockBlock()
    {
        yield return new WaitForSeconds(0.3f);
        Disable();
    }

    public void Disable()
    {
        SpringJoint2D spring = GetComponent<SpringJoint2D>();
        if (spring != null) spring.enabled = false;

        powerUpRb.linearVelocity = Vector2.zero;
        powerUpRb.bodyType = RigidbodyType2D.Static;
        transform.localPosition = new Vector3(0, 0, 0);
    }


}