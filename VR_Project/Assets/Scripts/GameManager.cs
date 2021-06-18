using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO;

public class GameManager : MonoBehaviour
{

    #region Properties

    [Header("Game Settings")]
    public bool isFinished = false;
    public bool hasWon = false;
    private bool calulated = false;
    private bool playedEndSound = false;

    public int growthRate = 1; // Decides how fast the scores will increase
    public int ticketsCollected = 0;
    private int endTickets = 0;
    public GameObject endGameUISpawn;
    public GameObject endGameUI;
    private GameObject menu;

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
    private float guiTime = 2;
    public TextMeshProUGUI enemiesText;
    public GameObject bonusTime;
    private string filePath = "";
    #endregion

    private void Start()
    {
        grabScript = FindObjectOfType<XRGrabInteractable>().GetComponent<XRGrabInteractable>();
        filePath = Application.dataPath + "TicketAmount.json";
        InitialiseGame();
        UpdateEnemies();
    }

    private void Update()
    {
        if (!isFinished)
        {
            UpdateTimer();
        }
        else
        {
            CompleteLevelUI();
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

    private void CompleteLevelUI()
    {
        // Obtain UI elements
        if (menu == null)
        {
            menu = Instantiate(endGameUI, endGameUISpawn.transform);
        }

        GameObject UICanvas = menu.GetComponentInChildren<Canvas>().gameObject;

        if (UICanvas == null)
        {
            return;
        }

        TextMeshProUGUI winLoseText = UICanvas.transform.Find("WinOrLose").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI finalTimeText = UICanvas.transform.Find("TimeLeft").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ticketsText = UICanvas.transform.Find("TicketsCollected").GetComponent<TextMeshProUGUI>();

        FindObjectOfType<AudioManager>().StopPlaying("Circus Theme Music 1");

        // If enemies remain, player loses. Otherwise the player wins.
        if (enemiesLeft > 0)
        {
            winLoseText.text = "You Lose!";
            winLoseText.color = GetColorFromString("D95151");
            if (!playedEndSound)
            {
                FindObjectOfType<AudioManager>().PlaySound("Lose Music");
            }
            playedEndSound = true;
        }
        else
        {
            winLoseText.text = "You Win!";
            winLoseText.color = GetColorFromString("71FF34");
            if (!playedEndSound)
            {
                FindObjectOfType<AudioManager>().PlaySound("Win Music");
            }
            playedEndSound = true;
        }

        // Converts time remaining into time format
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);

        // If time less than zero, set value to 0
        if (minutes < 0 && seconds < 0)
        {
            minutes = 0;
            seconds = 0;
        }

        // Format time and update UI element to display time
        string time = string.Format("{0:00}:{1:00}", minutes, seconds);
        finalTimeText.text = time;

        // Calculate the tickets gained
        if (!calulated)
        {
            ticketsCollected = CalculateTickets();

            StartCoroutine(TicketUpdater(ticketsText));
            calulated = true;
        }
    }

    private int CalculateTickets()
    {
        int tickets = (int)timeRemaining;
        return tickets;
    }

    IEnumerator TicketUpdater(TextMeshProUGUI a_ticketText)
    {
        while(true)
        {
            if (endTickets != ticketsCollected && ticketsCollected > endTickets)
            {
                endTickets += growthRate;
                a_ticketText.text = endTickets.ToString();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private class TicketObject
    {
        public int ticketAmount = 0;
        public TicketObject(int a_ticketAmount) => ticketAmount = a_ticketAmount;
    }
    //ADDS ONTO THE FILE TICKET AMOUNT
    public void WriteTicketToFile(int ticketAmount)
    {
        TicketObject ticket = new TicketObject(ticketAmount);

        int heldTickets = ReadTicketFile();
        //if there is a file then we can add
        if (heldTickets != -1)
            ticket.ticketAmount += heldTickets;
        
        StreamWriter stream = new StreamWriter(filePath);
        string json = JsonUtility.ToJson(ticket, true);
        stream.Write(json);
        stream.Close();
    }
    //OVERWRITES THE TICKET AMOUNT 
    public void OverwriteTicketAmount(int ticketAmount)
    {
        TicketObject ticket = new TicketObject(ticketAmount);
        StreamWriter stream = new StreamWriter(filePath);
        string json = JsonUtility.ToJson(ticket, true);
        stream.Write(json);
        stream.Close();
    }
    //RETURNS THE AMOUNT IN THE FILE
    public int ReadTicketFile()
    {
        if (!File.Exists(filePath))
            return -1;

        StreamReader stream = new StreamReader(filePath);
        string jsonData = stream.ReadToEnd();
        TicketObject tickets = JsonUtility.FromJson<TicketObject>(jsonData);
        stream.Close();
        return tickets.ticketAmount;
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
        }

        if (a_time <= 10 && !FindObjectOfType<AudioManager>().isPlaying("Timer") && !isFinished)
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

        if (enemiesLeft == 0)
        {
            isFinished = true;
        }

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

    #region Other Functions

    private float HexToFloatNormalised(string a_hex)
    {
        int dec = System.Convert.ToInt32(a_hex, 16);
        return dec / 255f;
    }

    private Color GetColorFromString(string a_hexString)
    {
        float red = HexToFloatNormalised(a_hexString.Substring(0, 2));
        float green = HexToFloatNormalised(a_hexString.Substring(2, 2));
        float blue = HexToFloatNormalised(a_hexString.Substring(4, 2));
        float alpha = 1f;

        if (a_hexString.Length >= 8)
        {
            alpha = HexToFloatNormalised(a_hexString.Substring(6, 2));
        }

        return new Color(red, green, blue, alpha);
    }

    #endregion
}
