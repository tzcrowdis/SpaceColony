using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColonyUI : MonoBehaviour
{
    GameObject buildingPanel;
    Button buildingPanelButton;
    Button buildingPanelExit;

    public static ColonyUI instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    void Start()
    {
        buildingPanelButton = GameObject.Find("BuildingPanelButton").GetComponent<Button>();
        buildingPanelButton.onClick.AddListener(OpenBuildingPanel);

        buildingPanelExit = GameObject.Find("ExitBuildingPanelButton").GetComponent<Button>();
        buildingPanelExit.onClick.AddListener(CloseBuildingPanel);

        buildingPanel = GameObject.Find("BuildingPanel");
        buildingPanel.SetActive(false); // NOTE deactivate panels last otherwise find doesn't work
    }

    // TODO find way to wrap buttons together (all build buttons, all default buttons, etc.)

    void OpenBuildingPanel()
    {
        buildingPanel.SetActive(true);
        buildingPanelButton.gameObject.SetActive(false);
    }

    void CloseBuildingPanel()
    {
        buildingPanel.SetActive(false);
        buildingPanelButton.gameObject.SetActive(true);
        ColonyControls.instance.CancelBuildingSelection();
    }

    public void CloseBuildingPanelWhileBuilding()
    {
        buildingPanel.SetActive(false);
        buildingPanelButton.gameObject.SetActive(true);
    }
}
