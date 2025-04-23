using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingList : MonoBehaviour
{
    public GameObject scrollView;

    List<Building> buildings = new List<Building>();

    public static BuildingList instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        DisableScrollControlsOverScrollView();
    }

    public void AddBuildingToList(Building bldg)
    {
        buildings.Add(bldg);

        // TODO add list item to scroll view
    }

    public void DeconstructBuilding(Building bldg)
    {
        // TODO remove from list and adjust

        Destroy(bldg.gameObject);
    }

    void DisableScrollControlsOverScrollView() // NOTE locks scroll for all UI not just scrollable...
    {
        bool overScrollableUI = false;
        
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        if (raycastResults.Count > 0)
        {
            foreach (RaycastResult result in raycastResults)
                if (result.gameObject == scrollView) overScrollableUI = true;
        }

        if (overScrollableUI)
            ColonyControls.instance.altitudeLock = true;
        else
            ColonyControls.instance.altitudeLock = false;
    }
}
