using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class ColonyControls : MonoBehaviour
{
    public float rotateSpeed;
    public float translateSpeed;
    public float altitudeSpeed;

    PlayerInput playerInput;
    InputAction rotateCameraAction;
    InputAction translateCameraAction;
    InputAction altitudeCameraAction;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rotateCameraAction = playerInput.actions["RotateCamera"];
        translateCameraAction = playerInput.actions["TranslateCamera"];
        altitudeCameraAction = playerInput.actions["AltitudeCamera"];
    }

    void Update()
    {
        // TODO conditions for controller type, active states, etc.
        
        TranslateCamera();
        RotateCamera();
        AltitudeCamera();
    }

    void TranslateCamera()
    {
        // translate forward/backward left/right
        Vector2 translation = translateCameraAction.ReadValue<Vector2>() * translateSpeed / 120f;
        transform.Translate(translation.x, 0, translation.y);
    }

    void RotateCamera()
    {
        // rotate camera left/right
        float rotation = rotateCameraAction.ReadValue<float>() * rotateSpeed;
        transform.RotateAround(Vector3.forward * 0.1f, Vector3.up, rotation);
    }

    void AltitudeCamera()
    {
        // move camera up/down
        float y = transform.position.y;
        y += altitudeCameraAction.ReadValue<float>() / 120f * altitudeSpeed;
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(y, -10f, 10f), transform.position.z); // clamp altitude
    }
}
