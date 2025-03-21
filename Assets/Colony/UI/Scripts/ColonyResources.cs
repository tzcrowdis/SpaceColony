using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
            genericText.text = $"Generic: {(int)colonyResources[ResourceTypes.Generic]}";
        }

        if (foodText != null)
        {
            foodText.text = $"Food: {(int)colonyResources[ResourceTypes.Food]}";
        }

        if (energyText != null)
        {
            energyText.text = $"Energy: {(int)colonyResources[ResourceTypes.Energy]}";
        }

        if (mineralText != null)
        {
            mineralText.text = $"Minerals: {(int)colonyResources[ResourceTypes.Mineral]}";
        }
    }
}
