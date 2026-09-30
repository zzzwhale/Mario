using UnityEngine;

public class JumpOverGoomba : MonoBehaviour
{
    GameManager gameManager;
    private bool isStomped = false;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isStomped)
        {
            isStomped = true;

            // 1. Trigger score and game manager events
            if (gameManager != null)
            {
                gameManager.TriggerEnemyStomp();

                // Play the stomp sound via the AudioEventTrigger component on GameManager
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

            // 3. Destroy the Goomba
            Destroy(transform.parent.gameObject);
        }
    }
}