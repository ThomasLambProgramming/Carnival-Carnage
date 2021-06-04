using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

public class HammerSpawn : MonoBehaviour
{
    private bool alreadySpawned = false;
    public GameObject HammerPrefab = null;
    private GameObject CurrentHammer = null;

    public InputDeviceCharacteristics controllerCharacteristics;
    private InputDevice controller;

    public void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        if (devices.Count > 0)
        {
            controller = devices[0];
        }

        SpawnHammer();
    }

    public void SpawnHammer()
    {
        CurrentHammer = Instantiate(HammerPrefab, transform);
        CurrentHammer.transform.localRotation.Set(0, 90, 0, 0);
        CurrentHammer.GetComponent<Rigidbody>().isKinematic = true;

        alreadySpawned = true;
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.GetComponentInChildren<XRController>().added)
        {
            controller.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton);
            controller.TryGetFeatureValue(CommonUsages.gripButton, out bool gripButton);

            if (triggerButton && gripButton)
            {
                CurrentHammer.GetComponent<Rigidbody>().isKinematic = false;
                SpawnHammer();
            }
        }
    }
}
