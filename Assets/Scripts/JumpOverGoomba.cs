using UnityEngine;

public class JumpOverGoomba : MonoBehaviour
{
    GameManager gameManager;
    private bool isStomped = false;
    public Animator GoombalAnimator;


    // Audio
    public AudioSource marioAudioSource;
    public AudioClip marioStompAudio;

    void Start()
    {
        
        gameManager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        GoombalAnimator.SetBool("isSquashed", false);

    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || isStomped) return;

        PlayerMovement mario = other.GetComponent<PlayerMovement>();
        if (mario == null || !mario.IsAlive) return;   // dead Mario can't stomp

        Rigidbody2D marioBody = other.GetComponent<Rigidbody2D>();

        // Mario must be above the stomp zone and not moving upward
        bool isAbove = other.bounds.min.y > GetComponent<Collider2D>().bounds.center.y;
        bool isFalling = marioBody != null && marioBody.linearVelocity.y <= 0.1f;

        
        if (!isAbove || !isFalling) return;

        Debug.Log("Mario stomped the Goomba!");
        isStomped = true;

        // play stomp sound and trigger the squash animation
        marioAudioSource.PlayOneShot(marioStompAudio);
        GoombalAnimator.SetBool("isSquashed", true);

        // disable the solid body collider so Mario doesn't die during the delay
        Collider2D bodyCollider = transform.parent.GetComponent<Collider2D>();
        if (bodyCollider != null)
        {
            bodyCollider.enabled = false;
        }

        // 1. trigger score and game manager events
        if (gameManager != null)
        {
            gameManager.TriggerEnemyStomp();
        }

        // 2. bounce Mario upward
        if (marioBody != null)
        {
            marioBody.linearVelocity = new Vector2(marioBody.linearVelocity.x, 8f);
        }

        // 3. hide goomba instead of destroying after the animation delay
        Invoke(nameof(HideGoomba), 0.2f);
    }

    void HideGoomba()
    {
        transform.parent.gameObject.SetActive(false);
    }

   
}