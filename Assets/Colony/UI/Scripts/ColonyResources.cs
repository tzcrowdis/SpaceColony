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
        Metals,
        Organics,
        Electricity,
        Research,
        Food,
        None // NOTE will probably break a lot of things but is here for resource extraction
    }

    public Dictionary<ResourceTypes, float> colonyResources;
    [Header("Resources")]
    public float metals;
    public float organics;
    public float electricity;
    public float research;
    public float food;
    // etc.

    public Dictionary<ResourceTypes, float> colonyResourcesCaps;
    [Header("Resource Caps")]
    public float metalsMax;
    public float organicsMax;
    public float electricityMax;
    public float researchMax;
    public float foodMax;

    [Header("Text where Resource is Displayed")]
    public TMP_Text metalsText;
    public TMP_Text organicsText;
    public TMP_Text electricityText;
    public TMP_Text researchText;
    public TMP_Text foodText;
    public int displayBuildingCost = 0;

    [Header("Unemployed Colonists")]
    public List<Colonist> unemployedColonists = new List<Colonist>();

    // Buildings by Job Type
    List<Building> genericBuildings = new List<Building>();
    List<Building> energyBuildings = new List<Building>();
    List<Building> extractionBuildings = new List<Building>();
    List<Building> farmBuildings = new List<Building>();
    List<Building> researchBuildings = new List<Building>();
    List<Building> storageBuildings = new List<Building>();
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
            { ResourceTypes.Metals, metals },
            { ResourceTypes.Organics, organics },
            { ResourceTypes.Electricity, electricity },
            { ResourceTypes.Research, research },
            { ResourceTypes.Food, food }
        };

        colonyResourcesCaps = new Dictionary<ResourceTypes, float>
        {
            { ResourceTypes.Metals, metalsMax },
            { ResourceTypes.Organics, organicsMax },
            { ResourceTypes.Electricity, electricityMax },
            { ResourceTypes.Research, researchMax },
            { ResourceTypes.Food, foodMax }
        };

        Building[] allBuildings = FindObjectsOfType<Building>();
        foreach (Building building in allBuildings)
            AddBuildingToTypeList(building);
    }

    void Update()
    {
        UpdateResourceFields(displayBuildingCost);
    }

    public void UpdateResourceFields(int cost) // NOTE cost is the cost of buildings that aren't built yet
    {
        // TODO allow for multiple cost types other than just metals

        if (metalsText != null)
        {
            metalsText.text = $"Metals: {(int)colonyResources[ResourceTypes.Metals] - cost} / {(int)colonyResourcesCaps[ResourceTypes.Metals]}";

            if (cost != 0) metalsText.color = Color.red;
            else metalsText.color = Color.white;
        }

        if (organicsText != null)
        {
            organicsText.text = $"Organics: {(int)colonyResources[ResourceTypes.Organics]} / {(int)colonyResourcesCaps[ResourceTypes.Organics]}";
        }

        if (electricityText != null)
        {
            electricityText.text = $"Electricity: {(int)colonyResources[ResourceTypes.Electricity]} / {(int)colonyResourcesCaps[ResourceTypes.Electricity]}";
        }

        if (researchText != null)
        {
            researchText.text = $"Research: {(int)colonyResources[ResourceTypes.Research]} / Inf";
        }

        if (foodText != null)
        {
            foodText.text = $"Food: {(int)colonyResources[ResourceTypes.Food]} / {(int)colonyResourcesCaps[ResourceTypes.Food]}";
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
            case Building.BuildingType.Storage:
                storageBuildings.Add(building);
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
            case Building.BuildingType.Storage:
                return storageBuildings;
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
