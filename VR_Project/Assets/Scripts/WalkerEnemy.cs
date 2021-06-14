using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class WalkerEnemy : MonoBehaviour
{
    public AudioManager audioSources = null;
    private Rigidbody rb = null;
    private NavMeshAgent navmesh = null;
    public bool makePath = true;
    public float distanceToNewPath = 2f;
    public GameObject headObject = null;
    private Rigidbody headRb = null;
    public bool kill = false;
    private BoxCollider mainCollider = null;
    private BoxCollider headCollider = null;

    public NodeContainer nodeData = null;
    // Start is called before the first frame update
    void Start()
    {
        mainCollider = GetComponent<BoxCollider>();
        headCollider = headObject.GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        navmesh = GetComponent<NavMeshAgent>();
        headRb = headObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (navmesh != null && navmesh.enabled && makePath)
        {
            if (Vector3.Magnitude(navmesh.destination - transform.position) < distanceToNewPath * distanceToNewPath)
                navmesh.SetDestination(nodeData.NodeGraph[Random.Range(0, nodeData.NodeGraph.Length - 1)].m_position);
        }
        if (kill)
        {
            HasBeenHit(new Vector3(0, 100, 0));
            kill = false;
        }

    }
    public void HasBeenHit(Vector3 forceToHit)
    {
        //there isnt a hit sound soooo.....
        //audioSources.PlaySound("Hit", transform.position);
        navmesh.enabled = false;
        mainCollider.enabled = false;
        headCollider.enabled = true;
        //deparent it so it doesnt move with the object ontop of it flying in the air
        headObject.transform.parent = null;
        headRb.isKinematic = false;
        headRb.AddForce(forceToHit);
        Destroy(gameObject, 4f);
    }
}
