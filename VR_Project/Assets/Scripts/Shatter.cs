using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shatter : MonoBehaviour
{
    //public particle effect
    public GameObject shatterVersion;
    public float shatterForce = 10f;
    public string[] TagsToCheck;
    public bool hasCollided = false;
    public bool launchable = false;
    private bool beenHit = false;

    //public float timer = 4f;
    //private float timeCount = 0;

    private void OnCollisionEnter(Collision collision)
    {
        ShatterInvoke(collision);
    }
    public void Update()
    {
        /*
         Destructable versions of enemies
         hammer interaction with enemies
         launching of enemies when hit with particle effect and sound
         spider walking working with wall climbing
        */






        //timeCount += Time.deltaTime;
        //if (timeCount >= timer)
        //{
        //    gameObject.SetActive(false);
        //    //GetComponent<SphereCollider>().enabled = false;
        //    //GetComponent<MeshRenderer>().enabled = false;
        //    shatterVersion.SetActive(true);
        //    shatterVersion.transform.position = transform.position;
        //    //play particle effect
        //    for (int i = 0; i < shatterVersion.transform.childCount; i++)
        //    {
        //        Transform temp = shatterVersion.transform.GetChild(i);
        //        temp.GetComponent<Rigidbody>().AddForce(temp.up * shatterForce);
        //    }
        //    Destroy(shatterVersion, 2f);
        //    Destroy(gameObject, 0.3f);
        //}
    }

    private void ShatterInvoke(Collision collision)
    {
        //this is to make it so if its launchable it can interact with the ground
        //without shattering early with no hammer hit
        if (launchable)
        {
            if (collision.transform.CompareTag("Hammer"))
            {
                beenHit = true;
                return;
            }
            //makes it when its stationary and not hit it doesnt check the ground
            //that its probably on
            if (beenHit == false)
            {
                return;
            }
        }
        foreach (string tagName in TagsToCheck)
        {
            if (collision.transform.CompareTag(tagName))
            {
                hasCollided = true;
                //make it so this object is still active so it still runs but does not interfere 

                //get collider and disable
                GetComponent<SphereCollider>().enabled = false;
                GetComponent<MeshRenderer>().enabled = false;

                //play particle effect
                for (int i = 0; i < shatterVersion.transform.childCount; i++)
                {
                    Transform temp = shatterVersion.transform.GetChild(i);
                    temp.GetComponent<Rigidbody>().AddForce(temp.up * shatterForce);
                }
                Destroy(shatterVersion, 2f);
                Destroy(gameObject, 0.3f);
                break;
            }
        }
    }
}
