using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingListItem : MonoBehaviour
{
    public Building building;

    [Header("Buttons")]
    public Button buildingButton;
    public Button deleteButton;

    void Start()
    {
        buildingButton.onClick.AddListener(ListItemClicked);
        deleteButton.onClick.AddListener(DeconstructBuilding);
    }

    void ListItemClicked()
    {
        ZoomToColonyBuilding();

        Transform parentCanvas = GameObject.Find("Building List Canvas").transform;
        GameObject menu = Instantiate(building.buildingMenuPrefab, parentCanvas);
        // TODO menu position
    }

    void ZoomToColonyBuilding()
    {
        // TODO center camera on this building
    }

    public void DeconstructBuilding()
    {
        // TODO make sure destroying this building isn't stranding others
        // check via navmesh path calcs..?
        
        Destroy(building.gameObject);
        Destroy(gameObject);
    }
}
