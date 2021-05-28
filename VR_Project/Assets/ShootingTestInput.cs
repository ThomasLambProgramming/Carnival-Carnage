using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class ShootingTestInput : MonoBehaviour
{
    public float ShootForce = 200f;
    private ActionBasedController controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<ActionBasedController>();
        controller.selectAction.action.ReadValue<bool>();

        controller.selectAction.action.performed += ShootTest;
    }

    private void ShootTest(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        int i = 0;
        while (i < 100)
        {
            GameObject test = GameObject.CreatePrimitive(PrimitiveType.Cube);
            test.transform.position = transform.position;
            test.AddComponent<Rigidbody>();
            Rigidbody rb = test.GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * ShootForce * 4);
            test.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            i++;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
