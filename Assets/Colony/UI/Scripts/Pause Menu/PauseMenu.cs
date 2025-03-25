using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Root Menu")]
    public GameObject rootMenu;
    public Button resumeBtn;
    public Button settingsMenuBtn;
    public Button saveMenuBtn;
    public Button loadMenuBtn;
    public Button quitToDesktopBtn;

    [Header("Settings Menu")]
    public GameObject settingsMenu;
    public Button backSettingsBtn;

    [Header("Save Menu")]
    public GameObject saveMenu;
    public Button backSaveBtn;

    [Header("Load Menu")]
    public GameObject loadMenu;
    public Button backLoadBtn;

    void Start()
    {
        // root menu
        resumeBtn.onClick.AddListener(Resume);
        settingsMenuBtn.onClick.AddListener(OpenSettingsMenu);
        saveMenuBtn.onClick.AddListener(OpenSaveMenu);
        loadMenuBtn.onClick.AddListener(OpenLoadMenu);
        quitToDesktopBtn.onClick.AddListener(QuitToDesktop);

        // settings menu
        settingsMenu.SetActive(false);
        backSettingsBtn.onClick.AddListener(RootFromSettings);

        // save menu
        saveMenu.SetActive(false);
        backSaveBtn.onClick.AddListener(RootFromSave);

        // load menu
        loadMenu.SetActive(false);
        backLoadBtn.onClick.AddListener(RootFromLoad);
    }


    /*
     * ROOT MENU
     */
    void Resume()
    {
        gameObject.SetActive(false);
        ColonyControls.instance.UnpauseGame();
    }

    void OpenSettingsMenu()
    {
        rootMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }

    void OpenSaveMenu()
    {
        rootMenu.SetActive(false);
        saveMenu.SetActive(true);
    }

    void OpenLoadMenu()
    {
        rootMenu.SetActive(false);
        loadMenu.SetActive(true);
    }

    void QuitToDesktop()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /*
     * SETTINGS MENU
     */
    void RootFromSettings()
    {
        settingsMenu.SetActive(false);
        rootMenu.SetActive(true);
    }

    /*
     * SAVE MENU
     */
    void RootFromSave()
    {
        saveMenu.SetActive(false);
        rootMenu.SetActive(true);
    }

    /*
     * LOAD MENU
     */
    void RootFromLoad()
    {
        loadMenu.SetActive(false);
        rootMenu.SetActive(true);
    }
}
