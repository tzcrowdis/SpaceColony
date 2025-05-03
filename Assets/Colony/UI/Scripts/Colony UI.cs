using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.UI;

public class ColonyUI : MonoBehaviour
{
    [Header("Time UI")]
    public TMP_Text clock;
    public Button stopTime;
    public Button oneTime;
    public Button twoTime;
    public Button threeTime;

    [Header("Pause Menu")]
    public Button pauseMenuButton;

    [Header("Build UI")]
    public GameObject build;
    public Button buildButton;
    public Button buildExit;

    [Header("Building List UI")]
    public GameObject buildingList;
    public Button buildingListButton;
    public Button buildingListExit;

    [Header("Colonist List UI")]
    public GameObject colonistList;
    public Button colonistListButton;
    public Button colonistListExit;

    List<Button> colonyButtons;

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
        colonyButtons = new List<Button>(){
            buildButton,
            buildingListButton
        };

        stopTime.onClick.AddListener(StopTime);
        oneTime.onClick.AddListener(OneTime);
        twoTime.onClick.AddListener(TwoTime);
        threeTime.onClick.AddListener(ThreeTime);

        pauseMenuButton.onClick.AddListener(ColonyControls.instance.PauseGame);

        buildButton.onClick.AddListener(OpenBuildCanvas);
        buildExit.onClick.AddListener(CloseBuildCanvas);
        build.SetActive(false);

        buildingListButton.onClick.AddListener(delegate { OpenList(buildingList); });
        buildingListExit.onClick.AddListener(delegate { CloseList(buildingList); });
        CloseList(buildingList);

        colonistListButton.onClick.AddListener(delegate { OpenList(colonistList); });
        colonistListExit.onClick.AddListener(delegate { CloseList(colonistList); });
        CloseList(colonistList);
    }

    void Update()
    {
        UpdateClock();
    }

    /*
     * GENERAL UI
     */
    void ActivateColonyButtons()
    {
        foreach (Button button in colonyButtons)
            button.gameObject.SetActive(true);
    }

    void DeactivateColonyButtons()
    {
        foreach (Button button in colonyButtons)
            button.gameObject.SetActive(false);
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
     *  BUILD UI
     */
    void OpenBuildCanvas()
    {
        build.SetActive(true);
        DeactivateColonyButtons();
    }

    void CloseBuildCanvas()
    {
        build.SetActive(false);
        ActivateColonyButtons();
        ColonyControls.instance.CancelBuildingSelection();
    }

    public void CloseBuildCanvasWhileBuilding()
    {
        build.SetActive(false);
        ActivateColonyButtons();
    }

    /*
     *  LISTS
     */
    void OpenList(GameObject list)
    {
        list.GetComponent<Canvas>().enabled = true;
        list.GetComponent<GraphicRaycaster>().enabled = true;
        list.transform.GetChild(0).gameObject.SetActive(true);
        DeactivateColonyButtons();
    }

    void CloseList(GameObject list)
    {
        list.GetComponent<Canvas>().enabled = false;
        list.GetComponent<GraphicRaycaster>().enabled = false;
        list.transform.GetChild(0).gameObject.SetActive(false);
        ActivateColonyButtons();
    }
}
