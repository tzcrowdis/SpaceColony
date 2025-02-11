using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.UI;

public class ColonyUI : MonoBehaviour
{
    GameObject buildingPanel;
    Button buildingPanelButton;
    Button buildingPanelExit;

    public TMP_Text clock;
    public Button stopTime;
    public Button oneTime;
    public Button twoTime;
    public Button threeTime;

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

        stopTime.onClick.AddListener(StopTime);
        oneTime.onClick.AddListener(OneTime);
        twoTime.onClick.AddListener(TwoTime);
        threeTime.onClick.AddListener(ThreeTime);
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
}
