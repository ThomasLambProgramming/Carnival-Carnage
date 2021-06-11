﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public static string time { get; set; }
    public bool isFinished = false;
    public float timeRemaining = 120;

    public Text debug;

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
            time = "00:00";
            FindObjectOfType<AudioManager>().StopPlaying("Timer");
            FindObjectOfType<AudioManager>().StopPlaying("Timer");
        }
    }

    private void SetTimer(float a_time)
    {
        float minutes = Mathf.FloorToInt(a_time / 60);
        float seconds = Mathf.FloorToInt(a_time % 60);

        time = string.Format("{0:00}:{1:00}", minutes, seconds);

        debug.text = time;
    }
}
