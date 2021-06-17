using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BalloonMainBody : MonoBehaviour
{
    public GameObject destructibleVersion = null;
    public AudioManager audiomanager = null;
    public ParticleSystem onDeathParticle;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Ground") || collision.gameObject.layer == 11)
        {
            gameObject.SetActive(false);
            destructibleVersion.transform.position = transform.position;
            destructibleVersion.SetActive(true);
            audiomanager.PlaySound("Shatter", gameObject);
            onDeathParticle.Play();
        }
    }
}
