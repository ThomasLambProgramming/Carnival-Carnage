using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Random = UnityEngine.Random;

public class PropellerEnemy : MonoBehaviour
{
    public GameObject propellerFan = null;
    public float flyAwaySpeed = 40f;
    public float fanSpinSpeed = 5f;
    public GameObject shatterVersion = null;
    public GameObject defaultHead = null;
    public GameObject hatObject = null;
    private bool flyAway = false;
    private Vector3[] path;
    public NodeContainer pathData = null;
    public float destroyTime = 3f;
    private int currentIndex = 0;
    public bool shatterHead = false;
    public float goNextDistance = 3;
    public float moveSpeed = 5;
    public float yOffSet = 1;
    public bool canMove = true;
    private Rigidbody rb = null;
    public float rotateSpeed = 10f;
    
    public AudioManager audioManager = null;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (propellerFan != null)
            propellerFan.transform.Rotate(new Vector3(0, fanSpinSpeed * Time.deltaTime, 0));
        
        if (flyAway)
        {
            Vector3 hatPos = hatObject.transform.position;
            hatPos.y += flyAwaySpeed * Time.deltaTime;
            hatObject.transform.position = hatPos;
        }

        if (shatterHead)
        {
            Shatter();
            shatterHead = false;
        }

        if (path == null)
        {
            path = FindPath();
        }


    }

    private void FixedUpdate()
    {
        if (canMove && path != null)
        {
            Vector3 moveDirection = (path[currentIndex] - transform.position).normalized;
            rb.velocity = moveDirection * moveSpeed;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDirection), rotateSpeed);
            if (Vector3.Magnitude(path[currentIndex] - transform.position) < goNextDistance)
            {
                if (currentIndex < path.Length - 1)
                    currentIndex++;
                else
                {
                    path = null;
                    currentIndex = 0;
                }
            }
        }
    }

    public void Shatter()
    {
        audioManager.PlaySound("Shatter", gameObject);
        canMove = false;
        defaultHead.SetActive(false);
        shatterVersion.transform.parent = null;
        shatterVersion.SetActive(true);
        flyAway = true;
        Destroy(shatterVersion, destroyTime);
        Destroy(gameObject, destroyTime);
    }

    public void HatHit()
    {
        Destroy(hatObject);
        defaultHead.AddComponent<Rigidbody>();
        defaultHead.transform.parent = null;
        Destroy(gameObject, destroyTime);
    }
    private Vector3[] FindPath()
    {
        if (pathData.NodeGraph == null)
        {
            Debug.LogWarning("There is no node graph! Please create one from the node window" +
                             " Window/NodeGraph.");
            return null;
        }

        Vector3[] path;
        PathFindJob pathfind = new PathFindJob();
        pathfind.NodeData = pathData.NodeGraph;

        int[] StartEndIndex = { 0, 0 };

        StartEndIndex[0] = FindClosestNode(transform.position);
        StartEndIndex[1] = Random.Range(0, pathData.NodeGraph.Length - 1);
        while (StartEndIndex[1] == StartEndIndex[0])
        {
            StartEndIndex[1] = Random.Range(0, pathData.NodeGraph.Length - 1);
        }

        NativeArray<int> startEndPos = new NativeArray<int>(StartEndIndex, Allocator.Temp);
        pathfind.startEndPos = startEndPos;

        //test for path finding time will need to improve the memory grabbing though its a bit slow and can be grouped
        //float time = Time.realtimeSinceStartup;
        pathfind.Execute();
        //Debug.Log(Time.realtimeSinceStartup - time);
        if (!pathfind.pathResult.IsCreated)
        {
            if (startEndPos.IsCreated)
                startEndPos.Dispose();
            Debug.Log("Pathresult was null");
            return null;
        }

        path = pathfind.pathResult.ToArray();
        pathfind.pathResult.Dispose();
        startEndPos.Dispose();

        //the path that the main loop was giving back was in the reverse order
        Vector3[] reversedPath = new Vector3[path.Length];
        for (int i = 0; i < path.Length; i++)
        {
            reversedPath[i] = path[path.Length - (1 + i)];
        }
        for (int i = 0; i < path.Length; i++)
        {
            Vector3 newPos = reversedPath[i];
            newPos.y = yOffSet;
            reversedPath[i] = newPos;
        }

        return reversedPath;
    }

    public int FindClosestNode(Vector3 a_position)
    {
        if (pathData.NodeGraph == null)
            return -1;

        int closestNode = -1;
        float distance = 1000000;

        for (int i = 0; i < pathData.NodeGraph.Length; i++)
        {
            float nodeDist = Vector3.Magnitude(pathData.NodeGraph[i].m_position - a_position);
            if (nodeDist < distance)
            {
                distance = nodeDist;
                closestNode = i;
            }
        }

        return closestNode;
    }
}

