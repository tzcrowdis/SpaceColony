using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    public GameObject buildingPrefab;

    public bool unlocked;

    Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SelectedBuilding);
    }

    void Update()
    {
        CheckBuildingAffordable();
    }

    // HACK gotta be a better way to perform these checks...
    void CheckBuildingAffordable()
    {
        Building building = buildingPrefab.GetComponent<Building>();
        
        if (building.genericCost > ColonyResources.instance.colonyResources[ColonyResources.ResourceTypes.Generic])
        {
            button.interactable = false;
            return;
        }
        
        if (building.energyCost > ColonyResources.instance.colonyResources[ColonyResources.ResourceTypes.Energy])
        {
            button.interactable = false;
            return;
        }

        button.interactable = true;
    }

    void SelectedBuilding()
    {
        // early exit for errors
        if (buildingPrefab == null)
        {
            Debug.Log("building prefab not found. have you added the reference in the editor?");
            return;
        }

        // on click destroy current selection
        Destroy(ColonyControls.instance.selectedBuilding);

        // instantiate building in blueprint mode
        ColonyControls.instance.state = ColonyControls.State.BuildingSelected;
        ColonyControls.instance.selectedBuilding = Instantiate(buildingPrefab);
        ColonyControls.instance.selectedBuilding.GetComponent<Building>().state = Building.State.Blueprint;

        // disable building colliders
        foreach (Collider c  in ColonyControls.instance.selectedBuilding.GetComponents<Collider>())
            c.enabled = false;
        foreach (Transform connections in ColonyControls.instance.selectedBuilding.transform)
            connections.gameObject.SetActive(false);

        // disable building panel
        ColonyUI.instance.CloseBuildingPanelWhileBuilding();
    }
}
