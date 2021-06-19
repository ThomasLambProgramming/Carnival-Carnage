using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

/*
* File: TeleportController.cs
*
* Author: Thomas Lamb (s200498@students.aie.edu.au)
* Date Created: 3rd June 2021
* Date Last Modified: 12th June 2021
*
* Main Dashing controller, hammer management and 
* player input manager
* 
*/
public class TeleportController : MonoBehaviour
{
    //Object that has the teleporting controller script
    public GameObject PlayerObject = null;

    //reference to the input action reference that contains the button mapping data for activation
    public float dashDistance = 1f;
    public float dashSpeed = 2f;
    
    //These variables manage the returning of the hammer
    public GameObject hammerObject = null;
    private HammerCollisionEnemy hammerScript = null;
    public SphereCollider handCollider = null;

    //this is the minimum amount the player has to move the stick before a dash
    public float minLeftStickInput = 0.2f;

    //this bool makes sure the player cant spam controls to make the hammer break
    private bool firstCancel = true;

    //amount of time till a dash can be done again
    public float dashCooldown = 1f;
    private float dashCDTimer = 1;
    
    //get the hand object to obtain the direct interator for hammer management
    public GameObject rightHandObject = null;
    private XRDirectInteractor directInteractor = null;
    
    public float dashRaySearchLimit = 2f;
    
    //this is to have a set dash movement over time so it doesnt change until the next input
    private Vector3 moveDirection;
    bool isDashing = false;
    
    //if this is true in the inspector we can reset the current scene 
    public bool debugMode = false;

    //distance check for the dash
    private float amountDashed = 0f;
    
    //controller input managers 
    private InputDevice rightJoyStick;
    private InputDevice leftJoyStick;
    
    public GameObject headSetObject = null;

    public void Start()
    {
        //This gets the left and right controllers as a input device so we can use the tryGetFeature for inputs
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.Left;
        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, devices);
        if (devices.Count > 0)
        {
            leftJoyStick = devices[0];
        }

        devices.Clear();
        devices = new List<InputDevice>();
        InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right;
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);
        if (devices.Count > 0)
        {
            rightJoyStick = devices[0];
        }

        //get all the variables to perform the hammer recalling
        hammerObject.GetComponent<Rigidbody>();
        hammerScript = hammerObject.GetComponent<HammerCollisionEnemy>();
        directInteractor = rightHandObject.GetComponent<XRDirectInteractor>();
    }

    public void Update()
    {
        
        dashCDTimer += Time.deltaTime;

        //get the left controller velocity, the joystick, joystick clicked, and trigger
        leftJoyStick.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 leftVelocity);
        //rightJoyStick.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 rightVelocity);

        leftJoyStick.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftAxisValue);
        //rightJoyStick.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightAxisValue);

        leftJoyStick.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed);
        //headSetObject.transform.Rotate(new Vector3(0, turnSpeed * Time.deltaTime * rightAxisValue.x, 0));

        leftJoyStick.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool resetScene);
        
        //get the right hand grip to summon the hammer
        rightJoyStick.TryGetFeatureValue(CommonUsages.gripButton, out bool recallHammer);

        //I had to add edge cases to stop repeats because the hammer would a* path multiple times and 
        //glitch its self out
        if (recallHammer && !hammerScript.IsBeingHeld())
        {
            //handCollider.enabled = true;
            // directInteractor.allowSelect = true;
            hammerScript.IsSummoned();
            firstCancel = true;
        }
        //first cancel stops the hammer from using the stopsummon more than once per recall attempt
        else if (firstCancel && !recallHammer)
        {
            //directInteractor.allowSelect = false;
            // handCollider.enabled = false;
            hammerScript.StopSummon();
            firstCancel = false;
        }

        if (resetScene && debugMode)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        //the reason we set the y to 0 is so the teleport is along the 2d axis so height does not change
        //as the headset will have rotation on it and can affect the y on transform forward and right
        Vector3 rightDirection = headSetObject.transform.right;
        rightDirection.y = 0;

        Vector3 forwardDirection = headSetObject.transform.forward;
        forwardDirection.y = 0;

        if (dashCDTimer >= dashCooldown && isDashing == false)
        {
            //if the joystick input is greater than the minimum
            //then we get the current direction (honestly its a bit inaccurate)
            if (Vector3.Magnitude(leftAxisValue) > minLeftStickInput)
            {
                moveDirection = headSetObject.transform.right * leftAxisValue.x +
                                headSetObject.transform.forward * leftAxisValue.y;
                moveDirection.y = 0;
                moveDirection = moveDirection.normalized;
                isDashing = true;
                dashCDTimer = 0;
            }
            
            //I forgot the hand velocity movement was a thing
            //if the trigger is pressed and the player moves their left controller enough
            //we get the cardinal direction with a dot product (cardinal is used because controller wrist flicking
            //makes it extremely hard to get the direction correct
            if (Vector3.SqrMagnitude(leftVelocity) > 3f && triggerPressed)
            {
                float dotCheck = 0.8f;
                Vector2 leftVelDirection = new Vector2(leftVelocity.x, leftVelocity.z).normalized;

                //we turn it into a vector2 because we dont care about the y value
                //and it is slightly faster to do a xz check rather than xyz
                Vector2 forwardCheck =
                    new Vector2(headSetObject.transform.forward.x, headSetObject.transform.forward.z);
                Vector2 rightCheck = new Vector2(headSetObject.transform.right.x, headSetObject.transform.right.z);

                if (Vector2.Dot(leftVelDirection, forwardCheck) > dotCheck)
                {
                    moveDirection = forwardDirection;
                    isDashing = true;
                    dashCDTimer = 0;
                }

                //down
                if (Vector2.Dot(leftVelDirection, -forwardCheck) > dotCheck)
                {
                    moveDirection = -forwardDirection;
                    isDashing = true;
                    dashCDTimer = 0;
                }

                //left
                if (Vector2.Dot(leftVelDirection, -rightCheck) > dotCheck)
                {
                    moveDirection = -rightDirection;
                    isDashing = true;
                    dashCDTimer = 0;
                }

                //right
                if (Vector2.Dot(leftVelDirection, rightCheck) > dotCheck)
                {
                    moveDirection = rightDirection;
                    isDashing = true;
                    dashCDTimer = 0;
                }
            }
        }
        
        //if the player is dashing we run the movement and if it returns true we are done dashing
        if (isDashing)
        {
            if (DashMove())
                isDashing = false;
        }
    }

    //this will move the player in the moveDirection until it has to stop then it will return false;
    private bool DashMove()
    {
        //gives the data back 
        RaycastHit hit;
        //this is to get the waist level so the raycasts dont go over anything 
        Vector3 searchPosition = headSetObject.transform.position;
        searchPosition.y = headSetObject.transform.position.y / 2;

        //we do two raycasts, one that is on a slight angle from forward facing downwards 
        //and the second one is a larger angle so the raycast is going to the floor int the movement direciton
        if (Physics.Raycast(searchPosition, new Vector3(moveDirection.x, -0.3f, moveDirection.z), out hit,
            dashRaySearchLimit))
        {
            //if on the obstacle layer
            if (hit.transform.gameObject.layer == 11)
            {
                amountDashed = 0;
                return true;
            }
        }

        if (Physics.Raycast(searchPosition, new Vector3(moveDirection.x, -0.6f, moveDirection.z), out hit,
            dashRaySearchLimit))
        {
            if (hit.transform.gameObject.layer == 11)
            {
                amountDashed = 0;
                return true;
            }
        }

        //if the dash distance has been hit then we return true as dash is done
        Vector3 moveAmount = moveDirection * (dashSpeed * Time.deltaTime);
        amountDashed += Vector3.Magnitude(moveAmount);
        if (amountDashed >= dashDistance)
        {
            amountDashed = 0;
            moveDirection = Vector3.zero;
            return true;
        }
        //Since the player cant (or shouldn't) have a rigidbody we do the transform position move instead
        PlayerObject.transform.position += moveAmount;


        return false;
    }
}