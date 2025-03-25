using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class PauseMenu : MonoBehaviour
{
    [Header("Root Menu")]
    public GameObject rootMenu;
    public Button resumeBtn;
    public Button settingsMenuBtn;
    public Button saveBtn;
    public TMP_Text saveConfirmationMsg;
    string lastSaved;
    public Button quitToDesktopBtn;

    [Header("Settings Menu")]
    public GameObject settingsMenu;
    public Button backSettingsBtn;

    void Start()
    {
        // last saved
        if (lastSaved == null)
            lastSaved = "never";
        saveConfirmationMsg.text = lastSaved;

        // root menu
        resumeBtn.onClick.AddListener(Resume);
        settingsMenuBtn.onClick.AddListener(OpenSettingsMenu);
        saveBtn.onClick.AddListener(SaveGame);
        quitToDesktopBtn.onClick.AddListener(QuitToDesktop);

        // settings menu
        settingsMenu.SetActive(false);
        backSettingsBtn.onClick.AddListener(RootFromSettings);
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

    void QuitToDesktop()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    /*
     * SAVE GAME
     */
    void SaveGame()
    {
        // call all save functions
        // TODO

        // update last saved msg
        lastSaved = DateTime.Now.ToString();
        saveConfirmationMsg.text = $"Last Saved: {lastSaved} [LIE]";

        // update and store json file
        // TODO
    }

    /*
     * SETTINGS MENU
     */
    void RootFromSettings()
    {
        settingsMenu.SetActive(false);
        rootMenu.SetActive(true);
    }
}
