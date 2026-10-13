using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{

    private Rigidbody2D marioBody;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;

    /// <summary>
    // speed
    /// </summary>
    public float speed;
    public float upSpeed;
    public float maxSpeed;
    private int moveDirection = 0;



    public GameObject enemies;
    public bool IsAlive => alive;

    private bool onGroundState = true;

    private bool moving = false;

    // Added from week 2 lab
    public Animator marioAnimator;

    [System.NonSerialized]
    private bool alive = true;        // Set to true so Mario can move and jump by default
    private bool jumpedState = false; // Tracks if the initial jump happened for a jumphold

    public float deathImpulse = 15;

    // Camera
    public Transform gameCamera;

    public GameManager gameManager;


    // Audio
    public AudioSource marioAudio;

    public AudioClip marioDeath;

  

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
            marioAudio.PlayOneShot(marioDeath);
            alive = false;


            Invoke("TriggerGameOver", 0.1f);
        }
    }
    void TriggerGameOver()
    {
        // Call Game Over function
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

    void PlayJumpSound()
    {
        // play jump sound
        marioAudio.PlayOneShot(marioAudio.clip);
    }

    void PlayDeathImpulse()
    {
        marioBody.AddForce(Vector2.up * deathImpulse, ForceMode2D.Impulse);
    }

    // Button listener
    public void RestartButtonCallback()
    {
        Debug.Log("Restart!");
        // reset everything
        
        GameRestart();
        // resume time
        Time.timeScale = 1.0f;
    }
    void GameOverScene()
    {
        // stop time
        Time.timeScale = 0.0f;
        // set gameover scene
        gameManager.GameOver(); // replace this with whichever way you triggered the game over screen for Checkoff 1
    }

    //public void GameRestart()
    //{
    //    // reset position
    //    marioBody.transform.position = new Vector3(-4f, -2.5f, 0.0f); // change this position to my starting position of Mario

    //    // reset sprite direction
    //    faceRightState = true;
    //    marioSprite.flipX = false;

    //    marioAnimator.SetBool("isAlive", true);

    //    // reset animation
    //    marioAnimator.SetTrigger("gameRestart");
    //    alive = true;

    //    // reset camera position
    //    gameCamera.position = new Vector3(0, 0, -10);



    //}

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
    }

    public void Awake()
    {
        //base.Awake();
        GameManager.instance.gameRestart.AddListener(GameRestart);

    }



}