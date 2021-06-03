using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shatter : MonoBehaviour
{
    //public particle effect
    public GameObject shatterVersion;
    public float shatterForce = 10f;
    public string[] TagsToCheck;
    private void OnCollisionEnter(Collision collision)
    {
        foreach (string tagName in TagsToCheck)
        {
            if (collision.transform.CompareTag(tagName))
            {
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
                Destroy(shatterVersion, 1f);
                Destroy(gameObject, 0.3f);
                break;
            }
        }
    }
}
