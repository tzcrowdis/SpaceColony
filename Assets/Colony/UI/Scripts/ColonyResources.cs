using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ColonyResources : MonoBehaviour
{
    public enum ResourceTypes
    {
        Generic,
        Food,
        Energy,
        Mineral
    }

    public Dictionary<ResourceTypes, float> colonyResources;
    [Header("Resources")]
    public float generic;
    public float food;
    public float energy;
    public float mineral;
    // etc.

    public Dictionary<ResourceTypes, float> colonyResourcesCaps;
    [Header("Resource Caps")]
    public float genericMax;
    public float foodMax;
    public float energyMax;
    public float mineralMax;

    [Header("Text where Resource is Displayed")]
    public TMP_Text genericText;
    public TMP_Text foodText;
    public TMP_Text energyText;
    public TMP_Text mineralText;

    [Header("Unemployed Colonists")]
    public List<Colonist> unemployedColonists = new List<Colonist>();

    public static ColonyResources instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    void Start()
    {
        colonyResources = new Dictionary<ResourceTypes, float>
        {
            { ResourceTypes.Generic, generic },
            { ResourceTypes.Food, food },
            { ResourceTypes.Energy, energy },
            { ResourceTypes.Mineral, mineral }
        };

        colonyResourcesCaps = new Dictionary<ResourceTypes, float>
        {
            { ResourceTypes.Generic, genericMax },
            { ResourceTypes.Food, foodMax },
            { ResourceTypes.Energy, energyMax },
            { ResourceTypes.Mineral, mineralMax }
        };
    }

    void Update()
    {
        UpdateResourceFields();
    }

    void UpdateResourceFields()
    {
        if (genericText != null)
        {
            genericText.text = $"Generic: {(int)colonyResources[ResourceTypes.Generic]} / {(int)colonyResourcesCaps[ResourceTypes.Generic]}";
        }

        if (foodText != null)
        {
            foodText.text = $"Food: {(int)colonyResources[ResourceTypes.Food]} / {(int)colonyResourcesCaps[ResourceTypes.Food]}";
        }

        if (energyText != null)
        {
            energyText.text = $"Energy: {(int)colonyResources[ResourceTypes.Energy]} / {(int)colonyResourcesCaps[ResourceTypes.Energy]}";
        }

        if (mineralText != null)
        {
            mineralText.text = $"Minerals: {(int)colonyResources[ResourceTypes.Mineral]} / {(int)colonyResourcesCaps[ResourceTypes.Mineral]}";
        }
    }

    /*
     * RESOURCE FUNCTIONS
     */
    public bool ProduceResource(ResourceTypes type, float quantity)
    {
        // determine max to use
        float max = colonyResourcesCaps[type];

        // resource is full then early exit
        if (colonyResources[type] + quantity > max)
        {
            colonyResources[type] = max; 
            return false;
        }

        // production successful
        colonyResources[type] += quantity;
        return true;
    }

    public bool ConsumeResource(ResourceTypes type, float quantity)
    {
        float min = 0;
        
        // resource cannot be consumed then early exit
        if (colonyResources[type] - quantity < min)
            return false;

        // consumption successful
        colonyResources[type] -= quantity;
        return true;
    }

    public bool FullOrEmptyResource(ResourceTypes type)
    {
        // TODO check for floating point errors
        if (colonyResources[type] == colonyResourcesCaps[type] || colonyResources[type] == 0)
            return true;
        else
            return false;
    }
}
