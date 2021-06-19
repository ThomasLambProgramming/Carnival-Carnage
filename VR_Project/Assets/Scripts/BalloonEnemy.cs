using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/*
* File: BalloonEnemy.cs
*
* Author: Thomas Lamb (s200498@students.aie.edu.au)
* Date Created: 10rd June 2021
* Date Last Modified: 12th June 2021
*
* Balloon Enemy script 
* 
*/
public class BalloonEnemy : MonoBehaviour
{
    public AudioManager audioManager = null;
    // up and down movement amount and speed
    public float bounceAmount = 0.6f;
    public float bounceSpeed = 0.2f;
    //y offset makes sure it doesnt gain or lose y while moving up and down
    private float yOffset = 0f;
    bool move = true;
    bool isDestroyed = false;
    private float delay = 0f;
    public GameObject ballonDefault = null;
    public GameObject mainBodyDefault = null;
    //this wasnt used but i dont want to change anything now incase references get destroyed by a script reload
    public GameObject ballonShatterVersion = null;
    public GameObject mainBodyShatter = null;
    
    public void Start()
    {
        //make it slightly random because of sin(time) they would
        //all move the same amount 
        delay = Random.Range(-1.0f, 1.0f);
        yOffset = transform.position.y;
    }

    // Update is called once per frame
    public void Update()
    {
        if (move)
        {
            //set the transform be its set value except for the y
            transform.position = new Vector3(
                transform.position.x,
                //time * bounce speed speeds up the sin, delay changes the amount of sin gets done * the bounce amount
                yOffset + (Mathf.Sin(Time.time * bounceSpeed) * delay * bounceAmount),
                transform.position.z);
        }
    }

    public void BalloonHit()
    {
        if (!isDestroyed)
        {
            FindObjectOfType<GameManager>().AddTime(gameObject, 3);
            isDestroyed = true;
        }

        //decouple the main body and add gravity or whatever
        //explode the balloon
        ballonDefault.SetActive(false);
        audioManager.PlaySound("Balloon Pop", ballonDefault);
        mainBodyDefault.AddComponent<Rigidbody>();
        mainBodyDefault.transform.parent = null;
        move = false;
        Destroy(gameObject, 4);
    }
    public void MainBodyHit()
    {
        //for now this parent null makes it so only when its attached to balloon will it shatter
        if (mainBodyDefault.transform.parent != null)
        {
            if (!isDestroyed)
            {
                FindObjectOfType<GameManager>().AddTime(gameObject, 3);
                isDestroyed = true;
            }

            audioManager.PlaySound("Shatter", gameObject);
            mainBodyDefault.SetActive(false);
            mainBodyShatter.SetActive(true);
            ballonDefault.SetActive(false);
            Destroy(gameObject, 4);
            //add to score
            //explode the balloon
            move = false;
        }
    }
}
