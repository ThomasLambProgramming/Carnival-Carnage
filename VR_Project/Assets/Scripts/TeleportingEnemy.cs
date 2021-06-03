using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TeleportingEnemy : MonoBehaviour
{
    //layer 9 
    //once every this many seconds it will teleport 
    public float TeleportRate = 5f;
    public float spinSpeed = 10f;
    private float teleportTimer = 0;
    //this makes sure it doesnt teleport into ground or anything
    public GameObject[] telePortPoints;


    void Update()
    {
        teleportTimer += Time.deltaTime;
        if (teleportTimer >= TeleportRate)
        {
            //play particle effect
            if (telePortPoints.Length > 0)
            {
                transform.position = telePortPoints[Random.Range(0, telePortPoints.Length - 1)].transform.position;
            }
            teleportTimer = 0;
        }
        transform.Rotate(new Vector3(0, spinSpeed * Time.deltaTime, 0));
    
        
    
    
    
    }


    


}
