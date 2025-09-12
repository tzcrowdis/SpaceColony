using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlanetResource : MonoBehaviour
{
    [Header("Resource")]
    public ColonyResources.ResourceTypes resource;
    public float resourceQuantity;
    
    [Header("Resource Visualization")]
    public GameObject metalsModel;
    public GameObject organicsModel;
    public GameObject electricityModel;
    public GameObject researchModel;
    public GameObject foodModel;
    public GameObject colonistModel;
    GameObject model;

    [Header("Resource Quantity Ranges")]
    public Vector2 metalsQuantityRange;
    public Vector2 organicsQuantityRange;
    public Vector2 electricityQuantityRange;
    public Vector2 researchQuantityRange;
    public Vector2 foodQuantityRange;

    [Header("Extraction")]
    public Transform extractionLocation;
    public Transform laserLocation; // set by extractor building
    //bool extracting = false;

    [Header("Info Panel")]
    public GameObject depositInfoPanel;

    /*
     * INSTANTIATION
     */
    public void RandomizeResource()
    {
        int rand = Random.Range(0, System.Enum.GetNames(typeof(ColonyResources.ResourceTypes)).Length - 1);
        resource = (ColonyResources.ResourceTypes)rand;

        switch (resource)
        {
            case ColonyResources.ResourceTypes.Metals:
                model = Instantiate(metalsModel, transform);
                resourceQuantity = Random.Range(metalsQuantityRange[0], metalsQuantityRange[1]);
                break;
            case ColonyResources.ResourceTypes.Organics:
                model = Instantiate(organicsModel, transform);
                resourceQuantity = Random.Range(organicsQuantityRange[0], organicsQuantityRange[1]);
                break;
            case ColonyResources.ResourceTypes.Electricity:
                model = Instantiate(electricityModel, transform);
                resourceQuantity = Random.Range(electricityQuantityRange[0], electricityQuantityRange[1]);
                break;
            case ColonyResources.ResourceTypes.Research:
                model = Instantiate(researchModel, transform);
                resourceQuantity = Random.Range(researchQuantityRange[0], researchQuantityRange[1]);
                break;
            case ColonyResources.ResourceTypes.Food:
                model = Instantiate(foodModel, transform);
                resourceQuantity = Random.Range(foodQuantityRange[0], foodQuantityRange[1]);
                break;
        }

        model.transform.localScale = Vector3.one * 0.15f;

        PlanetResourceInfoPanel infoPanel = gameObject.GetComponentInChildren<PlanetResourceInfoPanel>();
        infoPanel.SetVariables(this, depositInfoPanel);
    }

    /*
     * EXTRACTION
     */
    private void Update()
    {
        /*if (extracting)
        {
            // particle system tracking based on it's motion
            var particleMain = resourceParticles.main;
            var particleShape = resourceParticles.shape;
            float particleLifetimeAndScale = Vector3.Distance(extractionLocation.position, laserLocation.position);

            particleMain.startLifetime = particleLifetimeAndScale;
            //particleShape.position = new Vector3(0, 0, 500);
            particleShape.rotation = Quaternion.LookRotation(laserLocation.position - extractionLocation.position).eulerAngles;
            particleShape.scale = new Vector3(1f, 1f, particleLifetimeAndScale);
        }*/
    }

    public void ExtractResource(float extractionRate)
    {
        resourceQuantity -= extractionRate;

        /*
        extracting = true;

        if (!resourceParticles.isPlaying)
        {
            var particleMain = resourceParticles.main;
            var particleShape = resourceParticles.shape;
            float particleLifetimeAndScale = Vector3.Distance(extractionLocation.position, laserLocation.position);
            
            particleMain.startLifetime = particleLifetimeAndScale;
            //particleShape.position = new Vector3(0, 0, 500);
            particleShape.rotation = Quaternion.LookRotation(laserLocation.position - extractionLocation.position).eulerAngles;
            particleShape.scale = new Vector3(1f, 1f, particleLifetimeAndScale);

            resourceParticles.Play();
        }
        */
    }

    public void StopExtractingResource()
    {
        //resourceParticles.Stop();
        //extracting = false;
    }

    public void Depleted()
    {
        Destroy(gameObject);
    }
}
