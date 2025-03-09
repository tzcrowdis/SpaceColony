using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    public TMP_Dropdown[] workStations;

    public Button exit;

    void Start()
    {
        buildingName.text = building.name;

        //stats
        efficiency.text = $"Efficiency: {building.efficiency}%";
        generation.text = $"+ {building.productionQuantity} {building.productionResource}";
        consumption.text = $"- {building.consumptionQuantity} {building.consumptionResource}";

        // colonists / work stations
        workStations = new TMP_Dropdown[building.workStations.Length];
        // TODO align dropdowns based on number of work stations
        // TODO set each dropdown option list to the unemployed colonists list (option title should be colonists lore name)

        exit.onClick.AddListener(ClosePanel);
    }

    void Update()
    {
        
    }

    void ClosePanel()
    {
        Destroy(this);
    }
}
