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
        colonyResources = new Dictionary<ResourceTypes, float>();
        colonyResources.Add(ResourceTypes.Generic, generic);
        colonyResources.Add(ResourceTypes.Food, food);
        colonyResources.Add(ResourceTypes.Energy, energy);
        colonyResources.Add(ResourceTypes.Mineral, mineral);
    }

    void Update()
    {
        UpdateResourceFields();
    }

    void UpdateResourceFields()
    {
        if (genericText != null)
        {
            genericText.text = $"Generic: {(int)colonyResources[ResourceTypes.Generic]} / {(int)genericMax}";
        }

        if (foodText != null)
        {
            foodText.text = $"Food: {(int)colonyResources[ResourceTypes.Food]} / {(int)foodMax}";
        }

        if (energyText != null)
        {
            energyText.text = $"Energy: {(int)colonyResources[ResourceTypes.Energy]} / {(int)energyMax}";
        }

        if (mineralText != null)
        {
            mineralText.text = $"Minerals: {(int)colonyResources[ResourceTypes.Mineral]} / {(int)mineralMax}";
        }
    }

    /*
     * RESOURCE FUNCTIONS
     */
    public bool ProduceResource(ResourceTypes type, float quantity)
    {
        // determine max to use
        float max = 1000;
        if (type.ToString() == "Generic")
            max = genericMax;
        else if (type.ToString() == "Food")
            max = foodMax;
        else if (type.ToString() == "Energy")
            max = energyMax;
        else if (type.ToString() == "Mineral")
            max = mineralMax;
        else
            Debug.Log("couldn't determine max when adding to resource (default is 1000)");

        // resource is full then early exit
        if (colonyResources[type] == max)
            return false;

        // add to resource
        if (colonyResources[type] + quantity > max)
            colonyResources[type] = max;
        else
            colonyResources[type] += quantity;

        // production successful
        return true;
    }

    public bool ConsumeResource(ResourceTypes type, float quantity)
    {
        // resource is empty then early exit
        if (colonyResources[type] == 0)
            return false;

        // subtract from resource
        float min = 0;
        if (colonyResources[type] - quantity < min)
            colonyResources[type] = min;
        else
            colonyResources[type] -= quantity;

        // consumption successful
        return true;
    }

    public bool FullOrEmptyResource(ResourceTypes type)
    {
        // TODO
        return false;
    }
}
