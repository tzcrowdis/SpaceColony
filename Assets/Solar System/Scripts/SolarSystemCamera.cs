using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class SolarSystemCamera : MonoBehaviour
{
    Transform cam;

    public float zoomSpeed;
    public float rotationSpeed;

    PlayerInput playerInput;
    InputAction zoomAction;
    InputAction rotateAction;

    void Start()
    {
        cam = transform.GetChild(0);

        playerInput = GetComponent<PlayerInput>();
        zoomAction = playerInput.actions["Zoom"];
        rotateAction = playerInput.actions["Rotate"];
    }

    
    void Update()
    {
        // TODO add conditions to handle different devices, clicks, etc.
        
        Zoom();
        Rotate();
    }

    void Zoom()
    {
        // zoom in/out
        Vector3 camPos = cam.localPosition;
        float inputCompensation = 120; // HACK input reads as 120/-120 rather than 1/-1
        camPos.z += zoomAction.ReadValue<float>() / inputCompensation * zoomSpeed;
        cam.localPosition = new Vector3(0, 0, Mathf.Clamp(camPos.z, -20f, -5f)); // clamp zoom
    }

    void Rotate()
    {
        // rotate up/down left/right
        Vector2 rotation = rotateAction.ReadValue<Vector2>();
        transform.eulerAngles += new Vector3(rotation.y, -rotation.x, 0) * rotationSpeed;

        // clamp up/down rotation
        // source for condition: https://discussions.unity.com/t/clamping-between-everything-but-the-min-max-values/142014
        float x = transform.eulerAngles.x;
        transform.eulerAngles = new Vector3(Mathf.Clamp((x <= 180) ? x : -(360 - x), -30f, 30f), transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
