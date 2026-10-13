using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{

    // Have to assign them in the inspector
    // Your two score text objects
    public TextMeshProUGUI scoreTextInGame;
    public TextMeshProUGUI scoreTextGameOver;
    public Transform restartButton;
    public GameObject gameOverCanva;

    public void Awake()
    {
        // Subscribe to events safely
        if (GameManager.instance != null)
        {
            GameManager.instance.gameStart.AddListener(GameStart);
            GameManager.instance.gameOver.AddListener(GameOver);
            GameManager.instance.gameRestart.AddListener(GameStart);
            GameManager.instance.scoreChange.AddListener(SetScore);
        }
    }

    void Start()
    {
        // Grab the carried-over score immediately on load
        if (GameManager.instance != null)
        {
            SetScore(GameManager.instance.Score);
        }
    }

    public void GameStart()
    {
        // Hide game over panel on start
        if (gameOverCanva != null)
        {
            gameOverCanva.SetActive(false);
        }
    }

    public void SetScore(int score)
    {
        if (scoreTextInGame != null)
        {
            scoreTextInGame.text = "Score: " + score;
        }
        if (scoreTextGameOver != null)
        {
            scoreTextGameOver.text = "Score: " + score;
        }
    }

    public void GameOver()
    {
        Debug.Log("HUDManager.GameOver called, panel = " + gameOverCanva);
        if (gameOverCanva != null)
        {
            gameOverCanva.SetActive(true);
        }
    }
}