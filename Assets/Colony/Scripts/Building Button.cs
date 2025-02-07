using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    public GameObject buildingPrefab;
    public ColonyControls controls;

    Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SelectedBuilding);
    }

    void SelectedBuilding()
    {
        // early exit for errors
        if (buildingPrefab == null)
        {
            Debug.Log("building prefab not found. have you added the reference in the editor?");
            return;
        }
        else if (controls == null)
        {
            Debug.Log("controls not found. have you added the reference in the editor?");
            return;
        }

        // instantiate building and disable colliders while in blueprint mode
        controls.state = ColonyControls.State.BuildingSelected;
        controls.selectedBuilding = Instantiate(buildingPrefab);
        controls.selectedBuilding.GetComponent<BoxCollider>().enabled = false;
        foreach (Transform connections in controls.selectedBuilding.transform)
            connections.gameObject.SetActive(false);
    }
}
