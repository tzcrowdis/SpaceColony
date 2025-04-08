using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageBuildingPanel : ActiveBuildingPanel
{
    [Header("Resource Dropdown")]
    public TMP_Dropdown resourceDropdown;

    protected override void Start()
    {
        base.Start();

        resourceDropdown.onValueChanged.AddListener(ChangeStorageResource);

        // add resource options to dropdown
        foreach (var r in Enum.GetValues(typeof(ColonyResources.ResourceTypes)))
        {
            resourceDropdown.options.Add(new TMP_Dropdown.OptionData() { text = r.ToString() });
        }
    }

    void ChangeStorageResource(int value)
    {
        ColonyResources.ResourceTypes resource = ColonyResources.ResourceTypes.Generic;

        // determine resource type
        foreach (var r in Enum.GetValues(typeof(ColonyResources.ResourceTypes)))
        {
            if (r.ToString().Equals(resourceDropdown.options[value]))
            {
                resource = (ColonyResources.ResourceTypes)r;
                break;
            }
        }

        building.GetComponent<StorageBuilding>().ChangeStoredResource(resource);
    }
}
