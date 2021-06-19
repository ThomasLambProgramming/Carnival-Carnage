/*
* File: HandPresence.cs
*
* Author: Mara Dusevic (s200494@students.aie.edu.au)
* Date Created: Friday 28 May 2021
* Date Last Modified: Thursday 17 June 2021
*
* Used to change the hand model for the player
* to either the given hand model or their vr headset's
* controller.
*
*/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    #region Properties

    public bool showController = false;
    public InputDeviceCharacteristics controllerCharacteristics;
    public List<GameObject> controllerPrefabs;
    public GameObject handModelPrefab;
    private InputDevice targetDevice;
    private GameObject spawnedController;
    private GameObject spawnedHandModel;
    private Animator handAnimator;

    #endregion

    // Start Function
    private void Start()
    {
        TryInitialise();
    }

    // Update Function
    private void Update()
    {
        // If platform is not android, do not continue.
        if (Application.platform != RuntimePlatform.Android)
        {
            return;
        }

        // If target device is still invalid after initialising, try again
        if (!targetDevice.isValid)
        {
            TryInitialise();

        }
        // Otherwise spawn models
        else
        {
            // If controller wants to be shown
            if (showController)
            {
                spawnedHandModel.SetActive(false);
                spawnedController.SetActive(true);
            }
            // If hand wants to be shown
            else
            {
                spawnedHandModel.SetActive(true);
                spawnedController.SetActive(false);

                // Activates hand animations
                UpdateHandAnimation();
            }
        }

        // Uses the player input on their controller, to show or not show the controller.
        targetDevice.TryGetFeatureValue(CommonUsages.primaryTouch, out bool primaryTouchValue);
        if (primaryTouchValue)
        {
            showController = true;
        }
        else
        {
            showController = false;
        }
    }

    // Attempts to find the type of device the player is using
    private void TryInitialise()
    {
        // Gets all devices with the given controller characteristics
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        // If there are devices
        if (devices.Count > 0)
        {
            // Get the first device in the list and find the controller from its name in the controller prefabs
            targetDevice = devices[0];
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);
            
            // If the correct model can be found
            if (prefab)
            {
                // Spawn the model at the controller's transform
                spawnedController = Instantiate(prefab, transform);
            }
            else
            {
                // Otherwise show warning that the corresponding controller could not be found.
                Debug.Log("Did not find corresponding controller model");

                // Spawn the default model instead
                spawnedController = Instantiate(controllerPrefabs[0], transform);
            }
        }
        
        // Spawns the hand model and finds its animation
        spawnedHandModel = Instantiate(handModelPrefab, transform);
        handAnimator = spawnedHandModel.GetComponent<Animator>();
    }

    // Updates the hands animation depending on the player's input
    private void UpdateHandAnimation()
    {
        // Sets the correct variables in the animator to determine which animation to play
        // Gets input from the trigger
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        // Gets input from the grip
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }
}