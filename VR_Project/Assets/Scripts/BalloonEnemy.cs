using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class BalloonEnemy : MonoBehaviour
{
    public float bounceAmount = 0.6f;
    public float bounceSpeed = 0.2f;
    private float yOffset = 0f;
    bool move = true;
    private float delay = 0f;
    public GameObject ballonDefault = null;
    public GameObject mainBodyDefault = null;
    public GameObject ballonShatterVersion = null;
    public GameObject mainBodyShatter = null;
   
    /*
     * TO ADD
     * Particle effect for string snap, balloon pop and enemy hit
     */

    public void Start()
    {
        delay = Random.Range(-1.0f, 1.0f);
        yOffset = transform.position.y;
    }

    // Update is called once per frame
    public void Update()
    {
        if (move)
        transform.position = new Vector3(
            transform.position.x,
            yOffset + (Mathf.Sin(Time.time * bounceSpeed) * delay * bounceAmount) ,
            transform.position.z);

    }

    public void BalloonHit()
    {
        //decouple the main body and add gravity or whatever
        //explode the balloon
        ballonDefault.SetActive(false);
        mainBodyDefault.AddComponent<Rigidbody>();
        mainBodyDefault.transform.parent = null;
        move = false;
    }
    public void MainBodyHit()
    {
        //for now this parent null makes it so only when its attached to balloon will it shatter
        if (mainBodyDefault.transform.parent != null)
        {
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
