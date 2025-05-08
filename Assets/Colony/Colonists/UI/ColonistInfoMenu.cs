using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ColonistInfoMenu : MonoBehaviour
{
    [HideInInspector]
    public Colonist colonist; // NOTE set by colonist list item on click

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

    [Header("Suggestion Menu")]
    public Button workSuggestion;
    public Button eatSuggestion;
    public Button sleepSuggestion;
    public Button noneSuggestion;
    Button[] allSuggestButtons;

    [Header("Skills & Proficiencies Menu")]
    public GameObject skillsContent;
    public GameObject skillPrefab;
    public GameObject proficiencyContent;
    public GameObject proficiencyPrefab;

    void Start()
    {
        colonistName.text = colonist.characterName;
        exitButton.onClick.AddListener(CloseColonistMenu);

        // occupation
        occupationDropdown.ClearOptions();
        foreach (var r in Enum.GetValues(typeof(Colonist.JobTypes)))
            occupationDropdown.options.Add(new TMP_Dropdown.OptionData() { text = r.ToString() });
        occupationDropdown.onValueChanged.AddListener(colonist.ChangeColonistsJob);

        // mental state
        mentalState.text = $"Mental State: {colonist.mentalState.ToString()}";

        // suggestions
        suggestLabel.text = $"Suggestion: {colonist.suggestion.ToString()}";
        suggestButton.onClick.AddListener(ToggleSuggestionMenu);
        workSuggestion.onClick.AddListener(delegate { Suggest(workSuggestion.transform.GetComponentInChildren<TMP_Text>().text, workSuggestion); });
        eatSuggestion.onClick.AddListener(delegate { Suggest(eatSuggestion.transform.GetComponentInChildren<TMP_Text>().text, eatSuggestion); });
        sleepSuggestion.onClick.AddListener(delegate { Suggest(sleepSuggestion.transform.GetComponentInChildren<TMP_Text>().text, sleepSuggestion); });
        noneSuggestion.onClick.AddListener(delegate { Suggest(noneSuggestion.transform.GetComponentInChildren<TMP_Text>().text, noneSuggestion); });
        // etc.
        allSuggestButtons = new Button[]{
            workSuggestion,
            eatSuggestion,
            sleepSuggestion,
            noneSuggestion
        };
        InitializeSuggestionMenu();
        suggestMenu.SetActive(false);

        // skills and proficiencies
        skillprofButton.onClick.AddListener(ToggleSkillProfMenu);
        PopulateSkillProfMenu();
        skillprofMenu.SetActive(false);
    }

    void Update()
    {
        mentalState.text = $"Mental State: {colonist.mentalState.ToString()}";
    }

    void CloseColonistMenu()
    {
        gameObject.SetActive(false);
    }

    /*
     * SUGGESTION MENU
     */
    void ToggleSuggestionMenu()
    {
        if (!suggestMenu.activeSelf) 
            suggestMenu.SetActive(true);
        else 
            suggestMenu.SetActive(false);
    }

    void InitializeSuggestionMenu()
    {
        foreach (Button suggestBtn in allSuggestButtons)
        {
            string suggestBtnText = suggestBtn.transform.GetComponentInChildren<TMP_Text>().text;
            if (colonist.suggestion.ToString() == suggestBtnText)
                suggestBtn.interactable = false;
            else
                suggestBtn.interactable = true;
        }
    }

    void Suggest(string suggestion, Button btn)
    {
        colonist.MakeSuggestion((Colonist.Suggestion)Enum.Parse(typeof(Colonist.Suggestion), suggestion));

        foreach (Button suggestBtn in allSuggestButtons)
        {
            if (btn == suggestBtn)
                btn.interactable = false;
            else
                btn.interactable = true;
        }
    }

    /*
     * SKILLS + PROFICIENCY MENU
     */

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
}
