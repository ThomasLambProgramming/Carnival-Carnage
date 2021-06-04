using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerTest : MonoBehaviour
{
    public GameObject hammer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = hammer.transform.position;
        transform.rotation = hammer.transform.rotation;
    }
}
