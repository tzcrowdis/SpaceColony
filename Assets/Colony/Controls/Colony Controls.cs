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
    [HideInInspector]
    public bool altitudeLock = false;

    PlayerInput playerInput;
    
    // camera inputs
    InputAction rotateCameraAction;
    InputAction translateCameraAction;
    InputAction altitudeCameraAction;

    // time inputs
    InputAction stopTime;
    InputAction oneTime;
    InputAction twoTime;
    InputAction threeTime;

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
    public bool overValidConnectionPoint;

    [Header("Pause Menu")]
    public GameObject pauseMenu;
    InputAction escape;

    // states to track selections, menus, etc.
    public enum State
    {
        Default,
        BuildingSelected,
        InMenu
    }
    [Header("Control State")]
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

        stopTime = playerInput.actions["StopTime"];
        oneTime = playerInput.actions["1xTime"];
        twoTime = playerInput.actions["2xTime"];
        threeTime = playerInput.actions["3xTime"];

        buildingTracking = playerInput.actions["BuildingTracking"];
        rotateBuilding = playerInput.actions["RotateBuilding"];
        placeBuilding = playerInput.actions["PlaceBuilding"];
        cancelBuildingSelection = playerInput.actions["CancelBuildingSelection"];
        connectionLocation = null;
        overValidConnectionPoint = false;

        escape = playerInput.actions["Escape"];
        pauseMenu.SetActive(false);

        state = State.Default;
    }

    void Update()
    {
        // TODO conditions for controller type and anything else...

        switch (state)
        {
            case State.Default:
                CameraControls();
                TimeControls();
                if (escape.triggered) PauseGame();
                break;

            case State.BuildingSelected:
                CameraControls();
                TimeControls();

                BuildingControls();
                break;

            case State.InMenu:
                if (escape.triggered) UnpauseGame();
                break;
        }
    }

    /*
     *  CAMERA CONTROLS
     */
    void CameraControls()
    {
        TranslateCamera();
        RotateCamera();
        if (!altitudeLock) AltitudeCamera();

        altitudeLock = false; // NOTE refreshes the lock each frame
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
        transform.RotateAround(transform.forward * 0.1f, Vector3.up, rotation);
    }

    void AltitudeCamera()
    {
        // move camera up/down
        float y = transform.position.y;
        y += altitudeCameraAction.ReadValue<float>() / 120f * altitudeSpeed;
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(y, -10f, 10f), transform.position.z); // clamp altitude
    }

    /*
     *  TIME CONTROLS
     */
    void TimeControls()
    {
        PauseUnpauseTime();
        OneTime();
        TwoTime();
        ThreeTime();
    }

    void PauseUnpauseTime()
    {     
        
        if (stopTime.triggered)
        {
            if (Mathf.Approximately(Time.timeScale, 0f))
                Time.timeScale = 1f;
            else
                Time.timeScale = 0f;
        }  
    }

    void OneTime()
    {
        if (oneTime.triggered)
            Time.timeScale = 1f;
    }

    void TwoTime()
    {
        if (twoTime.triggered)
            Time.timeScale = 2f;
    }

    void ThreeTime()
    {
        if (threeTime.triggered)
            Time.timeScale = 3f;
    }

    /*
     *  BUILDING CONTROLS
     */
    void BuildingControls()
    {
        DisableBuildingInfoMenus();

        BuildingLocation();
        if (rotateBuilding.triggered) RotateBuilding();
        if (placeBuilding.triggered) PlaceBuilding();
        if (cancelBuildingSelection.triggered | escape.triggered) CancelBuildingSelection();
    }

    void BuildingLocation()
    {
        // find connection point
        Vector2 screenPos = buildingTracking.ReadValue<Vector2>();

        // follow mouse at predetermined depth
        selectedBuilding.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, selectedBuildingDepth));

        // overwrite location if mouse over connection position
        if (connectionLocation != null)
        {
            // determine the offset
            foreach (Transform connectionSelectedBuilding in selectedBuilding.GetComponent<Building>().connectionPoints.transform)
            {
                // skip if not on building connection layer
                if (connectionSelectedBuilding.gameObject.layer != 6)
                    continue;

                // set offset based on if connection points are facing eachother
                float dot = Vector3.Dot(connectionLocation.forward, connectionSelectedBuilding.forward);
                if (Mathf.Approximately(dot, -1f))
                {
                    Vector3 offset = connectionLocation.transform.position - connectionSelectedBuilding.transform.position;
                    selectedBuilding.transform.position += offset;
                    overValidConnectionPoint = true;
;                   break;
                }
            }
        }

        // building only visible when not over UI
        if (EventSystem.current.IsPointerOverGameObject())
            selectedBuilding.GetComponent<Building>().building.GetComponent<Renderer>().enabled = false;
        else
            selectedBuilding.GetComponent<Building>().building.GetComponent<Renderer>().enabled = true;
    }

    void RotateBuilding()
    {
        //rotates 90 degrees around y-axis
        selectedBuilding.transform.Rotate(new Vector3(0f, 90f, 0f));

        overValidConnectionPoint = false;
        BuildingLocation(); // update building location after rotate
    }

    void PlaceBuilding()
    {
        // building only placeable if not over UI and over connection point
        if (!EventSystem.current.IsPointerOverGameObject() && overValidConnectionPoint)
        {
            selectedBuilding.GetComponent<Building>().PlaceBuilding();
            selectedBuilding = null;
            state = State.Default;

            EnableBuildingInfoMenus();
        }
    }

    public void CancelBuildingSelection()
    {
        // remove the building and return to default
        Destroy(selectedBuilding);
        state = State.Default;

        EnableBuildingInfoMenus();
    }

    // TODO expand toggle info menus to colonists and other objects as well
    void EnableBuildingInfoMenus()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        foreach (GameObject building in buildings)
        {
            building.GetComponent<Building>().clickCollider.enabled = true;
        }
    }

    void DisableBuildingInfoMenus()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        foreach (GameObject building in buildings)
        {
            building.GetComponent<Building>().clickCollider.enabled = false;
        }
    }

    /*
     * PAUSE/UNPAUSE
     */
    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        state = State.InMenu;
    }

    public void UnpauseGame()
    {
        if (!pauseMenu.GetComponent<PauseMenu>().rootMenu.activeSelf)
        {
            // goes back to root from any sub menu
            // NOTE assumes menus have no child menus (should be ok?)
            foreach (Transform childMenu in pauseMenu.transform)
            {
                if (childMenu.gameObject.activeSelf)
                {
                    childMenu.gameObject.SetActive(false);
                    pauseMenu.GetComponent<PauseMenu>().rootMenu.SetActive(true);
                    break;
                }
            }
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
            state = State.Default;
        }    
    }
}
