using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Score_Board : MonoBehaviour
{
    private TextMeshPro TimerText;
    private TextMeshPro ScoreText;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Timer")
            {
                try { TimerText = transform.GetChild(i).GetComponent<TextMeshPro>(); }
                catch { }
            }
        }
    }

    void Update()
    {
        //TimerText.text = Timer.time;
    }
}
