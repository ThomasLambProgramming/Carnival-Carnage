using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
* File: HeadShatter.cs
*
* Author: Thomas Lamb (s200498@students.aie.edu.au)
* Date Created: 3rd June 2021
* Date Last Modified: 12th June 2021
*
* Head shattering script for a few of the enemies
* 
*/
public class HeadShatter : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject shatterVersion = null;
    public AudioManager audioManager = null;
    public ParticleSystem onDeathParticle;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy") || 
            collision.transform.CompareTag("Ground") || 
            collision.transform.CompareTag("Obstacle"))
        {
            //disable the head
            //make the shatter version not have a parent
            //set it to be in the same position and rotation as the original head
            //as a precaution
            //play sound then delete after a set time
            gameObject.SetActive(false);
            if (shatterVersion.transform.parent != null)
                shatterVersion.transform.parent = null;
            shatterVersion.transform.position = transform.position;
            shatterVersion.transform.rotation = transform.rotation;
            shatterVersion.SetActive(true);
            onDeathParticle.Play();
            audioManager.PlaySound("Shatter", gameObject);
            Destroy(shatterVersion, 3);
        }
    }
}
