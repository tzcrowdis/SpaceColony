using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class Building : MonoBehaviour
{
    bool selected;

    [Tooltip("Dimensions should be from the origin of the object. So if it has a width of 1 along the x-axis the x should be 0.5.")]
    public Vector3 dimensions;

    void Start()
    {
        // testing on unit cube
        dimensions = new Vector3(0.5f, 0.5f, 0.5f);
    }

    void Update()
    {

    }
}
