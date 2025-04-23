using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildUI : MonoBehaviour
{
    [Header("Build UI")]
    public GameObject buildingPanel;

    List<GameObject> allBuildingPanels;

    [Header("Generic Building UI")]
    public Button genericBuildingsButton;
    public GameObject genericBuildingsPanel;

    [Header("Energy Building UI")]
    public Button energyBuildingsButton;
    public GameObject energyBuildingsPanel;

    [Header("Extraction Building UI")]
    public Button extractionBuildingsButton;
    public GameObject extractionBuildingsPanel;

    [Header("Food Building UI")]
    public Button foodBuildingsButton;
    public GameObject foodBuildingsPanel;

    [Header("Research Building UI")]
    public Button researchBuildingsButton;
    public GameObject researchBuildingsPanel;

    // etc.


    void Awake()
    {
        allBuildingPanels = new List<GameObject>();

        genericBuildingsButton.onClick.AddListener(delegate { ActivateBuildingPanel(genericBuildingsPanel); });
        allBuildingPanels.Add(genericBuildingsPanel);

        energyBuildingsButton.onClick.AddListener(delegate { ActivateBuildingPanel(energyBuildingsPanel); });
        allBuildingPanels.Add(energyBuildingsPanel);

        extractionBuildingsButton.onClick.AddListener(delegate { ActivateBuildingPanel(extractionBuildingsPanel); });
        allBuildingPanels.Add(extractionBuildingsPanel);

        foodBuildingsButton.onClick.AddListener(delegate { ActivateBuildingPanel(foodBuildingsPanel); });
        allBuildingPanels.Add(foodBuildingsPanel);

        researchBuildingsButton.onClick.AddListener(delegate { ActivateBuildingPanel(researchBuildingsPanel); });
        allBuildingPanels.Add(researchBuildingsPanel);

        // etc.

        // NOTE deactivate panels last otherwise find doesn't work
        DeactivateAllOtherBuildingPanels(genericBuildingsPanel); // default active building panel
        buildingPanel.SetActive(false);
    }

    private void OnEnable()
    {
        buildingPanel.SetActive(true);
    }

    /*
     * PANEL TOGGLING
     */
    void ActivateBuildingPanel(GameObject panel)
    {
        panel.SetActive(true);
        DeactivateAllOtherBuildingPanels(panel);
    }

    void DeactivateAllOtherBuildingPanels(GameObject activePanel)
    {
        foreach (GameObject panel in allBuildingPanels)
        {
            if (panel == activePanel)
                continue;

            panel.SetActive(false);
        }
    }
}
