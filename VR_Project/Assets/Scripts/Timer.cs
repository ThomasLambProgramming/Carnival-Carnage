using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static string time { get; set; }
    public bool isFinished = false;
    public float timeRemaining = 120;

    public TextMeshProUGUI timerText;

    private void Start()
    {
        FindObjectOfType<AudioManager>().PlaySound("Timer");
    }

    private void Update()
    {
        if (timeRemaining > 0 && !isFinished)
        {
            timeRemaining -= Time.deltaTime;
            SetTimer(timeRemaining);
        }
        else
        {
            timeRemaining = 0;
            FindObjectOfType<AudioManager>().StopPlaying("Timer");
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

        time = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.text = time;
    }
}
