using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDManager : MonoBehaviour
{
  

    // I have 2 score text objects, one for in-game and one for game over
    public TextMeshProUGUI scoreTextInGame;
    public TextMeshProUGUI scoreTextGameOver;
    public Transform restartButton;

    public GameObject gameOverPanel;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GameStart()
    {
        // hide gameover panel
        gameOverPanel.SetActive(false);
        
    }

    public void SetScore(int score)
    {
        scoreTextInGame.text = "Score: " + score;
        scoreTextGameOver.text = "Score: " + score;
    }


    public void GameOver()
    {
        Debug.Log("HUDManager.GameOver called, panel = " + gameOverPanel);
        gameOverPanel.SetActive(true);
        
    }
}
