using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
    public int planetResourceCount;
    public GameObject planetResourcePrefab;
    public GameObject resourceInfoPanel;
    [HideInInspector] public List<PlanetResource> planetResources;

    float planetRadius;

    void Start()
    {        
        rotationEuler = new Vector3(0, rotationSpeed, 0);

        planetRadius = transform.localScale.x / 2;
        GenerateRandomPlanetResources();
    }

    void FixedUpdate()
    {
        PlanetRotation();
    }

    void PlanetRotation()
    {
        transform.Rotate(rotationEuler * Time.fixedDeltaTime);
    }

    void GenerateRandomPlanetResources()
    {
        for (int i = 0; i < planetResourceCount; i++)
        {
            // setup the resource
            GameObject resource = Instantiate(planetResourcePrefab, transform);
            PlanetResource resourceComp = resource.GetComponent<PlanetResource>();
            if (resourceComp)
            {
                resourceComp.depositInfoPanel = resourceInfoPanel;
                resourceComp.RandomizeResource();
                planetResources.Add(resourceComp);
            }

            // find the resource position
            for (int iter = 0; iter < 100; iter++) // give up after 100 attempts
            {
                Vector2 longLat = new Vector2(Random.Range(0, 2 * Mathf.PI), Random.Range(0f, Mathf.PI));
                Vector3 position = planetRadius * 
                    new Vector3(
                        Mathf.Cos(longLat[0]) * Mathf.Sin(longLat[1]),
                        Mathf.Sin(longLat[0]) * Mathf.Sin(longLat[1]),
                        Mathf.Cos(longLat[1])
                    );

                bool validPos = true;
                foreach (PlanetResource rsrc in planetResources)
                {
                    if (Vector3.Distance(rsrc.transform.position, position) < 100f)
                    {
                        validPos = false; 
                        break;
                    }
                }

                if (validPos)
                {
                    resource.transform.position = position + transform.position;
                    break;
                }
            }
            
            // orient resource out from center
            Vector3 outwardVector = (resource.transform.position - transform.position).normalized;
            resource.transform.up = outwardVector;
        }
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
