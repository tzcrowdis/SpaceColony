using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class Building : MonoBehaviour
{
    [Header("Physical Building & Connection Points")]
    public GameObject building;
    public GameObject connectionPoints;
    
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

    [Header("Colonists")]
    public GameObject[] workStations;
    [HideInInspector]
    public List<Colonist> colonists; // NOTE: line up indexes with work station
    int colonistCapacity;
    [HideInInspector]
    public float efficiency;

    [Header("Active Building UI Panel")]
    public GameObject activeBuildingPanel;

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
        r = building.GetComponent<Renderer>();
        ogColor = r.material.color;

        colonistCapacity = workStations.Length;
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

        BuildingClicked();
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

            // activate all connection points
            foreach (Transform connection in connectionPoints.transform)
            {
                // TODO in the future consider only activating exterior connections
                connection.gameObject.SetActive(true);
            }

            // rebake nav mesh surface
            GameObject.Find("Colony NavMesh Surface").GetComponent<NavMeshSurface>().BuildNavMesh();

            state = State.Operating;
        }
    }

    void Operation()
    {
        efficiency = colonists.Count / colonistCapacity;

        // production
        ColonyResources.instance.colonyResources[productionResource] += efficiency * productionQuantity * Time.deltaTime;

        // consumption
        ColonyResources.instance.colonyResources[consumptionResource] -= efficiency * consumptionQuantity * Time.deltaTime;
    }



    void BuildingClicked() // NOTE requires collider
    {
        if (state  == State.Operating)
        {
            Camera camera = Camera.main;
            Mouse mouse = Mouse.current;
            if (mouse.leftButton.wasPressedThisFrame)
            {
                Vector3 mousePosition = mouse.position.ReadValue();
                Ray ray = camera.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.gameObject == this)
                    {
                        Instantiate(activeBuildingPanel, mousePosition, Quaternion.identity, ColonyUI.instance.transform);
                    }
                }
            }
        }
    }
}
