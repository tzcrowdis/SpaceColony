using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageBuildingMenu : BuildingInfoMenu
{
    [Header("Resource Dropdown")]
    public TMP_Dropdown resourceDropdown;

    protected override void Start()
    {
        base.Start();

        resourceDropdown.onValueChanged.AddListener(ChangeStorageResource);

        // add resource options to dropdown
        resourceDropdown.ClearOptions();
        resourceDropdown.options.Add(new TMP_Dropdown.OptionData() { text = "None" });
        foreach (var r in Enum.GetValues(typeof(ColonyResources.ResourceTypes)))
        {
            resourceDropdown.options.Add(new TMP_Dropdown.OptionData() { text = r.ToString() });
        }
    }

    void ChangeStorageResource(int value)
    {
        building.GetComponent<StorageBuilding>().ChangeStoredResource(resourceDropdown.options[value].text);

        if (resourceDropdown.options[value].text == "None")
            building.buildingListItem.BuildingAlert();
        else
            building.buildingListItem.ClearBuildingAlert();

        resourceDropdown.Hide();
    }
}
