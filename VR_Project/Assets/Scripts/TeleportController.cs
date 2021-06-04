using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using System.Collections.Generic;

public class TeleportController : MonoBehaviour
{
    //Object that has the teleporting controller script
    public GameObject PlayerObject = null;
    //reference to the input action reference that contains the button mapping data for activation
    public float dashDistance = 0.3f;
    public float dashCooldown = 0.25f;

    public float turnSpeed = 10f;

    private InputDevice rightJoyStick;
    private InputDevice leftJoyStick;
    public GameObject headSetObject = null;

    private void Start()
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

    private void Update()
    {
        leftJoyStick.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftAxisValue);
        rightJoyStick.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightAxisValue);


        headSetObject.transform.Rotate(new Vector3(0, turnSpeed * Time.deltaTime * rightAxisValue.x, 0));


        float dotCheck = 0.8f;

        //the reason we set the y to 0 is so the teleport is along the 2d axis so height does not change
        //as the headset will have rotation on it and can affect the y on transform forward and right
        Vector3 rightDirection = headSetObject.transform.right;
        rightDirection.y = 0;

        Vector3 forwardDirection = headSetObject.transform.forward;
        forwardDirection.y = 0;

        //up
        if (Vector2.Dot(leftAxisValue, new Vector2(0, 1)) > dotCheck)
        {
            PlayerObject.transform.position += forwardDirection * dashDistance * Time.deltaTime;
        }
        //down
        if (Vector2.Dot(leftAxisValue, new Vector2(0, -1)) > dotCheck)
        {
            PlayerObject.transform.position -= forwardDirection * dashDistance * Time.deltaTime;
        }
        //left
        if (Vector2.Dot(leftAxisValue, new Vector2(-1, 0)) > dotCheck)
        {
            PlayerObject.transform.position -= rightDirection * dashDistance * Time.deltaTime;
        }
        //right
        if (Vector2.Dot(leftAxisValue, new Vector2(1, 0)) > dotCheck)
        {
            PlayerObject.transform.position += rightDirection * dashDistance * Time.deltaTime;
        }
    }

}
