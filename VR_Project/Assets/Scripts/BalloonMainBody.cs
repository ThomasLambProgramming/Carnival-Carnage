using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
* File: BalloonMainBody.cs
*
* Author: Thomas Lamb (s200498@students.aie.edu.au)
* Date Created: 10rd June 2021
* Date Last Modified: 12th June 2021
*
* Balloon body shattering script
* 
*/
public class BalloonMainBody : MonoBehaviour
{
    public GameObject destructibleVersion = null;
    public AudioManager audiomanager = null;
    public ParticleSystem onDeathParticle;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground") || collision.gameObject.layer == 11)
        {
            //if it hits the ground or the obstacle layer we want to explode 
            //disable object and set the destruct to where it currently is
            //set destruct to active and play a sound
            gameObject.SetActive(false);
            destructibleVersion.transform.position = transform.position;
            destructibleVersion.SetActive(true);
            audiomanager.PlaySound("Shatter", gameObject);
            onDeathParticle.Play();
        }
    }
}
