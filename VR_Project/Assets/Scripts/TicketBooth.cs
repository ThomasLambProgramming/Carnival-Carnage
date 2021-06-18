using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class TicketBooth : MonoBehaviour
{
    public GameObject Set1;
    public GameObject Set2;
    public AudioSource shutterSound;
    //private bool played = false;

    // Start is called before the first frame update
    void Start()
    {
        shutterSound = GetComponent<AudioSource>();
    }

    public void SwapPage()
    {
        if (Set1.activeInHierarchy == true)
        {
            Set1.SetActive(false);
            Set2.SetActive(true);
        }
        else
        {
            Set1.SetActive(true);
            Set2.SetActive(false);
        }
    }
}
