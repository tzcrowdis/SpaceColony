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

    [Header("Colonists")]
    public GameObject[] workStations;
    [HideInInspector]
    public List<Colonist> colonists; // NOTE: line up indexes with work station
    //[HideInInspector]
    //public float efficiency;

    [Header("Active Building UI Panel")]
    public GameObject activeBuildingPanelPrefab;
    GameObject bldgPanel;
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
        

        // spawn active building panel object for this building
        bldgPanel = Instantiate(activeBuildingPanelPrefab, Vector3.zero, Quaternion.identity, ColonyUI.instance.transform);
        bldgPanel.GetComponent<ActiveBuildingPanel>().building = this;
        bldgPanel.name = $"{gameObject.name} Panel";
        bldgPanel.SetActive(false);

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

    public virtual float BuildingEfficiency()
    {
        if (workStations.Length > 0)
        {
            float efficiency = 0;
            foreach (Colonist colonist in colonists)
            {
                efficiency += colonist.workEfficiency;
            }
            return efficiency / workStations.Length;
        }
        else
        {
            return 0;
        }
    }


    // MISC FUNCTIONS
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
                        bldgPanel.transform.position = mousePosition;
                        bldgPanel.SetActive(true);

                        panelOpen = true;
                    }
                }
            }
        }
    }
}
