using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5;
    private Rigidbody2D marioBody;
    public float upSpeed = 10;
    private bool onGroundState = true;

    public float maxSpeed = 20;
    private SpriteRenderer marioSprite;
    private bool faceRightState = true;

    
    public GameObject enemies;

   

    
    private bool moving = false;

    // Added from week 2 lab
    public Animator marioAnimator;
    private bool alive = true;        // Set to true so Mario can move and jump by default
    private bool jumpedState = false; // Tracks if the initial jump happened for a jumphold

    public Transform gameCamera;

    public GameManager gameManager;


    // Check if Mario is on the ground
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            onGroundState = true;

            // ---> THIS IS THE PART THAT WAS ADDED/EDITED:
            if (marioAnimator != null)
            {
                marioAnimator.SetBool("onGround", onGroundState);
            }
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
        marioAnimator.SetBool("isRunning", false);
        marioAnimator.SetBool("isAlive", true);

    }

    // Update is called once per frame
    void Update()
    {
        marioAnimator.SetFloat("xSpeed", Mathf.Abs(marioBody.linearVelocity.x));

    }

    // FixedUpdate is called 50 times a second
    void FlipMarioSprite(int value)
    {
        if (value == -1 && faceRightState)
        {
            faceRightState = false;
            marioSprite.flipX = true;
            if (marioBody.linearVelocity.x > 0.05f)
                marioAnimator.SetTrigger("onSkid");

        }

        else if (value == 1 && !faceRightState)
        {
            faceRightState = true;
            marioSprite.flipX = false;
            if (marioBody.linearVelocity.x < -0.05f)
                marioAnimator.SetTrigger("onSkid");
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            // If we are stomping from above, JumpOverGoomba handles it. 
            // We only want to trigger Game Over if Mario hits the enemy sideways or from below!
            Debug.Log("Collided with enemy trigger!");

            
            // mario dies
            marioAnimator.SetBool("isAlive", false);
            // Call Game Over function
            GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>().GameOver();

        }
    }
    // FixedUpdate may be called once per frame. See documentation for details.
    void FixedUpdate()
    {
        if (alive && moving)
        {
            Move(faceRightState == true ? 1 : -1);
        }

      

    }
    
    // Movements
    void Move(int value)
    {

        Vector2 movement = new Vector2(value, 0);
        // check if it doesn't go beyond maxSpeed
        if (marioBody.linearVelocity.magnitude < maxSpeed)
            marioBody.AddForce(movement * speed);
    }

    public void MoveCheck(int value)
    {   
        // if mario is not moving
        if (value == 0)
        {
            moving = false;
            marioAnimator.SetBool("isRunning", false);
        }   
        else
        {
            FlipMarioSprite(value);
            moving = true;
            Move(value);
            marioAnimator.SetBool("isRunning", true); // Set the "isRunning" parameter to true when moving

        }
    }

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
   

    public void JumpHold()
    {
        if (alive && jumpedState)
        {
            // jump higher
            marioBody.AddForce(Vector2.up * upSpeed * 30, ForceMode2D.Force);
            jumpedState = false;

        }
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

   
    public void GameRestart()
    {
        // reset position
        marioBody.transform.position = new Vector3(-4f, -2.5f, 0.0f); // change this position to my starting position of Mario
        
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



}