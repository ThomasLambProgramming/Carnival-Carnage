using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                gameObject.SetActive(false);
                if (shatterVersion.transform.parent != null)
                    shatterVersion.transform.parent = null;
                shatterVersion.SetActive(true);
                onDeathParticle.Play();
                Destroy(gameObject);
                Destroy(shatterVersion, 3);
                audioManager.PlaySound("Shatter", gameObject);
            }
        }
    }
}