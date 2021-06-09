using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerCollisionEnemy : MonoBehaviour
{
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
    }
}
