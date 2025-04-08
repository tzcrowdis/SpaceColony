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
        if (ColonyResources.instance.colonyResourcesCaps[ColonyResources.ResourceTypes.Generic] > 1000)
            ColonyResources.instance.colonyResourcesCaps[ColonyResources.ResourceTypes.Generic] -= storageAdder;
        else if (ColonyResources.instance.colonyResourcesCaps[ColonyResources.ResourceTypes.Food] > 1000)
            ColonyResources.instance.colonyResourcesCaps[ColonyResources.ResourceTypes.Food] -= storageAdder;
        else if (ColonyResources.instance.colonyResourcesCaps[ColonyResources.ResourceTypes.Energy] > 1000)
            ColonyResources.instance.colonyResourcesCaps[ColonyResources.ResourceTypes.Energy] -= storageAdder;
        else if (ColonyResources.instance.colonyResourcesCaps[ColonyResources.ResourceTypes.Mineral] > 1000)
            ColonyResources.instance.colonyResourcesCaps[ColonyResources.ResourceTypes.Mineral] -= storageAdder;
        else
            Debug.Log("no resource greater than starting value");

        // early exit 
        if (resource == "None")
            return;

        // increase cap of chosen colony resource
        storageType = (ColonyResources.ResourceTypes)System.Enum.Parse(typeof(ColonyResources.ResourceTypes), resource);
        ColonyResources.instance.colonyResourcesCaps[storageType] += storageAdder;
    }
}
