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

    public Button exit;

    void Start()
    {
        
    }

    void OnEnable()
    {
        // building name
        buildingName.text = building.name;

        //stats
        efficiency.text = $"Efficiency: {building.efficiency}%";
        generation.text = $"+ {building.productionQuantity} {building.productionResource}";
        consumption.text = $"- {building.consumptionQuantity} {building.consumptionResource}";

        // colonists / work stations
        UpdateDropdownUnemployedLists();
        foreach (TMP_Dropdown dropdown in workStationDropdowns)
            dropdown.onValueChanged.AddListener(delegate { WorkerChanged(dropdown); });

        // exit button
        exit.onClick.AddListener(ClosePanel);
    }

    void Update()
    {
        
    }

    void UpdateDropdownUnemployedLists()
    {
        foreach (TMP_Dropdown dropdown in workStationDropdowns)
        {
            if (dropdown.options[dropdown.value].text == "None")
            {
                dropdown.ClearOptions();

                dropdown.options.Add(new TMP_Dropdown.OptionData() { text = "None" });
                dropdown.transform.GetChild(0).GetComponent<TMP_Text>().text = "None";

                foreach (Colonist colonist in ColonyResources.instance.unemployedColonists)
                    dropdown.options.Add(new TMP_Dropdown.OptionData() { text = colonist.characterName });
            }
            else
            {
                // TODO case where not none but need to remove other employed colonist
            }
        }
    }

    void WorkerChanged(TMP_Dropdown dropdown)
    {
        // TODO case of removing colonist from work station
        
        Colonist employedColonist = ColonyResources.instance.unemployedColonists[dropdown.value - 1]; // -1 bc None option

        // NOTE assumes order of dropdowns is same as order of work stations in building object
        for (int i = 0; i < workStationDropdowns.Length; i++)
        {
            if (workStationDropdowns[i] == dropdown)
            {
                employedColonist.workStation = building.workStations[i];
                ColonyResources.instance.unemployedColonists.Remove(employedColonist);
                break;
            }  
        }
        
        UpdateDropdownUnemployedLists();
    }

    void ClosePanel()
    {
        building.panelOpen = false;
        
        Destroy(gameObject);
    }
}
