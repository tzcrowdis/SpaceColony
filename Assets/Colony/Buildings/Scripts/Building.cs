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

    [Header("Colonists")]
    public Transform workStationsParent;
    [HideInInspector]
    public List<Station> workStations;

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
    public Collider clickCollider;

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
    Color ogColor;
    Renderer r;

    protected virtual void Start()
    {
        //state = State.Blueprint;
        try
        {
            r = building.GetComponent<Renderer>();
            ogColor = r.material.color;
        }
        catch
        {
            Debug.Log($"renderer not found on {gameObject.name}");
        }

        // work stations
        foreach (Transform child in workStationsParent)
            workStations.Add(child.GetComponent<Station>());

        clickCollider = GetComponent<Collider>();
        if (state == State.Blueprint)
            clickCollider.enabled = false;
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

        BuildingClicked();
    }

    public void PlaceBuilding()
    {
        // enact cost of building
        ColonyResources.instance.colonyResources[ColonyResources.ResourceTypes.Generic] -= genericCost;
        ColonyResources.instance.colonyResources[ColonyResources.ResourceTypes.Energy] -= energyCost;

        // add to building list
        buildingListItem = BuildingList.instance.AddBuildingToList(this);
        ColonyResources.instance.AddBuildingToTypeList(this);

        state = State.Construction;
    }

    protected void Blueprint()
    {
        if (ColonyControls.instance.overValidConnectionPoint)
            r.material.SetColor("_BaseColor", Color.green);
        else
            r.material.SetColor("_BaseColor", Color.blue);

        // exited by colony controls
    }

    protected void Construction()
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
            NavMeshSurface surface = GameObject.Find("Colony NavMesh Surface").GetComponent<NavMeshSurface>();
            surface.BuildNavMesh();

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
     * WORK FUNCTIONS
     */
    public virtual float BuildingEfficiency()
    {
        if (workStations.Count > 0)
        {
            float efficiency = 0;
            foreach (Station station in workStations)
            {
                efficiency += station.GetWorkStationEfiiciency();
            }
            return efficiency / workStations.Count;
        }
        else
        {
            return 0;
        }
    }

    public int OccupiedWorkStationCount()
    {
        int count = 0;
        foreach (Station station in workStations)
        {
            if (station.stationedColonist != null)
                count++;
        }
        return count;  
    }

    public Station GetEmptyWorkStation()
    {
        if (OccupiedWorkStationCount() >= workStations.Count)
            Debug.Log($"no empty work stations at {gameObject.name}"); 

        foreach (Station station in workStations)
        {
            if (station.stationedColonist == null && station.type == Station.StationType.Work)
                return station;
        }

        return null;
    }

    public void RemoveColonistFromWorkplace(Colonist colonist)
    {
        foreach (Station station in workStations)
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
    protected void BuildingClicked() // NOTE requires collider
    {
        // TODO should this all just be in the OnMouseOver function?
        
        if (state == State.Operating & !panelOpen & !EventSystem.current.IsPointerOverGameObject())
        {
            Camera camera = Camera.main;
            Mouse mouse = Mouse.current;
            if (mouse.leftButton.wasPressedThisFrame)
            {
                Vector3 mousePosition = mouse.position.ReadValue();
                Ray ray = camera.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        bldgMenu.transform.position = mousePosition;
                        bldgMenu.SetActive(true);

                        panelOpen = true;
                    }
                }
            }
        }
    }

    void OnDestroy()
    {
        // TODO handle colonists inside building or other edge cases
    }
}
