using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
* File: PropellerShatter.cs
*
* Author: Thomas Lamb (s200498@students.aie.edu.au)
* Date Created: 3rd June 2021
* Date Last Modified: 12th June 2021
*
* propeller head shatter script
* 
*/
public class PropellerShatter : MonoBehaviour
{
    public GameObject shatterVersion = null;
    public AudioManager audioManager = null;
    public ParticleSystem onDeathParticle;
    private void OnCollisionEnter(Collision collision)
    {
        if (transform.parent == null)
        {
            if (collision.transform.CompareTag("Enemy") ||
                collision.transform.CompareTag("Ground") ||
                collision.transform.CompareTag("Obstacle"))
            {
                //disable the gameobject so nothing weird happens (and balloon "pops")
                gameObject.SetActive(false);
                if (shatterVersion.transform.parent != null)
                    shatterVersion.transform.parent = null;
                //make sure its deparented so it doesnt go inactive and can be on its own
                shatterVersion.SetActive(true);
                onDeathParticle.Play();
                Destroy(gameObject);
                Destroy(shatterVersion, 3);
                audioManager.PlaySound("Shatter", gameObject);
            }
        }
    }
}