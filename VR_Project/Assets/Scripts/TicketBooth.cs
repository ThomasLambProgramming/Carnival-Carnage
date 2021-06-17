﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicketBooth : MonoBehaviour
{
    public GameObject Set1;
    public GameObject Set2;
    public GameObject Shutters;
    public AudioSource shutterSound;
    // Start is called before the first frame update
    void Start()
    {
        shutterSound = GetComponent<AudioSource>();     
    }
    public void SwapPage()
    {
        StartCoroutine("CloseShutters");
    }
    IEnumerator CloseShutters()
    {
        Shutters.SetActive(true);
        shutterSound.Play();
        yield return new WaitForSeconds(0.5f);
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
        yield return new WaitForSeconds(1.5f);
        Shutters.SetActive(false);
        shutterSound.Play();
    }
}
