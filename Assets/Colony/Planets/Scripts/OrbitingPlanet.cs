using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class OrbitingPlanet : MonoBehaviour
{
    [Header("Planet Info")]
    public string planetName;
    [TextArea(10, 10)]
    public string planetDescription;
    public GameObject planetInfoPanel;
    
    [Header("Orbit Variables")]
    [Tooltip("degrees per second around y-axis")]
    public float rotationSpeed;
    Vector3 rotationEuler;

    [Header("Resources")]
    [HideInInspector]
    public List<PlanetResource> planetResources;

    void Start()
    {        
        rotationEuler = new Vector3(0, rotationSpeed, 0);

        foreach (Transform child in transform)
        {
            if (child.CompareTag("Planet Resource"))
                planetResources.Add(child.gameObject.GetComponent<PlanetResource>());
        }
    }

    void FixedUpdate()
    {
        PlanetRotation();
    }

    void PlanetRotation()
    {
        transform.Rotate(rotationEuler * Time.fixedDeltaTime);
    }

    /*
     * INFO PANEL FUNCTIONS
     */
    private void OnMouseEnter()
    {
        planetInfoPanel.SetActive(true);
    }

    private void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            // set active
            planetInfoPanel.SetActive(true);

            // track mouse
            Mouse mouse = Mouse.current;
            Vector3 mousePosition = mouse.position.ReadValue();
            mousePosition += planetInfoPanel.GetComponent<PlanetInfoPanel>().infoPanelOffset;
            planetInfoPanel.transform.position = mousePosition;

            // display lore on click
            if (mouse.leftButton.wasPressedThisFrame)
            {
                planetInfoPanel.GetComponent<PlanetInfoPanel>().planetDescription.enabled = true;
            }
        }
        else
        {
            planetInfoPanel.SetActive(false);
            planetInfoPanel.GetComponent<PlanetInfoPanel>().planetDescription.enabled = false;
        }
    }
    
    private void OnMouseExit()
    {
        planetInfoPanel.SetActive(false);
        planetInfoPanel.GetComponent<PlanetInfoPanel>().planetDescription.enabled = false;
    }
}
