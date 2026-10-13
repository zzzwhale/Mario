using System.Collections;
using UnityEngine;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class QuestionBoxPowerupController : MonoBehaviour, IPowerupController
{

    public Animator questionBoxAnimator;
    public Animator powerupAnimator;
    public BasePowerup powerup;


    private Rigidbody2D mushroomrb;
    private float restY;
    private bool isUsed = false; // check if the question box has been hit

    void Start()
    {
        mushroomrb = GetComponent<Rigidbody2D>();
        questionBoxAnimator = GetComponent<Animator>();
        restY = mushroomrb.position.y;

        powerupAnimator.gameObject.SetActive(false);   // hide the mushroom, like the coin
    }

    // the block can go up, but never below its resting height
    void FixedUpdate()
    {
        if (isUsed) return;
        if (mushroomrb.position.y < restY)
        {
            mushroomrb.position = new Vector2(mushroomrb.position.x, restY);
            if (mushroomrb.linearVelocity.y < 0)
                mushroomrb.linearVelocity = new Vector2(mushroomrb.linearVelocity.x, 0);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isUsed) return;                                   // already used? ignore
        if (!collision.gameObject.CompareTag("Player")) return;   // only Mario counts

        if (Mathf.Abs(collision.contacts[0].normal.y) > 0.5f &&
            collision.transform.position.y < transform.position.y)
        {
            isUsed = true;
            questionBoxAnimator.SetBool("isHit", true);       // play the used animation
            powerupAnimator.gameObject.SetActive(true);       // appear first
            powerupAnimator.SetTrigger("spawned");            // then pop out
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

        mushroomrb.linearVelocity = Vector2.zero;
        mushroomrb.bodyType = RigidbodyType2D.Static;
        transform.localPosition = new Vector3(0, 0, 0);
    }
}