using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class PlanetInfoPanel : MonoBehaviour
{
    OrbitingPlanet planet;

    public Vector3 infoPanelOffset;
    
    [Header("Text Properties")]
    public TMP_Text planetName;
    public TMP_Text planetDescription;

    void Start()
    {
        planet = GameObject.FindGameObjectWithTag("Planet").GetComponent<OrbitingPlanet>();

        planetName.text = planet.planetName;
        
        planetDescription.fontSize = planetName.fontSize - 2;
        planetDescription.enabled = false;
        planetDescription.text = planet.planetDescription;
    }
}
