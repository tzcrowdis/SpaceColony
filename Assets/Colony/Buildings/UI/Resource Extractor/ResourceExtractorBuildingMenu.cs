using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceExtractorBuildingMenu : BuildingInfoMenu
{
    [Header("Resource Target")]
    public TMP_Dropdown resourceDropdown;
    [Tooltip("text displayed when no resource is selected")]
    public string noResourceSelected;

    OrbitingPlanet orbitingPlanet;

    ResourceExtractorBuilding reBuilding;
    
    protected override void Start()
    {
        base.Start();

        orbitingPlanet = GameObject.FindGameObjectWithTag("Planet").GetComponent<OrbitingPlanet>();

        UpdateResourceTargetList(true); // forces starting variable to noResourceSelected
        resourceDropdown.onValueChanged.AddListener(ChangeTarget);

        reBuilding = building.gameObject.GetComponent<ResourceExtractorBuilding>();
    }

    void UpdateResourceTargetList(bool resourceDepleted)
    {
        TMP_Dropdown.OptionData selected;
        if (resourceDepleted)
            selected = new TMP_Dropdown.OptionData() { text = noResourceSelected };
        else
            selected = resourceDropdown.options[resourceDropdown.value];

        resourceDropdown.ClearOptions();
        resourceDropdown.options.Add(selected);
        resourceDropdown.RefreshShownValue();

        if (selected.text != noResourceSelected)
            resourceDropdown.options.Add(new TMP_Dropdown.OptionData() { text = noResourceSelected });

        foreach (PlanetResource resource in orbitingPlanet.planetResources)
            resourceDropdown.options.Add(new TMP_Dropdown.OptionData() { text = resource.resource.ToString() });
    }

    void ChangeTarget(int value)
    {
        PlanetResource selectedResource = null;
        if (resourceDropdown.options[value].text != noResourceSelected)
            selectedResource = orbitingPlanet.planetResources[value - 1];// -1 bc None always added first
        // TODO test this, i think something will be funky with more than one planet resources 
        // might be true for colonists as well
        
        reBuilding.ChangeSelectedResource(selectedResource); 
        //UpdateResourceTargetList(false);
    }
}
