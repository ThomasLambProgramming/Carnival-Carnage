using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.Collections;

public class HammerCollisionEnemy : MonoBehaviour
{
    //this makes sure that the hammer doesnt go into the ground on the recall
    public float yOffset = 1f;
    public NodeContainer pathData = null;
    public float yForceIncrease = 4f;
    //hitting enemy force to launch head
    public float meleeHitForce = 200f;
    public float throwHitForce = 10f;
    
    //private Vector3 SummonPosition;
    //bool firstSummon = true;
    private Vector3 previousPosition;
    //give it a default so a null does not have to be done
    private Vector3 bufferPosition = new Vector3(0, 0, 0);
    //hammerpart was used but with changes its just the hammer itself
    public GameObject hammerPart;
    //frame skips so we can get the average movement over time for the launching head direction
    public int frameSkipsOnUpdate = 2;
    private int amountSkipped = 0;
    
    public GameObject playerRightHand = null;
    //makes it so the player cant super spam the recall and cancel
    public float summonTime = 0.5f;
    public float withInGrabDistance = 1f;
    //pathfinding variables
    public float goNextNodeDist = 2f;
    private int currentPathIndex = 0;
    bool usePath = false;
    private bool isBeingSummoned = false;
    private Vector3[] path;
   
    //return speed is not used but hammers have a different value so i dont want to touch and break it
    public float returnspeed = 10f;
    
    public float maxReturnSpeed = 50f;
    XRGrabInteractable grabScript = null;
    private Rigidbody hammerRb = null;
    //have a seperate maxspeed so the hammer cant be thrown too hard 
    //if its not being recalled (its also to stop players using the recall and cancelling to "throw" the hammer
    //at a faster speed than they could throw
    public float maxSpeed = 20f;
    private float summonTimer = 0;
    public bool summon = false;
    public void Start()
    {
        hammerRb = GetComponent<Rigidbody>();
        grabScript = GetComponent<XRGrabInteractable>();
    }

    //recalling script
    public void IsSummoned()
    {
        isBeingSummoned = true;
        //if (firstSummon)
        //{
        //    SummonPosition = playerRightHand.transform.position;
        //    firstSummon = false;
        //}
    }
    public void StopSummon()
    {
        //cancel everything needed so update function doesnt run anything
        isBeingSummoned = false;
        summonTimer = 0;
        usePath = false;
        path = null;
        currentPathIndex = 0;
        //firstSummon = true;
    }
    public bool IsBeingHeld()
    {
        if (grabScript.isSelected)
        {
            //firstSummon = true;
            return true;
        }

        return false;
    }

    public void Update()
    {
        amountSkipped++;
        if (amountSkipped >= frameSkipsOnUpdate)
        {
            //this is done so there is a delay 
            previousPosition = bufferPosition;
            bufferPosition = hammerPart.transform.position;
            amountSkipped = 0;
        }
        //if its in the hand make the velocity 0 so it doesnt do that stupid throw glitch
        if (grabScript.isSelected)
        {
            isBeingSummoned = false;
            hammerRb.velocity = Vector3.zero;
            summonTimer = 0;
        }

        if (!isBeingSummoned)
        {
            //this is to allow for the hammer to return at great speeds but if the player 
            //cancels or throws it cant hit those same speeds (the recall when cancelled to late would go crazy)
            if (hammerRb.velocity.magnitude > maxSpeed)
            {
                hammerRb.velocity = hammerRb.velocity.normalized * maxSpeed;
            }
        }
        summonTimer += Time.deltaTime;
        //summon is a debugging option because aie is fun with their no software to allow for unity editor debugging
        if (isBeingSummoned && summonTimer > summonTime || summon)
        {
            Vector3 rightHandPos = playerRightHand.transform.position;
            rightHandPos.y = yOffset;
            var directionToHand = (rightHandPos - transform.position).normalized;
            RaycastHit hit;
            //small note, why does unitys find layer return the actual number of the layer when it requires the bit
            //2147482879 is for the 8th and 9th floor so it ignores all grab objects and the ground 2147482879
            //so i was very stupid and thought ah yes writing out the binary for the int and putting it in a calculator
            //would be faster than thinking about what bitwise would be needed, so yeah that number is correct but the bitwise makes more sense
            if (Physics.Raycast(transform.position, directionToHand, out hit, Mathf.Infinity, ~(3 << 8)) && !IsBeingHeld())
            {
                if (hit.transform.CompareTag("Obstacle"))
                {
                    if (path == null)
                        path = FindPath();

                    //else if (Vector3.Distance(SummonPosition, playerRightHand.transform.position) > reSummonDistance)
                    //{
                    //    path = FindPath();
                    //    firstSummon = true;
                    //}
                    currentPathIndex = 0;
                    usePath = true;
                }
                //if it hits the hand make the velocity half + direction because it makes it turn really well 
                //quick but not jarring im very happy with it
                else if (hit.transform.CompareTag("Hand"))
                {
                    hammerRb.velocity = hammerRb.velocity / 1.5f;
                    hammerRb.velocity += directionToHand * (maxReturnSpeed / 2);
                    path = null;
                    usePath = false;
                }
            }
            if (usePath)
            {
                //if no path get one and index is the start
                if (path == null)
                {
                    path = FindPath();
                    currentPathIndex = 0;
                }
                if (path != null)
                {
                    Vector3 indexPosition = path[currentPathIndex];
                    indexPosition.y = 1;
                    //makes sure the y isnt in ground
                    Vector3 forceDirection = (indexPosition - transform.position).normalized;
                    forceDirection = forceDirection * maxReturnSpeed;
                    hammerRb.velocity = forceDirection;
                    
                    //if close enough to position goto next or the path is null
                    if ((path[currentPathIndex] - transform.position).magnitude < goNextNodeDist)
                    {
                        if (currentPathIndex < path.Length - 1)
                        {
                            currentPathIndex++;
                        }
                        else if (currentPathIndex >= path.Length - 1)
                        {
                            currentPathIndex = 0;
                            path = null;
                        }
                    }
                }
            }
            //just a check at the end to make sure the hammer isnt going giga speeds
            if (hammerRb.velocity.magnitude > maxReturnSpeed)
            {
                hammerRb.velocity = hammerRb.velocity.normalized * maxReturnSpeed;
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            //for the walker since its on the floor i had to get the average velocity and increase the 
            //y of it to give a nicer launch angle (it kinda works but not the greatest)
            Vector3 hammerDifference = hammerPart.transform.position - previousPosition;
            float amountMoved = hammerRb.velocity.magnitude;
            Vector3 forceDirection = hammerDifference.normalized;
            forceDirection.y += yForceIncrease;

            Vector3 forceToAdd = forceDirection * amountMoved;

            //give a different launch speed depending on if melee or ranged
            if (grabScript.isSelected)
            {
                forceToAdd *= meleeHitForce;
                collision.transform.GetComponent<WalkerEnemy>().HasBeenHit(forceToAdd);
            }
            else
            {
                forceToAdd *= throwHitForce;
                collision.transform.GetComponent<WalkerEnemy>().HasBeenHit(forceToAdd);
            }
        }
        
        //every other function here just gets that objects hit function
        else if (collision.transform.CompareTag("Crate"))
        {
            collision.transform.GetComponent<CrateDestory>().HammerHit();
        }
        else if (collision.transform.CompareTag("BalloonBody"))
        {
            if (collision.transform.parent != null)
                collision.transform.parent.GetComponent<BalloonEnemy>().MainBodyHit();
        }
        else if (collision.collider.transform.CompareTag("Hat"))
        {
            collision.collider.transform.parent.GetComponent<PropellerEnemy>().HatHit();
        }
        else if (collision.collider.transform.CompareTag("PropellerHead"))
        {
            if (collision.collider.transform.parent != null)
                collision.collider.transform.parent.GetComponent<PropellerEnemy>().Shatter();
        }
        else if (collision.transform.CompareTag("Balloon"))
        {
            collision.transform.parent.GetComponent<BalloonEnemy>().BalloonHit();
        }
        //prankster has the same thing as the walker launch angle thing
        else if (collision.transform.CompareTag("Prankster"))
        {
            Vector3 hammerDifference = hammerPart.transform.position - previousPosition;
            float amountMoved = hammerRb.velocity.magnitude;
            Vector3 forceDirection = hammerDifference.normalized;
            forceDirection.y += yForceIncrease;

            Vector3 forceToAdd = forceDirection * amountMoved;
            
            if (grabScript.isSelected)
            {
                forceToAdd *= meleeHitForce;
                collision.transform.GetComponent<Pranksters>().HasBeenHit(forceToAdd);
            }
            else
            {
                forceToAdd *= throwHitForce;
                collision.transform.GetComponent<Pranksters>().HasBeenHit(forceToAdd);
            }
        }
    }
    //NOTE THIS PATHFINDING IS THE SAME AS THE PROPELLER ENEMY
    //i dont want to copy paste just incase i changed one slight thing in this hammer one
    //jesse please dont kill me i dont want to rewrite the comments for this
    //the comments are on the propeller enemy one 
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
        StartEndIndex[1] = FindClosestNode(playerRightHand.transform.position);

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

        return reversedPath;
    }

    public int FindClosestNode(Vector3 a_position)
    {
        if (pathData.NodeGraph == null)
            return -1;

        int closestNode = -1;
        float distance = 0;
        bool firstNode = true;

        for (int i = 0; i < pathData.NodeGraph.Length; i++)
        {
            if (firstNode)
            {
                distance = Vector3.Magnitude(pathData.NodeGraph[i].m_position - a_position);
                firstNode = false;
                closestNode = 0;
                continue;
            }
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
