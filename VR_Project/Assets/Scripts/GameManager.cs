using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

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

    [Header("Player Settings")]
    public GameObject player;
    private XRGrabInteractable grabScript = null;

    [Header("Enemy Stats")]
    public int enemiesLeft = 0;
    public float extraTime = 0;
    public float guiTime = 2;
    public TextMeshProUGUI enemiesText;
    public GameObject bonusTime;

    #endregion

    private void Start()
    {
        grabScript = FindObjectOfType<XRGrabInteractable>().GetComponent<XRGrabInteractable>();

        InitialiseGame();
    }

    private void Update()
    {
        if (!isFinished)
        {
            UpdateTimer();
            UpdateEnemies();
        }
    }

    #region Game State Functions

    private void InitialiseGame()
    {
        FindObjectOfType<AudioManager>().PlaySound("Circus Theme Music 1");
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

    private void UpdateTimer()
    {
        if (timeRemaining > 0 && !isFinished)
        {
            timeRemaining -= Time.deltaTime - extraTime;
            extraTime = 0;
            SetTimer(timeRemaining);
        }
        else
        {
            isFinished = true;
            FindObjectOfType<AudioManager>().StopPlaying("Timer");
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

        if (a_time <= 10 && !FindObjectOfType<AudioManager>().isPlaying("Timer"))
        {
            FindObjectOfType<AudioManager>().PlaySound("Timer");
        }
        else if (a_time > 10)
        {
            FindObjectOfType<AudioManager>().StopPlaying("Timer");
        }

        time = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.text = time;
    }

    #endregion

    #region Enemy Stat Functions

    private void UpdateEnemies()
    {
        enemiesLeft = FindObjectsOfType(typeof(BalloonEnemy), false).Length +
                      FindObjectsOfType(typeof(WalkerEnemy), false).Length +
                      FindObjectsOfType(typeof(Pranksters), false).Length +
                      FindObjectsOfType(typeof(PropellerEnemy), false).Length;

        enemiesText.text = enemiesLeft.ToString();

        if (enemiesLeft == 0)
        {
            isFinished = true;
        }
    }

    public void AddTime(GameObject a_source, float a_seconds)
    {
        // If melee attack +5s, else (hammer throw) +3s
        if (grabScript.isSelected == true)
        {
            extraTime += a_seconds + 2;
        }
        else
        {
            extraTime += a_seconds;
        }

        // Since this function is used when an enemy is killed, enemiesLeft needs to be updated
        enemiesLeft--;
        enemiesText.text = enemiesLeft.ToString();

        // Instantiates bonus time UI, sets value to given time
        GameObject newBonus = Instantiate(bonusTime, a_source.transform);

        // Rotates canvas to follow direction player is looking
        newBonus.transform.rotation = Quaternion.LookRotation(newBonus.transform.position - new Vector3(Camera.main.transform.position.x, newBonus.transform.position.y, Camera.main.transform.position.z));

        // Updates the text
        newBonus.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("+{0}s", extraTime);

        StartCoroutine(GUIDisplayBonusTime(newBonus));
    }

    IEnumerator GUIDisplayBonusTime(GameObject a_text)
    {
        //a_text.GetComponent<Animation>().Play();

        // Waits for a given amount of time
        yield return new WaitForSeconds(guiTime);

        // Destroy bonus time UI
        Destroy(a_text);
    }

    #endregion
}
