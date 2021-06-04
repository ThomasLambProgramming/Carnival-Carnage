using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObjPlane : MonoBehaviour
{
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Hammer")
        {
            Destroy(other.gameObject);
        }
    }
}
