using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using System.Collections.Generic;

public class TeleportController : MonoBehaviour
{
    //Object that has the teleporting controller script
    public GameObject PlayerObject = null;
    //reference to the input action reference that contains the button mapping data for activation
    public float dashDistance = 1f;
    public float dashSpeed = 2f;

    //this is the minimum amount the player has to move the stick before a dash
    public float minLeftStickInput = 0.2f;

    //amount of time till a dash can be done again
    public float dashCooldown = 1f;
    private float dashCDTimer = 1;

    public float dashRaySearchLimit = 2f;
    //this is to have a set dash movement over time so it doesnt change until the next input
    private Vector3 moveDirection;
    bool isDashing = false;

    //timer thingy for the dash
    private float amountDashed = 0f;

    //public float turnSpeed = 40f;

    private InputDevice rightJoyStick;
    private InputDevice leftJoyStick;
    public GameObject headSetObject = null;

    public void Start()
    {
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
    }

    public void Update()
    {
        dashCDTimer += Time.deltaTime;

        leftJoyStick.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 leftVelocity);
        //rightJoyStick.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 rightVelocity);

        leftJoyStick.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftAxisValue);
        //rightJoyStick.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightAxisValue);

        leftJoyStick.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerPressed);
        //headSetObject.transform.Rotate(new Vector3(0, turnSpeed * Time.deltaTime * rightAxisValue.x, 0));

        //the reason we set the y to 0 is so the teleport is along the 2d axis so height does not change
        //as the headset will have rotation on it and can affect the y on transform forward and right
        Vector3 rightDirection = headSetObject.transform.right;
        rightDirection.y = 0;

        Vector3 forwardDirection = headSetObject.transform.forward;
        forwardDirection.y = 0;

        if (dashCDTimer >= dashCooldown && isDashing == false)
        {
            if (Vector3.Magnitude(leftAxisValue) > minLeftStickInput)
            {
                moveDirection = headSetObject.transform.right * leftAxisValue.x + headSetObject.transform.forward * leftAxisValue.y;
                moveDirection.y = 0;
                moveDirection = moveDirection.normalized;
                isDashing = true;
                dashCDTimer = 0;
            }
            if (Vector3.SqrMagnitude(leftVelocity) > 3f && triggerPressed)
            {
                float dotCheck = 0.8f;
                Vector2 leftVelDirection = new Vector2(leftVelocity.x, leftVelocity.z).normalized;
                

                Vector2 forwardCheck = new Vector2(headSetObject.transform.forward.x, headSetObject.transform.forward.z);
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

        if (Physics.Raycast(searchPosition, new Vector3(moveDirection.x, -0.3f, moveDirection.z), out hit, dashRaySearchLimit))
        {
            if (hit.transform.gameObject.layer == 11)
            {
                amountDashed = 0;
                return true;
            }
        }
        if (Physics.Raycast(searchPosition, new Vector3(moveDirection.x, -0.6f, moveDirection.z), out hit, dashRaySearchLimit))
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
        PlayerObject.transform.position += moveAmount;


        return false;
    }

}
