using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class WalkerEnemy : MonoBehaviour
{
    private Rigidbody rb = null;
    private NavMeshAgent navmesh = null;
    public bool makePath = true;
    public float distanceToNewPath = 2f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        navmesh = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (makePath)
        {
            if (Vector3.Magnitude(navmesh.destination - transform.position) < distanceToNewPath * distanceToNewPath)
                navmesh.SetDestination(new Vector3(Random.Range(-18, 2), 1, Random.Range(-35, -7)));
        }

    }
    public void HasBeenHit()
    {
        navmesh.enabled = false;
        Destroy(gameObject, 4f);
    }
}
