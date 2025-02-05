using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    GameObject selectedBuilding;
    [Header("Construction Variables")]
    [Tooltip("How far a selected building appears in the world from the camera.")]
    public float selectedBuildingDepth;
    [HideInInspector]
    public Transform connectionLocation;
    Vector3 connectionOffset;

    public GameObject buildingPrefab; // TESTING

    // states to track selections, menus, etc.
    enum State
    {
        Default,
        BuildingSelected,
        InMenu // TODO implement different menus/overlays
    }
    State state;

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

        //state = State.Default;

        // testing building selected
        state = State.BuildingSelected;
        selectedBuilding = Instantiate(buildingPrefab);
        selectedBuilding.GetComponent<BoxCollider>().enabled = false;
        foreach (Transform connections in selectedBuilding.transform)
        {
            connections.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // TODO conditions for controller type and anything else...
        if (state != State.InMenu)
        {
            TranslateCamera();
            RotateCamera();
            AltitudeCamera();

            if (state == State.BuildingSelected)
            {
                // TODO clicking the UI element should set the state and the selectedBuilding gameobject
                BuildingLocation();
                if (rotateBuilding.triggered) RotateBuilding();
                if (placeBuilding.triggered) PlaceBuilding();
                if (cancelBuildingSelection.triggered) CancelBuildingSelection();
            }
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
                Debug.Log(Vector3.Dot(connectionLocation.forward, connectionSelectedBuilding.forward));
                if (Vector3.Dot(connectionLocation.forward, connectionSelectedBuilding.forward) == -1f)
                {
                    connectionOffset = -connectionSelectedBuilding.localPosition;
                    Debug.Log(connectionSelectedBuilding.localPosition);
                    break;
                }
            }
            //Debug.Log(connectionLocation.position);
            //Debug.Log(offset);
            selectedBuilding.transform.position = connectionLocation.position + connectionOffset;
        }
        else
        {
            // follow mouse at predetermined depth
            selectedBuilding.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, selectedBuildingDepth));
        }
    }

    void RotateBuilding()
    {
        //rotates 90 degrees around y-axis
        selectedBuilding.transform.Rotate(new Vector3(0f, 90f, 0f));
    }

    void PlaceBuilding()
    {
        // TODO
        // if selected building is in acceptable position
        // then set selected building to null here
        // and run start building function in 

        selectedBuilding = null;
        state = State.Default;
    }

    void CancelBuildingSelection()
    {
        // remove the building and return to default
        Destroy(selectedBuilding);
        state = State.Default;
    }
}
