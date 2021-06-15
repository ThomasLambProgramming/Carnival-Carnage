using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Properties

    [Header("Game Settings")]
    public bool isFinished = false;
    public bool hasWon = false;
    public int ticketsCollected = 0;

    [Header("Timer Settings")]
    public static string time;
    public float timeRemaining = 120;
    public TextMeshProUGUI timerText;

    [Header("Enemy Stats")]
    public int enemiesLeft = 0;
    public TextMeshProUGUI enemiesText;
    public GameObject bonusTime;

    #endregion

    private void Start()
    {
        InitialiseGame();
    }

    private void Update()
    {
        if (!isFinished)
        {
            UpdateTimer();
        }
    }

    #region Game State Functions

    private void InitialiseGame()
    {
        FindObjectOfType<AudioManager>().PlaySound("Circus Theme Music 1");
        InitialiseTimer();
        UpdateEnemies();
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ChangeLevel(string a_name)
    {
        if (SceneManager.GetSceneByName(a_name) == null)
        {
            Debug.LogWarning("No scene named " + a_name + " exists.");
            return;
        }
        else
        {
            SceneManager.LoadScene(a_name);
        }
    }

    public void ExitGame()
    {
        isFinished = true;
        Application.Quit();
    }
    
    #endregion

    #region Timer Functions

    private void InitialiseTimer()
    {
        // Plays timer coundown on start
        FindObjectOfType<AudioManager>().PlaySound("Timer", timerText.gameObject);
    }

    private void UpdateTimer()
    {
        if (timeRemaining > 0 && !isFinished)
        {
            timeRemaining -= Time.deltaTime;
            SetTimer(timeRemaining);
        }
        else
        {
            isFinished = true;
            FindObjectOfType<AudioManager>().StopPlaying("Timer", timerText.gameObject);
        }
    }

    private void SetTimer(float a_time)
    {
        float minutes = Mathf.FloorToInt(a_time / 60);
        float seconds = Mathf.FloorToInt(a_time % 60);

        if (minutes < 0 && seconds < 0)
        {
            minutes = 0;
            seconds = 0;
            timeRemaining = 0;
        }

        time = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.text = time;
    }

    #endregion

    #region Enemy Stat Functions

    private void UpdateEnemies()
    {
        enemiesLeft = FindObjectsOfType<BalloonEnemy>().Length +
                      FindObjectsOfType<WalkerEnemy>().Length +
                      FindObjectsOfType<Pranksters>().Length;

        enemiesText.text = enemiesLeft.ToString();

        if (enemiesLeft == 0)
        {
            isFinished = true;
        }
    }

    public void AddTime(float a_seconds)
    {
        timeRemaining += a_seconds;

        
    }

    #endregion
}
