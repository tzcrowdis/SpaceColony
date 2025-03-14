using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetResource : MonoBehaviour
{
    public ColonyResources.ResourceTypes resource;
    public float resourceQuantity;

    public void ExtractResource(float extractionRate)
    {
        resourceQuantity -= extractionRate;
    }
}
