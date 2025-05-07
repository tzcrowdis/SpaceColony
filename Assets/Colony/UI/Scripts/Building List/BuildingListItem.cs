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
    public float focusStopDistance;
    public float focusSpeed;

    // cam focus vars
    Camera cam;
    bool engageFocus;
    Vector3 camPositionStart = Vector3.one;
    Vector3 focusDestination = Vector3.one;
    float tol = 0.1f;
    Vector3 camForwardStart = Vector3.one;
    Vector3 focusForwardVec = Vector3.one;
    float focusTime = 0f;


    void Start()
    {
        buildingButton.onClick.AddListener(ListItemClicked);
        deleteButton.onClick.AddListener(DeconstructBuilding);

        cam = Camera.main;

        if (building.requiresPlayerAttention)
            BuildingAlert();
    }

    void Update()
    {
        if (engageFocus)
            FocusColonyBuilding();
    }

    void ListItemClicked()
    {
        engageFocus = true;

        Transform parentCanvas = GameObject.Find("Building List Canvas").transform;

        // makes sure building menu isn't already open
        BuildingInfoMenu[] menus = parentCanvas.GetComponentsInChildren<BuildingInfoMenu>();
        foreach (BuildingInfoMenu bldgMenu in menus)
            if (bldgMenu.building == building) return;

        // otherwise open menu
        GameObject menu = Instantiate(building.buildingMenuPrefab, parentCanvas);
        menu.GetComponent<BuildingInfoMenu>().building = building;
        if (menus.Length > 0) 
            menu.transform.position = menus[menus.Length - 1].transform.position + new Vector3(25f, -25f, 0);
    }

    void FocusColonyBuilding()
    {
        // find point inbetween camera and building that's a dist of x from the building
        if (focusDestination == Vector3.one)
        {
            focusDestination = (cam.transform.position - building.transform.position).normalized * focusStopDistance + building.transform.position;
            focusDestination.y = building.transform.position.y; // locks rotation to y-axis

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

    public void BuildingAlert()
    {
        // TODO some other means of alerting player

        ColorBlock cb = buildingButton.colors;
        cb.normalColor = Color.red;
        buildingButton.colors = cb;
    }

    public void ClearBuildingAlert()
    {
        // NOTE assumes only thing was making the button red

        buildingButton.colors = ColorBlock.defaultColorBlock;
    }

    public void DeconstructBuilding()
    {
        // TODO make sure destroying this building isn't stranding others
        // check via navmesh path calcs..?
        
        Destroy(building.gameObject);
        Destroy(gameObject);
    }
}
