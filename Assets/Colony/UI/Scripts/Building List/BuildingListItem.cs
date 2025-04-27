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
    Vector3 focusDestination = Vector3.one;
    public float moveSpeed;
    Vector3 focusForwardVec = Vector3.one;
    public float rotateSpeed;
    public float focusStopDistance;
    float tol = 0.1f;

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

        Transform parentCanvas = GameObject.Find("Building List Canvas").transform;
        GameObject menu = Instantiate(building.buildingMenuPrefab, parentCanvas);
        // TODO menu position
    }

    void FocusColonyBuilding()
    {
        // TODO adjust speeds so rotation and movement always stop at the end (LERP?)
        
        // center camera on this building
        // find point inbetween camera and building that's a dist of x from the building
        if (focusDestination == Vector3.one)
        {
            focusDestination = (cam.transform.position - building.transform.position).normalized * focusStopDistance;
            focusDestination.y = building.transform.position.y; // locks rotation to y-axis
        }

        //cam.transform.position = Vector3.MoveTowards(cam.transform.position, focusDestination, moveSpeed * Time.deltaTime);
        //cam.transform.position = Vector3.Lerp(cam.transform.position, focusDestination, 1f);

        // rotate towards normal from point to building
        if (focusForwardVec == Vector3.one)
        {
            focusForwardVec = (building.transform.position - focusDestination).normalized;
        }

        //cam.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(cam.transform.forward, focusForwardVec, rotateSpeed * Time.deltaTime, 0f));
        //cam.transform.rotation = Vector3.Lerp(cam.transform.forward,)

        // check if focused
        if (Vector3.Distance(cam.transform.position, focusDestination) < tol)
            Debug.Log("within distance");

        if (Mathf.Approximately(Vector3.Dot(cam.transform.forward, focusForwardVec), 1f))
            Debug.Log("within rotation");

        if (Vector3.Distance(cam.transform.position, focusDestination) < tol & Mathf.Approximately(Vector3.Dot(cam.transform.forward, focusForwardVec), 1f))
        {
            cam.transform.position = focusDestination;
            cam.transform.rotation = Quaternion.LookRotation(focusForwardVec);
            
            engageFocus = false;
            focusDestination = Vector3.one;
            focusForwardVec = Vector3.one;
            Debug.Log("focused");
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
