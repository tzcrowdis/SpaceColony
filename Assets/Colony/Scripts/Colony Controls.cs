using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ColonyControls : MonoBehaviour
{
    [Header("Camera Motion Variables")]
    public float rotateSpeed;
    public float translateSpeed;
    public float altitudeSpeed;

    PlayerInput playerInput;
    
    // camera inputs
    InputAction rotateCameraAction;
    InputAction translateCameraAction;
    InputAction altitudeCameraAction;

    // building inputs
    InputAction buildingTracking;
    InputAction rotateBuilding;
    InputAction placeBuilding;
    InputAction cancelBuildingSelection;
    public GameObject selectedBuilding;
    [Header("Construction Variables")]
    [Tooltip("How far a selected building appears in the world from the camera.")]
    public float selectedBuildingDepth;
    [HideInInspector]
    public Transform connectionLocation;
    Vector3 connectionOffset;
    public bool placeable;

    // states to track selections, menus, etc.
    public enum State
    {
        Default,
        BuildingSelected,
        InMenu // TODO implement different menus/overlays
    }
    public State state;

    public static ColonyControls instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        rotateCameraAction = playerInput.actions["RotateCamera"];
        translateCameraAction = playerInput.actions["TranslateCamera"];
        altitudeCameraAction = playerInput.actions["AltitudeCamera"];

        buildingTracking = playerInput.actions["BuildingTracking"];
        rotateBuilding = playerInput.actions["RotateBuilding"];
        placeBuilding = playerInput.actions["PlaceBuilding"];
        cancelBuildingSelection = playerInput.actions["CancelBuildingSelection"];
        connectionLocation = null;
        placeable = false;

        state = State.Default;

        // testing building selected
        //state = State.BuildingSelected;
    }

    void Update()
    {
        // TODO conditions for controller type and anything else...

        switch (state)
        {
            case State.Default:
                // camera controls
                TranslateCamera();
                RotateCamera();
                AltitudeCamera();
                break;

            case State.BuildingSelected:
                // camera controls
                TranslateCamera();
                RotateCamera();
                AltitudeCamera();

                // building controls
                BuildingLocation();
                if (rotateBuilding.triggered) RotateBuilding();
                if (placeBuilding.triggered) PlaceBuilding();
                if (cancelBuildingSelection.triggered) CancelBuildingSelection();
                break;

            case State.InMenu:
                break;
        }
    }

    /*
     *  CAMERA CONTROLS
     */
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

    /*
     *  BUILDING CONTROLS
     */
    void BuildingLocation()
    {
        // find connection point
        Vector2 screenPos = buildingTracking.ReadValue<Vector2>();

        // mouse over connection position
        if (connectionLocation != null)
        {
            // determine the offset
            foreach (Transform connectionSelectedBuilding in selectedBuilding.transform)
            {
                // skip if not on building connection layer
                if (connectionSelectedBuilding.gameObject.layer != 6)
                    continue;

                // set offset based on if connection points are facing eachother
                float dot = Vector3.Dot(connectionLocation.forward, connectionSelectedBuilding.forward);
                if (Mathf.Approximately(dot, -1f))
                {
                    connectionOffset = -connectionSelectedBuilding.localPosition;
;                   break;
                }
            }
            
            selectedBuilding.transform.position = connectionLocation.position + connectionOffset;
        }
        else
        {
            // follow mouse at predetermined depth
            selectedBuilding.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, selectedBuildingDepth));
        }

        // building only visible when not over UI
        if (EventSystem.current.IsPointerOverGameObject())
            selectedBuilding.GetComponent<Renderer>().enabled = false;
        else
            selectedBuilding.GetComponent<Renderer>().enabled = true;
    }

    void RotateBuilding()
    {
        //rotates 90 degrees around y-axis
        selectedBuilding.transform.Rotate(new Vector3(0f, 90f, 0f));
    }

    void PlaceBuilding()
    {
        // building only placeable if not over UI and told placeable by connection point
        if (!EventSystem.current.IsPointerOverGameObject() && placeable)
        {
            selectedBuilding.GetComponent<Building>().PlaceBuilding();
            selectedBuilding = null;
            state = State.Default;
        }
    }

    public void CancelBuildingSelection()
    {
        // remove the building and return to default
        Destroy(selectedBuilding);
        state = State.Default;
    }
}
