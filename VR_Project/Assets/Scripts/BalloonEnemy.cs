using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonEnemy : MonoBehaviour
{
    public float bounceAmount = 0.6f;
    public float bounceSpeed = 0.2f;
    private float yOffset = 0f;
   
    void Start()
    {
        yOffset = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            transform.position.x, 
            yOffset + (Mathf.Sin(Time.time * bounceSpeed) * bounceAmount),
            transform.position.z);
    }
}
