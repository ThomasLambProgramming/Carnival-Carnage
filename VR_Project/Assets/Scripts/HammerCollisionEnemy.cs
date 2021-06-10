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
    private Vector3 bufferPosition = new Vector3(0,0,0);


    XRGrabInteractable grabScript = null;
    private Rigidbody hammerRb = null;
    public void Start()
    {
        hammerRb = GetComponent<Rigidbody>();
        grabScript = GetComponent<XRGrabInteractable>();
    }
    public void FixedUpdate()
    {
        //this is done so there is a delay 
        previousPosition = bufferPosition;        
        bufferPosition = transform.position;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("BalloonBody"))
        {
            collision.transform.parent.GetComponent<BalloonEnemy>().MainBodyHit();
        }
        if (collision.transform.CompareTag("Balloon"))
        {
            collision.transform.parent.GetComponent<BalloonEnemy>().BalloonHit();
        }
        if (collision.transform.CompareTag("Enemy"))
        {
            Vector3 hammerDifference = transform.position - previousPosition;
            float amountMoved = hammerDifference.magnitude;
            Vector3 forceDirection = hammerDifference.normalized;

            Vector3 forceToAdd = forceDirection * amountMoved;
            forceToAdd.y += yForceIncrease;


            if (grabScript.isSelected)
            {
                forceToAdd = forceToAdd * meleeHitForce;
                collision.transform.GetComponent<WalkerEnemy>().HasBeenHit(forceToAdd);
            }
            else
            {
                forceToAdd = forceToAdd * throwHitForce;
                collision.transform.GetComponent<WalkerEnemy>().HasBeenHit(forceToAdd);
            }
        }
    }
}
