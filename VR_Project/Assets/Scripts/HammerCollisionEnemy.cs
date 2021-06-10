using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HammerCollisionEnemy : MonoBehaviour
{

    public float yForceIncrease = 4f;
    public float meleeHitForce = 200f;
    public float throwHitForce = 10f;

    private Vector3 previousPosition;
    //give it a default so a null does not have to be done
    private Vector3 bufferPosition = new Vector3(0, 0, 0);
    public GameObject hammerPart;
    public int frameSkipsOnUpdate = 2;
    private int amountSkipped = 0;

    XRGrabInteractable grabScript = null;
    private Rigidbody hammerRb = null;
    public void Start()
    {
        hammerRb = GetComponent<Rigidbody>();
        grabScript = GetComponent<XRGrabInteractable>();
    }
    public void FixedUpdate()
    {
        amountSkipped++;
        if (amountSkipped >= frameSkipsOnUpdate)
        {
            //this is done so there is a delay 
            previousPosition = bufferPosition;
            bufferPosition = hammerPart.transform.position;
            amountSkipped = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            Vector3 hammerDifference = hammerPart.transform.position - previousPosition;
            float amountMoved = hammerRb.velocity.magnitude;
            Vector3 forceDirection = hammerDifference.normalized;
            forceDirection.y += yForceIncrease;

            Vector3 forceToAdd = forceDirection * amountMoved;

            
            if (grabScript.isSelected)
            {
                forceToAdd *= meleeHitForce;
                collision.transform.GetComponent<WalkerEnemy>().HasBeenHit(forceToAdd);
            }
            else
            {
                forceToAdd *= throwHitForce;
                collision.transform.GetComponent<WalkerEnemy>().HasBeenHit(forceToAdd);
            }
        }
        else if (collision.transform.CompareTag("BalloonBody"))
        {
            collision.transform.parent.GetComponent<BalloonEnemy>().MainBodyHit();
        }
        else if (collision.transform.CompareTag("Balloon"))
        {
            collision.transform.parent.GetComponent<BalloonEnemy>().BalloonHit();
        }
        else if (collision.transform.CompareTag("Prankster"))
        {
            Vector3 hammerDifference = hammerPart.transform.position - previousPosition;
            float amountMoved = hammerRb.velocity.magnitude;
            Vector3 forceDirection = hammerDifference.normalized;
            forceDirection.y += yForceIncrease;

            Vector3 forceToAdd = forceDirection * amountMoved;



            if (grabScript.isSelected)
            {
                forceToAdd *= meleeHitForce;
                collision.transform.GetComponent<Pranksters>().HasBeenHit(forceToAdd);
            }
            else
            {
                forceToAdd *= throwHitForce;
                collision.transform.GetComponent<Pranksters>().HasBeenHit(forceToAdd);
            }
        }
    }
}
