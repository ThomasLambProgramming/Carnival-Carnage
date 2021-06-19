using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
* File: CrateDestroy.cs
*
* Author: Thomas Lamb (s200498@students.aie.edu.au)
* Date Created: 10rd June 2021
* Date Last Modified: 12th June 2021
*
* Crate destructable script
* 
*/
public class CrateDestory : MonoBehaviour
{
    //I dont think this one needs and explaination
    //on hammer hit, deparent the destructable and set it active
    public float destroyTime = 3;
    public GameObject destructableVersion = null;

    public AudioManager audioManager = null;
    // Start is called before the first frame update
    public void HammerHit()
    {
        destructableVersion.transform.parent = null;
        destructableVersion.SetActive(true);
        gameObject.SetActive(false);
        Destroy(gameObject, destroyTime);
        Destroy(destructableVersion, destroyTime);
        //audio.PlaySound("", transform.position);
    }
}
