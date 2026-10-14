using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{

    // Game Constants
    public GameConstants gameConstants;
    float deathImpulse;
    float upSpeed;
    float maxSpeed;
    float speed;

    // Mario 

    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;
    public Animator marioAnimator;


    // speed
    
    //public float speed;
    //public float upSpeed;
    //public float maxSpeed;
    private int moveDirection = 0;



    public GameObject enemies;
    public bool IsAlive => alive;

    private bool onGroundState = true;

    private bool moving = false;

    // states
    

    [System.NonSerialized]
    private bool alive = true;        // Set to true so Mario can move and jump by default
    private bool jumpedState = false; // Tracks if the initial jump happened for a jumphold

    //public float deathImpulse = 15;

    // Camera
    public Transform gameCamera;

    public GameManager gameManager;


    // Audio
    public AudioSource marioAudioSource;
    public AudioSource staticEnvironmentAudioSource; // plays the theme music
    public AudioClip marioDeathAudio;
    public AudioClip marioJumpAudio;

    // stomping audio is in JumpOverGoomba script

    // current mario jump sound is played from animation event
    // the death sound is played from the OnTriggerEnter2D function when Mario collides with an enemy 



    // Check if Mario is on the ground
    void OnCollisionEnter2D(Collision2D col)
    {
        if ((col.gameObject.CompareTag("Ground") || col.gameObject.CompareTag("Enemy") || col.gameObject.CompareTag("Obstacles")) && !onGroundState)
        {
            onGroundState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        // Set constants (done in lab4)
        speed = gameConstants.speed;
        maxSpeed = gameConstants.maxSpeed;
        deathImpulse = gameConstants.deathImpulse;
        upSpeed = gameConstants.upSpeed;

        // Set to be 60 FPS
        Application.targetFrameRate = 60;
        marioBody = GetComponent<Rigidbody2D>();
        marioSprite = GetComponent<SpriteRenderer>();
        marioAnimator = GetComponent<Animator>();

        // FORCE MARIO TO START IDLE
        // update animator state
        marioAnimator.SetBool("onGround", onGroundState);
        marioAnimator.SetBool("isRunning", false);
        marioAnimator.SetBool("isAlive", true);

    }


    // Update is called once per frame

    void Update()
    {

        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));
    }

    // FixedUpdate may be called once per frame. See documentation for details.
    void FixedUpdate()
    {
        if (alive && moveDirection != 0)
            Move(moveDirection);

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") && alive)
        {
            Debug.Log("Collided with goomba!");

            // play death animation
            marioAnimator.Play("mario_death");
            marioAudioSource.PlayOneShot(marioDeathAudio);
            alive = false;

            // stop the theme music
            staticEnvironmentAudioSource.Stop(); // Stop background music

            // wait for 0.1s before calling TriggerGameOver function to allow death animation to play
            Invoke("TriggerGameOver", 0.1f);
        }
    }
    void TriggerGameOver()
    {
        // Call Game Over function in GameManager
        GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().GameOver();

        


    }

    // Actual Movement function, called from FixedUpdate
    void Move(int value)
    {

        if (Mathf.Abs(marioBody.linearVelocity.x) < maxSpeed)
            marioBody.AddForce(new Vector2(value, 0) * speed);
    }

    // Receives the move direction from the ActionManager and updates the animator state accordingly
    public void MoveCheck(int value)
    {
        if (!alive) return;

        Debug.Log("MoveCheck received: " + value);
        moveDirection = value;
        marioAnimator.SetBool("isRunning", value != 0);

        if (value != 0)
            FaceDirection(value);
    }

    // Flips the sprite based on the direction of movement and triggers the skidding animation if necessary
    private void FaceDirection(int value)
    {
        bool wantsRight = value > 0;
        if (wantsRight == faceRightState) return;

        faceRightState = wantsRight;
        marioSprite.flipX = !wantsRight;

        float vx = marioBody.linearVelocity.x;
        bool skidding = wantsRight ? vx < -0.1f : vx > 0.1f;
        if (skidding)
            marioAnimator.SetTrigger("onSkid");
    }

    // Jump function, called from ActionManager when jump button is pressed
    public void Jump()
    {
        if (alive && onGroundState)
        {
            // jump
            marioBody.AddForce(Vector2.up * upSpeed, ForceMode2D.Impulse);
            onGroundState = false;
            jumpedState = true;
            // update animator state
            marioAnimator.SetBool("onGround", onGroundState);
            

        }
    }

    // Jump hold function, called from ActionManager when jump button is held down
    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            // jump higher
            marioBody.AddForce(Vector2.up * upSpeed * 30, ForceMode2D.Force);
            jumpedState = false;

        }
    }

    // used for animation event to play sounds
    void PlayJumpSound()
    {
        // play jump sound
        marioAudioSource.PlayOneShot(marioJumpAudio);
    }

    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }


    public void GameRestart()
    {
        // reset position
        marioBody.transform.position = new Vector3(-4f, -2.5f, 0.0f);

        // reset movement state
        moveDirection = 0;
        marioBody.linearVelocity = Vector2.zero;
        onGroundState = true;
        jumpedState = false;
        marioAnimator.SetBool("onGround", true);
        marioAnimator.SetBool("isRunning", false);

        // reset sprite direction
        faceRightState = true;
        marioSprite.flipX = false;

        marioAnimator.SetBool("isAlive", true);

        // reset animation
        marioAnimator.SetTrigger("gameRestart");
        alive = true;

        // reset camera position
        gameCamera.position = new Vector3(0, 0, -10);

        Debug.Log("Player Movement script restarted!");

        // stop death sound if it's still playing
        marioAudioSource.Stop();

        staticEnvironmentAudioSource.Play(); // Restart background music

    }

    public void Awake()
    {
        //base.Awake();
        GameManager.instance.gameRestart.AddListener(GameRestart);

    }



}