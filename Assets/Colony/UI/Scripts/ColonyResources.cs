using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

public class ColonyResources : MonoBehaviour
{
    public enum ResourceTypes
    {
        Generic,
        Food,
        Energy,
        Mineral,
        Research,
        None // NOTE will probably break a lot of things but is here for resource extraction
    }

    public Dictionary<ResourceTypes, float> colonyResources;
    [Header("Resources")]
    public float generic;
    public float food;
    public float energy;
    public float mineral;
    public float research;
    // etc.

    public Dictionary<ResourceTypes, float> colonyResourcesCaps;
    [Header("Resource Caps")]
    public float genericMax;
    public float foodMax;
    public float energyMax;
    public float mineralMax;
    public float researchMax;

    [Header("Text where Resource is Displayed")]
    public TMP_Text genericText;
    public TMP_Text foodText;
    public TMP_Text energyText;
    public TMP_Text mineralText;
    public TMP_Text researchText;

    [Header("Unemployed Colonists")]
    public List<Colonist> unemployedColonists = new List<Colonist>();

    // Buildings by Job Type
    List<Building> genericBuildings = new List<Building>();
    List<Building> energyBuildings = new List<Building>();
    List<Building> extractionBuildings = new List<Building>();
    List<Building> farmBuildings = new List<Building>();
    List<Building> researchBuildings = new List<Building>();
    List<Building> cafeteriaBuildings = new List<Building>();
    List<Building> medicalBuildings = new List<Building>();
    List<Building> recreationBuildings = new List<Building>();
    List<Building> sleepBuildings = new List<Building>();

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
            { ResourceTypes.Mineral, mineral },
            { ResourceTypes.Research, research }
        };

        colonyResourcesCaps = new Dictionary<ResourceTypes, float>
        {
            { ResourceTypes.Generic, genericMax },
            { ResourceTypes.Food, foodMax },
            { ResourceTypes.Energy, energyMax },
            { ResourceTypes.Mineral, mineralMax },
            { ResourceTypes.Research, researchMax }
        };

        Building[] allBuildings = FindObjectsOfType<Building>();
        foreach (Building building in allBuildings)
            AddBuildingToTypeList(building);
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

        if (researchText != null)
        {
            researchText.text = $"Research: {(int)colonyResources[ResourceTypes.Research]} / Inf";
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

    /*
     * BUILDING FUNCTIONS
     */
    public void AddBuildingToTypeList(Building building)
    {
        switch (building.buildingType)
        {
            case Building.BuildingType.Generic:
                genericBuildings.Add(building);
                break;
            case Building.BuildingType.Energy:
                energyBuildings.Add(building);
                break;
            case Building.BuildingType.Extraction:
                extractionBuildings.Add(building);
                break;
            case Building.BuildingType.Farm:
                farmBuildings.Add(building);
                break;
            case Building.BuildingType.Research:
                researchBuildings.Add(building);
                break;
            case Building.BuildingType.Cafeteria:
                cafeteriaBuildings.Add(building);
                break;
            case Building.BuildingType.Medical:
                medicalBuildings.Add(building);
                break;
            case Building.BuildingType.Recreation:
                recreationBuildings.Add(building);
                break;
            case Building.BuildingType.Sleep:
                sleepBuildings.Add(building);
                break;
        }
    }

    public List<Building> GetBuildingListByType(Building.BuildingType type)
    {
        switch (type)
        {
            case Building.BuildingType.Generic:
                return genericBuildings;
            case Building.BuildingType.Energy:
                return energyBuildings;
            case Building.BuildingType.Extraction:
                return extractionBuildings;
            case Building.BuildingType.Farm:
                return farmBuildings;
            case Building.BuildingType.Research:
                return researchBuildings;
            case Building.BuildingType.Cafeteria:
                return cafeteriaBuildings;
            case Building.BuildingType.Medical:
                return medicalBuildings;
            case Building.BuildingType.Recreation:
                return recreationBuildings;
            case Building.BuildingType.Sleep:
                return sleepBuildings;
        }

        return null;
    }
}
