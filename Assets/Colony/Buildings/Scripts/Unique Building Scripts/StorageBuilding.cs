using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBuilding : Building
{
    [Header("Stored Resource")]
    public ColonyResources.ResourceTypes storageType;
    public float storageAdder;

    protected override void Start()
    {
        base.Start();
    }

    public void ChangeStoredResource(ColonyResources.ResourceTypes resource)
    {
        storageType = resource;

        // change cap of colony resources
        if (storageType.ToString() == "Generic")
            ColonyResources.instance.genericMax += storageAdder;
        else if (storageType.ToString() == "Food")
            ColonyResources.instance.genericMax += storageAdder;
        else if (storageType.ToString() == "Energy")
            ColonyResources.instance.genericMax += storageAdder;
        else if (storageType.ToString() == "Mineral")
            ColonyResources.instance.genericMax += storageAdder;
        else
            Debug.Log("resource type not found");
    }
}
