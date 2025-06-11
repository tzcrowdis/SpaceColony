using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using TMPro;

public class ColonistListItem : MonoBehaviour
{
    public Colonist colonist;

    [Header("Buttons")]
    public Button colonistButton;

    [Header("Focus on Colonist")]
    public float focusStopDistance;
    public float focusSpeed;

    // cam focus vars
    Camera cam;
    bool engageFocus;
    Vector3 camPositionStart = Vector3.one;
    Vector3 focusDestination = Vector3.one;
    float tol = 0.1f;
    Vector3 camForwardStart = Vector3.one;
    Vector3 focusForwardVec = Vector3.one;
    float focusTime = 0f;


    void Start()
    {
        transform.GetChild(0).gameObject.GetComponent<Image>().sprite = colonist.headshot;
        transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = colonist.characterName;
        
        colonistButton.onClick.AddListener(ListItemClicked);

        cam = Camera.main;

        if (colonist.requiresPlayerAttention)
            ColonistAlert();
    }

    void Update()
    {
        if (engageFocus)
            FocusColonist();
    }

    void ListItemClicked()
    {
        engageFocus = true;

        // makes sure building menu isn't already open
        ColonistInfoMenu[] menus = colonist.colonistMenuParent.GetComponentsInChildren<ColonistInfoMenu>();
        foreach (ColonistInfoMenu clnstMenu in menus)
            if (clnstMenu.colonist == colonist) return;

        // otherwise open menu
        GameObject menu = Instantiate(colonist.colonistMenuPrefab, colonist.colonistMenuParent);
        menu.GetComponent<ColonistInfoMenu>().colonist = colonist;
        if (menus.Length > 0)
            menu.transform.position = menus[menus.Length - 1].transform.position + new Vector3(25f, -25f, 0);
    }

    void FocusColonist()
    {
        // find point inbetween camera and building that's a dist of x from the building
        if (focusDestination == Vector3.one)
        {
            focusDestination = (cam.transform.position - colonist.transform.position).normalized * focusStopDistance + colonist.transform.position;
            focusDestination.y = colonist.transform.position.y; // locks rotation to y-axis

            camPositionStart = cam.transform.position;
        }
        cam.transform.position = Vector3.Lerp(camPositionStart, focusDestination, focusTime * focusSpeed);

        // rotate towards normal from cam og point to building
        if (focusForwardVec == Vector3.one)
        {
            focusForwardVec = (colonist.transform.position - focusDestination).normalized;

            camForwardStart = cam.transform.forward;
        }
        cam.transform.rotation = Quaternion.LookRotation(Vector3.Lerp(camForwardStart, focusForwardVec, focusTime * focusSpeed));

        // increment focous/lerp time
        focusTime += Time.deltaTime;

        // check if focused
        if (Vector3.Distance(cam.transform.position, focusDestination) < tol & Mathf.Approximately(Vector3.Dot(cam.transform.forward, focusForwardVec), 1f))
        {
            cam.transform.position = focusDestination;
            cam.transform.rotation = Quaternion.LookRotation(focusForwardVec);
            
            engageFocus = false;
            focusDestination = Vector3.one;
            focusForwardVec = Vector3.one;
            focusTime = 0f;
        }
    }

    public void ColonistAlert()
    {
        // TODO some other means of alerting player

        ColorBlock cb = colonistButton.colors;
        cb.normalColor = Color.red;
        colonistButton.colors = cb;
    }

    public void ClearColonistAlert()
    {
        // NOTE assumes only thing was making the button red

        colonistButton.colors = ColorBlock.defaultColorBlock;
    }
}
