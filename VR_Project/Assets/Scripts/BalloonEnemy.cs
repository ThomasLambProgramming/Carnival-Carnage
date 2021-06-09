using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonEnemy : MonoBehaviour
{
    public float bounceAmount = 0.6f;
    public float bounceSpeed = 0.2f;
    private float yOffset = 0f;
    bool move = true;
    private float delay = 0f;
    public GameObject ballonShatterVersion = null;
    public GameObject headShatterVersion = null;
   
    /*
     * TO ADD
     * Particle effect for string snap, balloon pop and enemy hit
     */

    void Start()
    {
        delay = Random.Range(-1.0f, 1.0f);
        yOffset = transform.position.y;
    }

    // Update is called once per frame
    void Update()
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
        move = false;
    }
    public void MainBodyHit()
    {

        //add to score
        //explode the balloon
        move = false;
    }
}
