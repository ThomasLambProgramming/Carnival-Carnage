//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class SimpleCamera : MonoBehaviour
//{
//    [SerializeField]
//    [Range(1, 1000)] float mouseSensitivity = 200;
    
//    private float xRotation = 0f;
//    private float yRotation = 0f;

//    void Update()
//    {
//        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
//        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

//        xRotation -= mouseY;
//        yRotation -= mouseX;
//        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
//        yRotation = Mathf.Clamp(xRotation, -90f, 90f);

//        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

//    }
//}
