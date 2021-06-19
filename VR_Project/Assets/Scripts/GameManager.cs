/*
* File: GameManager.cs
*
* Author: Mara Dusevic (s200494@students.aie.edu.au)
* Date Created: Sunday 13 June 2021
* Date Last Modified: Friday 18 June 2021
*
* Appears in every scene and updates the games state.
* Contains functions that calculates the time, save
* tickets collected, update enemy count, etc,.
*
*/

using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using System.IO;

public class GameManager : MonoBehaviour
{
    #region Properties

    [Header("Game Settings")]
    public bool isFinished = false;
    public bool hasWon = false;
    private bool calulated = false;
    //private bool initialised = false;
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
    public GameObject rightGameplayController;
    public GameObject rightRayController;
    private XRGrabInteractable grabScript = null;

    [Header("Enemy Stats")]
    public int enemiesLeft = 0;
    public float extraTime = 0;
    private float guiTime = 2;
    public TextMeshProUGUI enemiesText;
    public GameObject bonusTime;
    private string filePath = "";
    //private string hammer_filePath = "";
    #endregion

    // Start Function
    private void Start()
    {
        // Finds the controller's grab interaction script
        grabScript = FindObjectOfType<XRGrabInteractable>().GetComponent<XRGrabInteractable>();

        // Sets the path to the file that stores the player's ticket amount
        filePath = Application.dataPath + "TicketAmount.json";

        // hammer_filePath = Application.dataPath + "EquippedHammer.json";
        // SetStartHammer();

        InitialiseGame();
        UpdateEnemies();
    }

    // Update Function
    private void Update()
    {
        // If game is not finished, continue updating the timer
        if (!isFinished)
        {
            UpdateTimer();
        }
        // Otherwise call the end game level UI
        else
        {
            CompleteLevelUI();
        }
    }

    #region Game State Functions

    // Called at start and initialises the game
    private void InitialiseGame()
    {
        FindObjectOfType<AudioManager>().PlaySound("Circus Theme Music 1");
    }

    // Sets components in the end level UI to reflect player progress
    private void CompleteLevelUI()
    {
        // Obtain UI elements
        if (menu == null)
        {
            // Spawns the UI at the given point
            menu = Instantiate(endGameUI, endGameUISpawn.transform);
        }
        
        // Finds the canvas in the given menu
        GameObject UICanvas = menu.GetComponentInChildren<Canvas>().gameObject;

        if (UICanvas == null)
        {
            return;
        }

        // Sets canvas render camera
        UICanvas.GetComponent<Canvas>().worldCamera = Camera.main;

        // Finds all text components in the given canvas
        TextMeshProUGUI winLoseText = UICanvas.transform.Find("WinOrLose").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI finalTimeText = UICanvas.transform.Find("TimeLeft").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI ticketsText = UICanvas.transform.Find("TicketsCollected").GetComponent<TextMeshProUGUI>();

        // Switches to controller that can raycast to UI
        rightRayController.SetActive(true);
        rightGameplayController.SetActive(false);

        // Since level is over, stop playing main game theme
        FindObjectOfType<AudioManager>().StopPlaying("Circus Theme Music 1");

        // If enemies remain, player loses. Otherwise the player wins.
        if (enemiesLeft > 0)
        {
            // Sets win text to show the player lost
            winLoseText.text = "You Lose!";
            winLoseText.color = GetColorFromString("D95151");
            
            // Plays losing music
            if (!playedEndSound)
            {
                FindObjectOfType<AudioManager>().PlaySound("Lose Music");
            }
            playedEndSound = true;
        }
        else
        {
            // Sets win text to show the player won
            winLoseText.text = "You Win!";
            winLoseText.color = GetColorFromString("71FF34");

            // Plays winning music
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

    // Calculates the tickets gained from the time remaining
    private int CalculateTickets()
    {
        int tickets = (int)timeRemaining;
        return tickets;
    }

    // Creates an increasing score effect on the tickets collected
    IEnumerator TicketUpdater(TextMeshProUGUI a_ticketText)
    {
        while (true)
        {
            // While the end tickets haven't reached the tickets collected
            if (endTickets != ticketsCollected && ticketsCollected > endTickets)
            {
                // Updates the end ticket amount by the given growth rate
                endTickets += growthRate;

                // Updates the UI text
                a_ticketText.text = endTickets.ToString();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    // Saves the ticket count

    private class TicketObject
    {
        public int ticketAmount = 0;
        public TicketObject(int a_ticketAmount) => ticketAmount = a_ticketAmount;
    }

    // Adds onto the file ticket amount
    public void WriteTicketToFile(int ticketAmount)
    {
        TicketObject ticket = new TicketObject(ticketAmount);

        int heldTickets = ReadTicketFile();

        // If there is a file then we can add
        if (heldTickets != -1)
        {
            ticket.ticketAmount += heldTickets;
        }

        StreamWriter stream = new StreamWriter(filePath);
        string json = JsonUtility.ToJson(ticket, true);
        stream.Write(json);
        stream.Close();
    }

    // Overwrites the file ticket amount 
    public void OverwriteTicketAmount(int ticketAmount)
    {
        TicketObject ticket = new TicketObject(ticketAmount);
        StreamWriter stream = new StreamWriter(filePath);
        string json = JsonUtility.ToJson(ticket, true);
        stream.Write(json);
        stream.Close();
    }

    // Returns the amount in the file
    public int ReadTicketFile()
    {
        if (!File.Exists(filePath))
        {
            return -1;
        }

        StreamReader stream = new StreamReader(filePath);
        string jsonData = stream.ReadToEnd();
        TicketObject tickets = JsonUtility.FromJson<TicketObject>(jsonData);
        stream.Close();
        return tickets.ticketAmount;
    }

    #endregion

    #region Timer Functions

    // Updates the timer
    private void UpdateTimer()
    {
        // If game is not finished and the timer is above 0 continue updating
        if (timeRemaining > 0 && !isFinished)
        {
            timeRemaining -= Time.deltaTime - extraTime;
            extraTime = 0;
            SetTimer(timeRemaining);
        }
        // Otherwise, game is finished and stop playing the timer sound
        else
        {
            isFinished = true;
            FindObjectOfType<AudioManager>().StopPlaying("Timer");
        }
    }

    // Sets the timer text with the given time
    private void SetTimer(float a_time)
    {
        // Converts the time into minutes and seconds
        float minutes = Mathf.FloorToInt(a_time / 60);
        float seconds = Mathf.FloorToInt(a_time % 60);

        // If both are below zero, set to zero
        if (minutes < 0 && seconds < 0)
        {
            minutes = 0;
            seconds = 0;
        }

        // If time is lower that 10 seconds and game is not over, play timer sound.
        // Otherwise stop playing the timer if the time is above 10 seconds and game is finished. 
        if (a_time <= 10 && !FindObjectOfType<AudioManager>().isPlaying("Timer") && !isFinished)
        {
            FindObjectOfType<AudioManager>().PlaySound("Timer");
        }
        else if (a_time > 10)
        {
            FindObjectOfType<AudioManager>().StopPlaying("Timer");
        }

        // Formats the time into the normal time format 
        time = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        // Updates the timer text
        timerText.text = time;
    }

    #endregion

    #region Enemy Stat Functions

    // Updates the enemies count, used at the beginning of the game
    private void UpdateEnemies()
    {
        // Finds all the enemies in scene 
        enemiesLeft = FindObjectsOfType(typeof(BalloonEnemy), false).Length +
                      FindObjectsOfType(typeof(WalkerEnemy), false).Length +
                      FindObjectsOfType(typeof(Pranksters), false).Length +
                      FindObjectsOfType(typeof(PropellerEnemy), false).Length;

        // Updates the enemies left text
        enemiesText.text = enemiesLeft.ToString();
    }

    // Called in other scripts to add extra time and update enemies
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

        // If not enemies remain, game is over
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

    // Used to destroy UI that appears to indicate extra time gained
    IEnumerator GUIDisplayBonusTime(GameObject a_text)
    {
        // Waits for a given amount of time
        yield return new WaitForSeconds(guiTime);

        // Destroy bonus time UI
        Destroy(a_text);
    }

    #endregion

    #region Other Functions

    // Used to convert hex values to decimal values and normalise them 
    private float HexToFloatNormalised(string a_hex)
    {
        int dec = System.Convert.ToInt32(a_hex, 16);
        return dec / 255f;
    }

    // Using the given hex string it returns it's color
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

    #region Hammer Management

    //public class Hammer
    //{
    //    public GameObject HammerObject;
    //    public int TicketCost = 1;
    //    public bool Unlocked = false;
    //}

    //[SerializeField]
    //public List<GameObject> HammerObjects = new List<GameObject>();

    //public List<Hammer> Hammers = new List<Hammer>();

    //private int EquippedHammer = 0;


    //public void BuyOrEquip(int HammerNumber)
    //{
    //    if (Hammers[EquippedHammer].Unlocked)
    //        EquippedHammer = HammerNumber;

    //    else
    //    {
    //        int TotalTickets = ReadTicketFile();

    //        if (TotalTickets >= Hammers[EquippedHammer].TicketCost)
    //        {
    //            OverwriteTicketAmount(TotalTickets - Hammers[EquippedHammer].TicketCost);
    //            Hammers[EquippedHammer].Unlocked = true;
    //            EquippedHammer = HammerNumber;
    //        }
    //    }
    //    OverwriteEqippedHammer(EquippedHammer);
    //}


    //public void SetStartHammer()
    //{
    //    try
    //    {
    //        GameObject HammerParent = GameObject.Find("Hammer (1)");
    //        Transform OriginalHammer = HammerParent.transform.Find("Hammerbase1");
    //        GameObject NewHammer = Instantiate(HammerObjects[EquippedHammer]);
    //        NewHammer.transform.parent = OriginalHammer.parent;
    //        NewHammer.transform.position = OriginalHammer.position;
    //        NewHammer.transform.rotation = OriginalHammer.rotation;
    //        NewHammer.transform.localScale = OriginalHammer.localScale;
    //    }
    //    catch { }
    //}


    ////OVERWRITES THE TICKET AMOUNT 
    //public void OverwriteEqippedHammer(int CurrentEquippedHammer)
    //{
    //    int EquippedHammer = CurrentEquippedHammer;
    //    StreamWriter stream = new StreamWriter(hammer_filePath);
    //    string json = JsonUtility.ToJson(EquippedHammer, true);
    //    stream.Write(json);
    //    stream.Close();
    //}
    ////RETURNS THE AMOUNT IN THE FILE
    //public int ReadEquippedHammer()
    //{
    //    if (!File.Exists(hammer_filePath))
    //        return -1;

    //    StreamReader stream = new StreamReader(hammer_filePath);
    //    string jsonData = stream.ReadToEnd();
    //    int EquippedHammer = JsonUtility.FromJson<int>(jsonData);
    //    stream.Close();
    //    return EquippedHammer;
    //}

    //public GameObject GetEquippedHammer()
    //{
    //    return Hammers[EquippedHammer].HammerObject;
    //}


    #endregion

    #endregion
}