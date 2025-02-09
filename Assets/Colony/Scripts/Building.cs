using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Header("Building Construction Variables")]
    public int genericCost;
    public int energyCost;

    public float constructionTime;

    [Header("Building Production Variables")]
    public ColonyResources.ResourceTypes resource;
    [Tooltip("resource per second")]
    public float productionQuantity;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void PlaceBuilding()
    {
        // enact cost of building
        ColonyResources.instance.colonyResources[ColonyResources.ResourceTypes.Generic] -= genericCost;
        ColonyResources.instance.colonyResources[ColonyResources.ResourceTypes.Energy] -= energyCost;

        // TODO add construction delay (animations, effects, etc) before adding colliders

        // activate all colliders
        GetComponent<BoxCollider>().enabled = true;
        foreach (Transform connections in transform)
        {
            // TODO in the future consider only activating exterior connections

            connections.gameObject.SetActive(true);
        }

        // TODO once construction is completed enable resource, workers, and other features
    }
}
