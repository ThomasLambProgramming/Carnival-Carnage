using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
