using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ColonistInfoMenu : MonoBehaviour
{
    [HideInInspector]
    public Colonist colonist; // NOTE set by colonist list item

    [Header("Menu Items")]
    public TMP_Text colonistName;
    public Button exitButton;
    public TMP_Dropdown occupationDropdown;
    public TMP_Text mentalState;
    public TMP_Text suggestLabel;
    public Button suggestButton;
    public GameObject suggestMenu;
    public Button skillprofButton;
    public GameObject skillprofMenu;

    void Start()
    {
        colonistName.text = colonist.characterName;
        exitButton.onClick.AddListener(CloseColonistMenu);

        occupationDropdown.ClearOptions();
        foreach (var r in Enum.GetValues(typeof(Colonist.JobTypes)))
            occupationDropdown.options.Add(new TMP_Dropdown.OptionData() { text = r.ToString() });
        occupationDropdown.onValueChanged.AddListener(colonist.ChangeColonistsJob);

        mentalState.text = $"Mental State: {colonist.mentalState.ToString()}";

        suggestLabel.text = $"Suggestion: {colonist.suggestion.ToString()}";
        suggestButton.onClick.AddListener(ToggleSuggestionMenu);
        // TODO setup all suggest menu buttons
        suggestMenu.SetActive(false);

        skillprofButton.onClick.AddListener(ToggleSkillProfMenu);
        PopulateSkillProfMenu();
        skillprofMenu.SetActive(false);
    }

    void Update()
    {
        
    }

    void ToggleSuggestionMenu()
    {
        if (!suggestMenu.activeSelf) 
            suggestMenu.SetActive(true);
        else 
            suggestMenu.SetActive(false);
    }

    void ToggleSkillProfMenu()
    {
        if (!skillprofMenu.activeSelf)
            skillprofMenu.SetActive(true);
        else
            skillprofMenu.SetActive(false);
    }

    void PopulateSkillProfMenu()
    {
        // TODO get skill and proficiency details from colonist
    }
    
    void CloseColonistMenu()
    {
        gameObject.SetActive(false);
    }
}
