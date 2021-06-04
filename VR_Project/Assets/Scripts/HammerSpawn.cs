using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.UI;
using System.Linq;

public class HammerSpawn : MonoBehaviour
{
    public GameObject HammerPrefab = null;
    private bool spawnHammer = false;

    private GameObject CurrentHammer = null;
    private InputDevice device;

    public void Start()
    {
        XRNode xrNodeRight = XRNode.RightHand;
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(xrNodeRight, devices);
        device = devices.FirstOrDefault();
    }

    public void SpawnHammer()
    {
        CurrentHammer = Instantiate(HammerPrefab, transform);
        CurrentHammer.transform.localRotation.Set(0, -90, 0, 0);
    }

    public void Update()
    {
        if (spawnHammer && CurrentHammer == null)
        {
            device.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButton);
            device.TryGetFeatureValue(CommonUsages.gripButton, out bool gripButton);

            if (triggerButton && gripButton)
            {
                SpawnHammer();
            }
        }
        
        if (CurrentHammer != null)
        {
            if (!CurrentHammer.activeInHierarchy)
            {
                CurrentHammer = null;
            }
        }
    }

    public void OnTriggerStay(Collider collider)
    {
        if (collider.name == "LeftHand Collider" || collider.name == "RightHand Collider")
        {
            spawnHammer = true;
        }
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.name == "LeftHand Collider" || collider.name == "RightHand Collider")
        {
            spawnHammer = false;
        }
    }
}
