using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TeleportingEnemy : MonoBehaviour
{
    //layer 9 
    //once every this many seconds it will teleport 
    public float TeleportRate = 5f;
    public float spinSpeed = 10f;
    private float teleportTimer = 0;
    //this makes sure it doesnt teleport into ground or anything
    public GameObject[] telePortPoints;
    public GameObject destructableVersion = null;
    private bool hasBeenHit = false;
    void Update()
    {
        //if it has been hit we dont want it rotating or teleporting after
        if (!hasBeenHit)
        {
            teleportTimer += Time.deltaTime;
            if (teleportTimer >= TeleportRate)
            {
                //play particle effect
                if (telePortPoints.Length > 0)
                {
                    transform.position = telePortPoints[Random.Range(0, telePortPoints.Length - 1)].transform.position;
                }
                teleportTimer = 0;
            }
            transform.Rotate(new Vector3(0, spinSpeed * Time.deltaTime, 0));
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Hammer"))
        {
            //get hammer velocity or probs just increase the inital velocity 
            //as the hammer will already move it the way we want it too
            hasBeenHit = true;
        }
        if (collision.transform.CompareTag("Enemy"))
        {
            //disableGameobject
            gameObject.SetActive(false);

            //AddScore / ticket thingy
            
            //enable shatter version and do all the force stuff required
            destructableVersion.SetActive(true);
        }
    }





}
