using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;

public class VRUIButtonBrain : MonoBehaviour
{
    #region MATERIALS

    // materials
    [SerializeField]
    private Material DefaultMaterial;   // default material for the button when it is not at all interacted with 

    [SerializeField]
    private Material HighlightMaterial; // material for when the button is pointed at, representing which button is going to be selected

    [SerializeField]
    private Material PressMaterial;     // material for when the button is pressed 

    #endregion

    #region INPUT

    // input
    private InputDevice LeftController;
    private InputDevice RightController;

    #endregion

    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.Left;
        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, devices);
        if (devices.Count > 0)
        {
            LeftController = devices[0];
        }
        devices.Clear();
        devices = new List<InputDevice>();
        InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right;
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);
        if (devices.Count > 0)
        {
            RightController = devices[0];
        }
    }

    void Update()
    {
        // leftJoyStick.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out bool resetScene);

        //LeftController.TryGetFeatureUsages(CommonUsages.)
        //if (Physics.Raycast()
    }
}
