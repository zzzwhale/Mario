using UnityEngine;

public class JumpOverGoomba : MonoBehaviour
{
    GameManager gameManager;
    private bool isStomped = false;
    public Animator GoombalAnimator;

    void Start()
    {

        gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        GoombalAnimator.SetBool("isSquashed", false);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isStomped)
        {

            PlayerMovement mario = other.GetComponent<PlayerMovement>();
            if (mario == null || !mario.IsAlive) return;   // dead Mario can't stomp

            isStomped = true;
            GoombalAnimator.SetBool("isSquashed", true);

            // Disable the solid body collider so Mario doesn't die during the delay
            Collider2D bodyCollider = transform.parent.GetComponent<Collider2D>();
            if (bodyCollider != null)
            {
                bodyCollider.enabled = false;
            }

            // 1. Trigger score and game manager events
            if (gameManager != null)
            {
                gameManager.TriggerEnemyStomp();

                AudioEventTrigger audioTrigger = gameManager.GetComponent<AudioEventTrigger>();
                if (audioTrigger != null)
                {
                    audioTrigger.PlaySound();
                }
            }

            // 2. Bounce Mario upward
            Rigidbody2D marioBody = other.GetComponent<Rigidbody2D>();
            if (marioBody != null)
            {
                marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, 8f);
            }

            // 3. Destroy the Goomba after the animation delay
            Destroy(transform.parent.gameObject, 0.2f);
        }
    }
}