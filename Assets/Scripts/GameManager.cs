using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    // Events
    public UnityEvent gameStart;
    public UnityEvent gameRestart;
    public UnityEvent<int> scoreChange;
    public UnityEvent gameOver;
    public UnityEvent enemyStomped;

    // Make score public or add a getter property so HUDs can access it
    [Header("Score")]
    [SerializeField] private int score = 0;
    public int Score => score;

    override public void Awake()
        {
            base.Awake();
            Debug.Log("awake called");
        }

    void Start()
    {
        gameStart.Invoke();
        Time.timeScale = 1.0f;

        // Subscribe to scene changes (GameManager persists, so this runs once)
        SceneManager.activeSceneChanged += SceneSetup;
    }

    public void SceneSetup(Scene current, Scene next)
    {
        gameStart.Invoke();
        // Broadcast the current score to any newly spawned HUD in the new scene
        SetScore(score);
    }

    public void TriggerEnemyStomp()
    {
        enemyStomped.Invoke();
        IncreaseScore(1);
    }

    public void GameRestart()
    {
        score = 0;
        SetScore(score);
        gameRestart.Invoke();
        Time.timeScale = 1.0f;

    }

    public void IncreaseScore(int increment)
    {
        score += increment;
        SetScore(score);
    }

    public void SetScore(int newScore)
    {
        scoreChange.Invoke(newScore);
    }

    public void GameOver()
    {
        Time.timeScale = 0.0f;
        gameOver.Invoke();
    }

    
}