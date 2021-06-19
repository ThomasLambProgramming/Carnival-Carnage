using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Random = UnityEngine.Random;
/*
* File: PropellerEnemy.cs
*
* Author: Thomas Lamb (s200498@students.aie.edu.au)
* Date Created: 3rd June 2021
* Date Last Modified: 12th June 2021
*
* Propeller enemy movement and hit management 
* 
*/
public class PropellerEnemy : MonoBehaviour
{
    public GameObject propellerFan = null;
    //hat fly away speed
    public float flyAwaySpeed = 40f;
    //propeller spin speed
    public float fanSpinSpeed = 5f;
    public GameObject shatterVersion = null;
    public GameObject defaultHead = null;
    //the hatobject had to be made because the propeller was not
    //perfectly centered so it didnt rotate properly
    public GameObject hatObject = null;
    private bool flyAway = false;
    //pathfinding data
    private Vector3[] path;
    public NodeContainer pathData = null;
    private int currentIndex = 0;
    public float goNextDistance = 3;
    
    
    public float destroyTime = 3f;
    public bool shatterHead = false;
    public float moveSpeed = 5;
    //y height we want it at
    public float yOffSet = 1;
    //do we want it to pathfind
    public bool canMove = true;
    private bool isDestroyed = false;
    private Rigidbody rb = null;
    //how quickly does the enemy rotate to the next pathfinding point
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
        //always rotate the propeller
        if (propellerFan != null)
            propellerFan.transform.Rotate(new Vector3(0, fanSpinSpeed * Time.deltaTime, 0));
        
        //if we want the hat to fly away
        if (flyAway)
        {
            //increase the y of the hat object
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
            //get the direction to the next path point
            Vector3 moveDirection = (path[currentIndex] - transform.position).normalized;
            //move towards
            rb.velocity = moveDirection * moveSpeed;
            //slowly rotate towards the next point so it looks somewhat natural 
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
        //this function is for if the head is hit not the hat
        if (!isDestroyed)
        {
            FindObjectOfType<GameManager>().AddTime(gameObject, 3);
            isDestroyed = true;
        }

        audioManager.PlaySound("Shatter", gameObject);
        //we dont want it to move anymore
        canMove = false;
        //disable the object and make the head non parented
        defaultHead.SetActive(false);
        shatterVersion.transform.parent = null;
        shatterVersion.SetActive(true);
        //make the propeller fly away 
        flyAway = true;
        
        Destroy(shatterVersion, destroyTime);
        Destroy(gameObject, destroyTime);
    }

    public void HatHit()
    {
        if (!isDestroyed)
        {
            FindObjectOfType<GameManager>().AddTime(gameObject, 3);
            isDestroyed = true;
        }
        
        //this was much easier then the hat
        //add a rigidbody to the head and let it drop
        Destroy(hatObject);
        defaultHead.AddComponent<Rigidbody>();
        defaultHead.transform.parent = null;
        Destroy(gameObject, destroyTime);
    }
    private Vector3[] FindPath()
    {
        //if no nodes can be found
        if (pathData.NodeGraph == null)
        {
            Debug.LogWarning("There is no node graph! Please create one from the node window" +
                             " Window/NodeGraph.");
            return null;
        }
        
        
        //new pathfinding job
        Vector3[] path;
        PathFindJob pathfind = new PathFindJob();
        pathfind.NodeData = pathData.NodeGraph;

        //index for the start and end node in the array
        int[] StartEndIndex = { 0, 0 };

        //for the ai who are simple we just get the closest and a random node
        StartEndIndex[0] = FindClosestNode(transform.position);
        StartEndIndex[1] = Random.Range(0, pathData.NodeGraph.Length - 1);
        //stops overlaps of the same node as start and end so it will always be at least some distance away
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
        //give the array all the data we need
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

        //make the distance really high 
        int closestNode = -1;
        float distance = 1000000;

        //get the smallest distance over time.
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

