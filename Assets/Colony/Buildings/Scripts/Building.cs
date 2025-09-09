using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Building : MonoBehaviour
{
    [Header("Physical Building & Connection Points")]
    public GameObject building;
    public GameObject connectionPoints;
    
    [Header("Building Construction Variables")]
    public int metalsCost;
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

    public enum BuildingType
    {
        Generic,
        Energy,
        Extraction,
        Farm,
        Research,
        Storage,
        Cafeteria,
        Medical,
        Recreation,
        Sleep
    }
    [Header("Building Type")]
    public BuildingType buildingType;

    [Header("Colonists")] // TODO: generalize to station
    public Transform stationsParent;
    [HideInInspector]
    public List<Station> stations;

    [Header("Building UI")]
    //TODO reworking...
    public GameObject buildingMenuPrefab;
    GameObject bldgMenu;
    [HideInInspector]
    public BuildingListItem buildingListItem;
    public Sprite icon;
    public string title;
    public bool requiresPlayerAttention;
    // drop vars when building clicking dropped
    public bool panelOpen = false;

    public enum State
    {
        Blueprint,
        Construction,
        Operating, 
        Idle
    }
    [Header("Building Working State")]
    public State state;

    // temp vars for construction
    public Color ogColor;
    public Renderer render;

    protected virtual void Start()
    {
        //state = State.Blueprint;
        try
        {
            render = building.GetComponent<Renderer>();
            ogColor = render.material.color;
        }
        catch
        {
            Debug.Log($"renderer not found on {gameObject.name}");
        }

        // work stations
        foreach (Transform child in stationsParent)
            stations.Add(child.GetComponent<Station>());

        foreach (ConnectionPoint connection in connectionPoints.transform.GetComponentsInChildren<ConnectionPoint>())
            connection.building = this;
    }

    protected virtual void Update()
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

            case State.Idle:
                Idle();
                break;
        }
    }

    public void PlaceBuilding()
    {
        // enact cost of building
        ColonyResources.instance.displayBuildingCost = 0;
        ColonyResources.instance.colonyResources[ColonyResources.ResourceTypes.Metals] -= metalsCost;

        // add to building list
        buildingListItem = BuildingList.instance.AddBuildingToList(this);
        ColonyResources.instance.AddBuildingToTypeList(this);

        // disables on mouse events for building connections
        int buildingConnectionLayer = 1 << LayerMask.NameToLayer("BuildingConnection");
        ColonyControls.instance.GetComponent<Camera>().eventMask &= ~buildingConnectionLayer;

        // activate all connection points
        foreach (Transform connection in connectionPoints.transform)
            connection.gameObject.SetActive(true);
        BuildingManager.instance.UpdateActiveConnectionPoints();

        state = State.Construction;
    }

    protected void Blueprint()
    {
        if (ColonyControls.instance.overValidConnectionPoint)
            render.material.SetColor("_BaseColor", Color.green);
        else
            render.material.SetColor("_BaseColor", Color.blue);

        // exited by colony controls
    }

    protected void Construction()
    {
        // construction begin
        if (Mathf.Approximately(constructionTime, 0f))
        {
            // TODO add (animations, effects, etc) for construction
            render.material.SetColor("_BaseColor", Color.yellow);
        }
        
        // constructing
        // TODO modify construction speed based on workers
        constructionTime += Time.deltaTime;

        // construction complete
        if (constructionTime > totalConstructionTime)
        {
            render.material.SetColor("_BaseColor", ogColor);

            // rebake nav mesh surface
            BuildingManager.instance.navSurface.BuildNavMesh();

            state = State.Operating;
        }
    }

    protected virtual void Operation()
    {
        // consumption
        bool consumed = ColonyResources.instance.ConsumeResource(consumptionResource, BuildingEfficiency() * consumptionQuantity * Time.deltaTime);
        
        // production
        bool produced = ColonyResources.instance.ProduceResource(productionResource, BuildingEfficiency() * productionQuantity * Time.deltaTime);

        if (!consumed | !produced)
            state = State.Idle;
    }

    protected virtual void Idle()
    {
        // check if we can go back to operating
        if (!ColonyResources.instance.FullOrEmptyResource(productionResource) && !ColonyResources.instance.FullOrEmptyResource(consumptionResource))
            state = State.Operating;
    }

    /*
     * STATION FUNCTIONS
     */
    public virtual float BuildingEfficiency()
    {
        if (stations.Count > 0)
        {
            float efficiency = 0;
            foreach (Station station in stations)
            {
                efficiency += station.GetWorkStationEfiiciency();
            }
            return efficiency / stations.Count;
        }
        else
        {
            return 0;
        }
    }

    public int OccupiedStationCount(Station.StationType type)
    {
        int count = 0;
        foreach (Station station in stations)
        {
            if (station.stationedColonist != null && station.type == type)
                count++;
        }
        return count;
    }

    public Station GetEmptyStation(Station.StationType type)
    {
        if (OccupiedStationCount(type) >= StationCount(type))
            Debug.Log($"no empty stations at {gameObject.name}"); 

        foreach (Station station in stations)
        {
            if (station.stationedColonist == null && station.type == type)
                return station;
        }

        return null;
    }

    public int StationCount(Station.StationType type)
    {
        int count = 0;
        foreach (Station station in stations)
        {
            if (station.type == type)
                count++;
        }
        return count;
    }

    public void RemoveColonistFromWorkplace(Colonist colonist)
    {
        foreach (Station station in stations)
        {
            if (station.stationedColonist == colonist && station.type == Station.StationType.Work)
            {
                station.stationedColonist = null;
                colonist.workStation = null;
                return;
            }
        }
    }


    /*
     * MISC FUNCTIONS
     */
    void OnDestroy()
    {
        // TODO handle colonists inside building or other edge cases
        // TODO make sure destroying this building isn't stranding colonists
        // check via navmesh path calcs..?

        BuildingManager.instance.UpdateActiveConnectionPoints(); // NOTE actually needs to be after destroy
        BuildingManager.instance.navSurface.BuildNavMesh();
    }
}
