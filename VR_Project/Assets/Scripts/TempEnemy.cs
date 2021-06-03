using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class TempEnemy : MonoBehaviour
{
    private Rigidbody rb = null;
    private NavMeshAgent navmesh = null;
    public bool makePath = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        navmesh = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (navmesh.hasPath == false)
        {
            if (makePath)
            navmesh.SetDestination(new Vector3(Random.Range(-12, 12), 0.1f, Random.Range(-12, 12)));
        }
    }
    public void HasBeenHit()
    {
        navmesh.enabled = false;
        Destroy(gameObject, 4f);
    }
}
