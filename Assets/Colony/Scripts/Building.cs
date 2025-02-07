using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class Building : MonoBehaviour
{
    bool selected;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void PlaceBuilding()
    {
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
