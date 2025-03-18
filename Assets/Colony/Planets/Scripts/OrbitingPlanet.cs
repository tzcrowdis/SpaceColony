using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingPlanet : MonoBehaviour
{
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
}
