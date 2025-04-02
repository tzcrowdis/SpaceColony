using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.UI;

public class ColonyUI : MonoBehaviour
{
    [Header("Building UI")]
    // building variables
    public GameObject buildingPanel;
    public Button buildingPanelButton;
    public Button buildingPanelExit;

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

    // etc.

    [Header("Time UI")]
    // time variables
    public TMP_Text clock;
    public Button stopTime;
    public Button oneTime;
    public Button twoTime;
    public Button threeTime;

    [Header("Pause Menu")]
    public Button pauseMenuButton;

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
        allBuildingPanels = new List<GameObject>();
        
        // TODO [PANEL ACTIVATION] rework with delegates so there's one activate panels button that takes a reference to the desired panel
        genericBuildingsButton.onClick.AddListener(ActivateGenericBuildingPanel);
        allBuildingPanels.Add(genericBuildingsPanel);

        energyBuildingsButton.onClick.AddListener(ActivateEnergyBuildingPanel);
        allBuildingPanels.Add(energyBuildingsPanel);

        extractionBuildingsButton.onClick.AddListener(ActivateExtractionBuildingPanel);
        allBuildingPanels.Add(extractionBuildingsPanel);

        foodBuildingsButton.onClick.AddListener(ActivateFoodBuildingsPanel);
        allBuildingPanels.Add(foodBuildingsPanel);

        // TODO other building panels...

        DeactivateAllOtherBuildingPanels(genericBuildingsPanel); // default active building panel

        buildingPanelButton.onClick.AddListener(OpenBuildingPanel);
        buildingPanelExit.onClick.AddListener(CloseBuildingPanel);
        buildingPanel.SetActive(false); // NOTE deactivate panels last otherwise find doesn't work

        stopTime.onClick.AddListener(StopTime);
        oneTime.onClick.AddListener(OneTime);
        twoTime.onClick.AddListener(TwoTime);
        threeTime.onClick.AddListener(ThreeTime);

        pauseMenuButton.onClick.AddListener(ColonyControls.instance.PauseGame);
    }

    void Update()
    {
        UpdateClock();
    }

    // TODO find way to wrap buttons together (all build buttons, all default buttons, etc.)

    /*
     *  BUILDING UI
     */
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

    // TODO [PANEL ACTIVATION]
    // GENERIC PANEL
    void ActivateGenericBuildingPanel()
    {
        genericBuildingsPanel.SetActive(true);
        DeactivateAllOtherBuildingPanels(genericBuildingsPanel);
    }

    // ENERGY PANEL
    void ActivateEnergyBuildingPanel()
    {
        energyBuildingsPanel.SetActive(true);
        DeactivateAllOtherBuildingPanels(energyBuildingsPanel);
    }

    // EXTRACTION PANEL
    void ActivateExtractionBuildingPanel()
    {
        extractionBuildingsPanel.SetActive(true);
        DeactivateAllOtherBuildingPanels(extractionBuildingsPanel);
    }

    // FOOD PANEL
    void ActivateFoodBuildingsPanel()
    {
        foodBuildingsPanel.SetActive(true);
        DeactivateAllOtherBuildingPanels(foodBuildingsPanel);
    }

    // ETC.

    void DeactivateAllOtherBuildingPanels(GameObject activePanel)
    {
        foreach (GameObject panel in allBuildingPanels)
        {
            if (panel == activePanel)
                continue;

            panel.SetActive(false);
        }
    }


    /*
     *  TIME UI
     */
    
    void UpdateClock() // TODO format this to the game world time
    {
        int time = (int)Time.time;
        clock.text = time.ToString();
    }

    void StopTime()
    {
        Time.timeScale = 0f;
    }

    void OneTime()
    {
        Time.timeScale = 1f;
    }

    void TwoTime()
    {
        Time.timeScale = 2f;
    }

    void ThreeTime()
    {
        Time.timeScale = 3f;
    }

    /*
     * PAUSE MENU
     */
    void OpenPauseMenu()
    {
        
    }
}
