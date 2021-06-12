using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateDestory : MonoBehaviour
{
    public float destroyTime = 3;
    public GameObject destructableVersion = null;
    // Start is called before the first frame update
    public void HammerHit()
    {
        destructableVersion.transform.parent = null;
        destructableVersion.SetActive(true);
        gameObject.SetActive(false);
        Destroy(gameObject, destroyTime);
        Destroy(destructableVersion, destroyTime);
    }
}
