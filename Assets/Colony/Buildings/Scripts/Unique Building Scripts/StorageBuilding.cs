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

    public void ChangeStoredResource(string resource)
    {
        // reset caps of other resources
        // TODO take into account other resource buildings in place
        if (ColonyResources.instance.genericMax > 1000)
            ColonyResources.instance.genericMax -= storageAdder;
        else if (ColonyResources.instance.foodMax > 1000)
            ColonyResources.instance.foodMax -= storageAdder;
        else if (ColonyResources.instance.energyMax > 1000)
            ColonyResources.instance.energyMax -= storageAdder;
        else if (ColonyResources.instance.mineralMax > 1000)
            ColonyResources.instance.mineralMax -= storageAdder;
        else
            Debug.Log("no resource greater than starting value");

        // early exit 
        if (resource == "None")
            return;

        // increase cap of chosen colony resource
        storageType = (ColonyResources.ResourceTypes)System.Enum.Parse(typeof(ColonyResources.ResourceTypes), resource);
        if (storageType.ToString() == "Generic")
            ColonyResources.instance.genericMax += storageAdder;
        else if (storageType.ToString() == "Food")
            ColonyResources.instance.foodMax += storageAdder;
        else if (storageType.ToString() == "Energy")
            ColonyResources.instance.energyMax += storageAdder;
        else if (storageType.ToString() == "Mineral")
            ColonyResources.instance.mineralMax += storageAdder;
        else
            Debug.Log("resource type not found");
    }
}
