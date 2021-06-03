using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingHammerThomas : MonoBehaviour
{
    public Transform handTransform = null;
    private Rigidbody rb = null;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = handTransform.position + (handTransform.up * 0.5f);
        transform.rotation = handTransform.rotation;
        transform.rotation = new Quaternion(transform.rotation.x + 60, transform.rotation.y, transform.rotation.z, transform.rotation.w);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            collision.transform.GetComponent<TempEnemy>().HasBeenHit();
            
        }
    }
}
