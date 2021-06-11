using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public static string time;
    public bool isFinished = false;
    public float timeRemaining = 120;

    public Text debug;

    private void Update()
    {
        if (timeRemaining > 0 && !isFinished)
        {
            timeRemaining -= Time.deltaTime;
            SetTimer(timeRemaining);
        }
    }

    private void SetTimer(float a_time)
    {
        float minutes = (int)a_time / 60;
        float seconds = a_time % 60;

        time = string.Format("{0:00}:{1:00}", minutes, seconds);

        debug.text = time;
    }
}
