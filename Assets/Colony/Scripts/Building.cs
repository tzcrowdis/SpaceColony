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
    // etc.
    [Tooltip("real time seconds")]
    public float totalConstructionTime;
    float constructionTime = 0f;

    [Header("Building Production")]
    public ColonyResources.ResourceTypes productionResource;
    [Tooltip("resource per second")]
    public float productionQuantity;

    [Header("Building Consumption")]
    public ColonyResources.ResourceTypes consumptionResource;
    [Tooltip("resource per second")]
    public float consumptionQuantity;

    public enum State
    {
        Blueprint,
        Construction,
        Operating
    }
    [Header("Building Working State")]
    public State state;

    // temp vars for construction
    Color ogColor;
    Renderer r;

    void Start()
    {
        //state = State.Blueprint;

        r = GetComponent<Renderer>();
        ogColor = r.material.color;
    }

    void Update()
    {
        switch (state)
        {
            case State.Blueprint:
                Blueprint();
                break;

            case State.Construction:
                Construction();
                break;

            case State.Operating:
                Operation();
                break;
        }
    }

    public void PlaceBuilding()
    {
        // enact cost of building
        ColonyResources.instance.colonyResources[ColonyResources.ResourceTypes.Generic] -= genericCost;
        ColonyResources.instance.colonyResources[ColonyResources.ResourceTypes.Energy] -= energyCost;

        state = State.Construction;
    }

    void Blueprint()
    {
        if (ColonyControls.instance.overConnectionPoint)
            r.material.SetColor("_BaseColor", Color.green);
        else
            r.material.SetColor("_BaseColor", Color.blue);

        // exited by colony controls
    }

    void Construction()
    {
        // construction begin
        if (Mathf.Approximately(constructionTime, 0f))
        {
            // TODO add (animations, effects, etc) for construction
            r.material.SetColor("_BaseColor", Color.yellow);
        }
        
        // constructing
        // TODO modify construction speed based on workers
        constructionTime += Time.deltaTime;

        // construction complete
        if (constructionTime > totalConstructionTime)
        {
            r.material.SetColor("_BaseColor", ogColor);

            // activate all colliders
            foreach (Collider c in GetComponents<Collider>())
                c.enabled = true;
            foreach (Transform connections in transform)
            {
                // TODO in the future consider only activating exterior connections
                connections.gameObject.SetActive(true);
            }

            state = State.Operating;
        }
    }

    void Operation()
    {
        // TODO track workers and modifiers from them

        // production
        ColonyResources.instance.colonyResources[productionResource] += productionQuantity * Time.deltaTime;

        // consumption
        ColonyResources.instance.colonyResources[consumptionResource] -= consumptionQuantity * Time.deltaTime;
    }
}
