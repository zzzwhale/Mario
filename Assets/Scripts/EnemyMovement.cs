using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyMovement : MonoBehaviour
{

    private float originalX;
    private float maxOffset = 5.0f;
    private float enemyPatroltime = 2.0f;
    private int moveRight = -1;
    private Vector2 velocity;

    private Rigidbody2D enemyBody;
    private Animator enemyAnimator;
    public Vector3 startPosition = new Vector3(0.0f, 0.0f, 0.0f);

    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();
        // get the starting position
        originalX = transform.position.x;
        ComputeVelocity();
    }
    void ComputeVelocity()
    {
        velocity = new Vector2((moveRight) * maxOffset / enemyPatroltime, 0);
    }
    void Movegoomba()
    {
        enemyBody.MovePosition(enemyBody.position + velocity * Time.fixedDeltaTime);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name);
    }
    // note that this is Update(), which still works but not ideal. See below.
    void FixedUpdate()
    {
        if (Mathf.Abs(enemyBody.position.x - originalX) < maxOffset)
        {
            Movegoomba();
        }
        else
        {
            moveRight *= -1;
            ComputeVelocity();
            Movegoomba();
        }
    }
    public void GameRestart()
    {

        
        gameObject.SetActive(true);                 // un-hide if it was stomped
        transform.position = startPosition;
        enemyBody.linearVelocity = Vector2.zero;
        enemyAnimator.SetBool("isSquashed", false);

        // reset patrol so it walks the same route again
        originalX = startPosition.x;
        moveRight = -1;
        ComputeVelocity();

        // re-enable the solid body collider that the stomp turned off
        Collider2D body = GetComponent<Collider2D>();
        if (body != null) body.enabled = true;

        Debug.Log("EnemyMovement script restarted!");
    }



    public void Awake()
    {
        startPosition = transform.position;
        //base.Awake();
        GameManager.instance.gameRestart.AddListener(GameRestart);

    }



}