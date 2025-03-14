using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingPlanet : MonoBehaviour
{
    [Header("Orbit Variables")]
    [Tooltip("degrees per second around y-axis")]
    public float rotationSpeed;
    Vector3 rotationEuler;

    void Start()
    {
        rotationEuler = new Vector3(0, rotationSpeed, 0);
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
