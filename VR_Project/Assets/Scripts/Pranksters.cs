using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
* File: Pranksters.cs
*
* Author: Thomas Lamb (s200498@students.aie.edu.au)
* Date Created: 10th June 2021
* Date Last Modified: 12th June 2021
*
* Teleporting enemy class
* 
*/
public class Pranksters : MonoBehaviour
{
    //public GameObject gameManager;


    //layer 9 
    //once every this many seconds it will teleport 
    public float TeleportRate = 5f;
    public float spinSpeed = 10f;
    private float teleportTimer = 0;
    //this makes sure it doesnt teleport into ground or anything
    public GameObject[] telePortPoints;
    public GameObject headObject = null;
    private bool hasBeenHit = false;
    private bool isDestroyed = false;
    private Rigidbody headRb = null;
    private BoxCollider headCollider = null;
    private BoxCollider mainCollider = null;
    public AudioManager audioManager = null;

    private bool chargeParticleHappened = false;
    public ParticleSystem pranksterchargeParticle;
    public ParticleSystem pranksterExplosionParticle;
    private void Start()
    {
        mainCollider = GetComponent<BoxCollider>();
        headCollider = headObject.GetComponent<BoxCollider>();
        headRb = headObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        //if it has been hit we dont want it rotating or teleporting after
        if (!hasBeenHit)
        {
            teleportTimer += Time.deltaTime;
            
            //play particle 1 second before teh teleport occurs
            if (((TeleportRate - teleportTimer) <= 1) && chargeParticleHappened == false)
            {
                //i am commenting this out because it is 1.30am and i have made it so no errors appear
                //in the unity console at all and i am not having this be my downfall but i wont remove the line
                //in case the designers get angry about it for some reason
                //count one of designers not looking at audio file names when trying to call them
                //audioManager.PlaySound("PranksterCharge", gameObject);
                pranksterchargeParticle.Play();
                chargeParticleHappened = true;
            }

            if (teleportTimer >= TeleportRate)
            {
                
                //play particle effect
                audioManager.PlaySound("Enemy - Prankster", gameObject);
                if (telePortPoints.Length > 0)
                {
                    transform.position = telePortPoints[Random.Range(0, telePortPoints.Length - 1)].transform.position;
                    //play second explosion particle and sound
                    //Count two of designers not looking at audio file names
                    //audioManager.PlaySound("Prankster arrives", gameObject);
                    pranksterExplosionParticle.Play();
                }
                teleportTimer = 0;
                chargeParticleHappened = false;
            }
            //constant rotation
            transform.Rotate(new Vector3(0, spinSpeed * Time.deltaTime, 0));
        }
    }

    public void HasBeenHit(Vector3 forceToHit)
    {
        if (!isDestroyed)
        {
            FindObjectOfType<GameManager>().AddTime(gameObject, 3);
            isDestroyed = true;
        }

        //disable what isnt needed, make the head deparent and hit it with the required force
        hasBeenHit = true;
        mainCollider.enabled = false;
        headCollider.enabled = true;
        //deparent it so it doesnt move with the object ontop of it flying in the air
        headObject.transform.parent = null;
        headRb.isKinematic = false;
        headRb.AddForce(forceToHit);
        Destroy(gameObject, 4f);

    }
}
