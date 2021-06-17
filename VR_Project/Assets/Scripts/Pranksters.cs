using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
                    //play second explosion particle
                    pranksterExplosionParticle.Play();
                }
                teleportTimer = 0;
                chargeParticleHappened = false;
            }
            transform.Rotate(new Vector3(0, spinSpeed * Time.deltaTime, 0));
        }
    }

    public void HasBeenHit(Vector3 forceToHit)
    {
        FindObjectOfType<GameManager>().AddTime(gameObject, 3);
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
