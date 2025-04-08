using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class ActiveBuildingPanel : MonoBehaviour
{
    [HideInInspector]
    public Building building; // set by on click function of building (same as the screen position)

    [Header("Text Fields")]
    public TMP_Text buildingName;

    [Header("Building Stats")]
    public TMP_Text efficiency;
    public TMP_Text generation;
    public TMP_Text consumption;

    [Header("Work Stations")]
    [Tooltip("order of dropdowns must be same as order of work stations in building hierarchy")]
    public TMP_Dropdown[] workStationDropdowns;
    int[] dropdownSelections;

    [Header("Exit Button")]
    public Button exit;

    [Header("Text Displayed When No Worker Selected")]
    public string noWorkerSelected;

    protected virtual void Start()
    {
        // building name
        buildingName.text = building.name;
        buildingName.text = buildingName.text.Remove(buildingName.text.Length - 7); // removes "(Clone)" from name
        // TODO write a function that adds spaces before capital letters

        //stats
        if (efficiency != null || generation != null || consumption != null)
        {
            efficiency.text = $"Efficiency: {building.BuildingEfficiency() * 100}%";
            generation.text = $"+ {building.productionQuantity} {building.productionResource}";
            consumption.text = $"- {building.consumptionQuantity} {building.consumptionResource}";
        }

        // colonists / work stations
        dropdownSelections = new int[workStationDropdowns.Length];
        for (int i = 0; i < workStationDropdowns.Length; i++)
            dropdownSelections[i] = workStationDropdowns[i].value;

        UpdateDropdownUnemployedLists();

        foreach (TMP_Dropdown dropdown in workStationDropdowns)
            dropdown.onValueChanged.AddListener(delegate { WorkerChanged(dropdown); });

        // exit button
        exit.onClick.AddListener(ClosePanel);
    }

    protected virtual void Update()
    {
        if (building.BuildingEfficiency() > 0)
        {
            // update stats
            efficiency.text = $"Efficiency: {building.BuildingEfficiency() * 100}%";
            generation.text = $"+ {building.productionQuantity} {building.productionResource}";
            consumption.text = $"- {building.consumptionQuantity} {building.consumptionResource}";
        }
    }

    protected virtual void UpdateDropdownUnemployedLists()
    {
        foreach (TMP_Dropdown dropdown in workStationDropdowns)
        {
            TMP_Dropdown.OptionData selected = dropdown.options[dropdown.value];
            dropdown.ClearOptions();
            dropdown.options.Add(selected);
            dropdown.RefreshShownValue();

            if (selected.text != noWorkerSelected)
                dropdown.options.Add(new TMP_Dropdown.OptionData() { text = noWorkerSelected });

            foreach (Colonist colonist in ColonyResources.instance.unemployedColonists)
                dropdown.options.Add(new TMP_Dropdown.OptionData() { text = colonist.characterName });
        }
    }

    protected virtual void WorkerChanged(TMP_Dropdown dropdown)
    {
        if (dropdown.options[dropdown.value].text == noWorkerSelected) // remove colonist from work station
        {
            // find index of previous selection then update current dropdown selections
            int previousValue = 0;
            for (int i = 0; i < workStationDropdowns.Length; i++)
            {
                if (workStationDropdowns[i] == dropdown)
                {
                    previousValue = dropdownSelections[i];
                    dropdownSelections[i] = dropdown.value;
                    break;
                }
            }

            // unemploy the colonist
            Colonist unemployedColonist = GameObject.Find(dropdown.options[previousValue].text).GetComponent<Colonist>();
            unemployedColonist.workStation = null;
            building.colonists.Remove(unemployedColonist);
            ColonyResources.instance.unemployedColonists.Add(unemployedColonist);
        }
        else // add colonist to work station
        {
            Colonist employedColonist = ColonyResources.instance.unemployedColonists[dropdown.value - 1]; // -1 bc None option

            // NOTE assumes order of dropdowns is same as order of work stations in building object
            for (int i = 0; i < workStationDropdowns.Length; i++)
            {
                if (workStationDropdowns[i] == dropdown)
                {
                    employedColonist.workStation = building.workStations[i];
                    building.colonists.Add(employedColonist);
                    ColonyResources.instance.unemployedColonists.Remove(employedColonist);
                    break;
                }
            }
        }
        
        UpdateDropdownUnemployedLists();
    }

    protected virtual void ClosePanel()
    {
        building.panelOpen = false;
        
        gameObject.SetActive(false);
    }
}
