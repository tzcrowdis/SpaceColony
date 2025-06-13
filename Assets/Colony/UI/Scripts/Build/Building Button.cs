using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject buildingPrefab;

    public bool unlocked;

    Button button;

    Transform buildingParent;

    public TMPro.TMP_Text costText;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SelectedBuilding);

        buildingParent = GameObject.Find("BUILDING_MANAGER").transform;

        costText.text = $"-{buildingPrefab.GetComponent<Building>().genericCost} Generic";
    }

    void Update()
    {
        CheckBuildingAffordable();
    }

    // TODO gotta be a better way to perform these checks...
    void CheckBuildingAffordable()
    {
        Building building = buildingPrefab.GetComponent<Building>();
        
        if (building.genericCost > ColonyResources.instance.colonyResources[ColonyResources.ResourceTypes.Generic])
        {
            button.interactable = false;
            return;
        }

        // etc.

        button.interactable = true;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        ColonyResources.instance.displayBuildingCost = buildingPrefab.GetComponent<Building>().genericCost;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        ColonyResources.instance.displayBuildingCost = 0;
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
        ColonyControls.instance.selectedBuilding = Instantiate(buildingPrefab, buildingParent);
        ColonyControls.instance.selectedBuilding.GetComponent<Building>().state = Building.State.Blueprint;

        // disable building connections
        foreach (Transform connection in ColonyControls.instance.selectedBuilding.GetComponent<Building>().connectionPoints.transform)
            connection.gameObject.SetActive(false);

        // disable building panel
        ColonyUI.instance.CloseBuildCanvasWhileBuilding();
    }
}
