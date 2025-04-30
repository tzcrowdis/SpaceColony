using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class BuildingListItem : MonoBehaviour
{
    public Building building;

    [Header("Buttons")]
    public Button buildingButton;
    public Button deleteButton;

    [Header("Focus on Building")]
    Camera cam;
    bool engageFocus;

    Vector3 camPositionStart = Vector3.one;
    Vector3 focusDestination = Vector3.one;
    public float focusStopDistance;
    float tol = 0.1f;

    Vector3 camForwardStart = Vector3.one;
    Vector3 focusForwardVec = Vector3.one;

    public float focusSpeed;
    float focusTime = 0f;


    void Start()
    {
        buildingButton.onClick.AddListener(ListItemClicked);
        deleteButton.onClick.AddListener(DeconstructBuilding);

        cam = Camera.main;
    }

    void Update()
    {
        if (engageFocus)
            FocusColonyBuilding();
    }

    void ListItemClicked()
    {
        engageFocus = true;

        Transform parentCanvas = GameObject.Find("Colony Canvas").transform;
        GameObject menu = Instantiate(building.buildingMenuPrefab, parentCanvas);
        menu.GetComponent<BuildingInfoMenu>().building = building;
        // TODO menu position
    }

    void FocusColonyBuilding()
    {
        // find point inbetween camera and building that's a dist of x from the building
        if (focusDestination == Vector3.one)
        {
            focusDestination = (cam.transform.position - building.transform.position).normalized * focusStopDistance + building.transform.position;
            focusDestination.y = building.transform.position.y; // locks rotation to y-axis

            Debug.Log(Vector3.Distance(building.transform.position, focusDestination));

            camPositionStart = cam.transform.position;
        }
        cam.transform.position = Vector3.Lerp(camPositionStart, focusDestination, focusTime * focusSpeed);

        // rotate towards normal from cam og point to building
        if (focusForwardVec == Vector3.one)
        {
            focusForwardVec = (building.transform.position - focusDestination).normalized;

            camForwardStart = cam.transform.forward;
        }
        cam.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(camForwardStart, focusForwardVec, focusTime * focusSpeed));

        // increment focous/lerp time
        focusTime += Time.deltaTime;

        // check if focused
        if (Vector3.Distance(cam.transform.position, focusDestination) < tol & Mathf.Approximately(Vector3.Dot(cam.transform.forward, focusForwardVec), 1f))
        {
            cam.transform.position = focusDestination;
            cam.transform.rotation = Quaternion.LookRotation(focusForwardVec);
            
            engageFocus = false;
            focusDestination = Vector3.one;
            focusForwardVec = Vector3.one;
            focusTime = 0f;
        }
    }

    public void DeconstructBuilding()
    {
        // TODO make sure destroying this building isn't stranding others
        // check via navmesh path calcs..?
        
        Destroy(building.gameObject);
        Destroy(gameObject);
    }
}
