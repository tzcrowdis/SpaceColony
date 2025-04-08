using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceExtractorBuilding : Building
{
    [Header("Laser")]
    public LineRenderer laser;
    public Transform laserStart;
    Vector3 laserEndPosition;
    public Vector3 laserDestination; // maybe could drop this due to extractionLocation
    public float laserSpeed;

    [Header("Resource")]
    public PlanetResource selectedResource;

    // TODO track orbiting planet and resource deposits

    protected override void Start()
    {
        base.Start();

        laserEndPosition = laserStart.position;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Idle()
    {
        base.Idle();

        laser.enabled = false;
    }

    protected override void Operation()
    {
        // check line of sight from laser to deposit
        RaycastHit hit;
        Physics.Raycast(laserStart.position, (laserDestination - laserStart.position).normalized, out hit, 2 * Vector3.Distance(laserDestination, laserStart.position));
        
        // Operate Laser
        if (hit.collider.CompareTag("Planet Resource") & BuildingEfficiency() != 0 & selectedResource != null)
        {
            OperateLaser();
        }
        else
        {
            // cleanup vars if cant work
            laserEndPosition = laserStart.position;

            // TODO send signal to colonist to idle?
        }

        // always track location of resource
        if (selectedResource != null)
            laserDestination = selectedResource.extractionLocation.position;

        laser.SetPosition(laser.positionCount - 1, laserEndPosition);

        CheckResourceDepleted();
    }

    void OperateLaser()
    {
        if (!laser.enabled)
        {
            laser.enabled = true;
            laser.SetPosition(0, laserStart.position);
        }

        bool produced = true;
        if (Vector3.Distance(laserEndPosition, laserDestination) > 1f)
        {
            // set end of laser to destination
            laserEndPosition = laserDestination;
        }
        else
        {
            float extractionRate = BuildingEfficiency() * productionQuantity * Time.deltaTime;

            // increment resource
            produced = ColonyResources.instance.ProduceResource(productionResource, extractionRate);

            // decrement deposit
            selectedResource.ExtractResource(extractionRate);

            // keep the laser and particle system tracking the deposit
            laserEndPosition = laserDestination;
        }

        // energy use
        bool consumed = ColonyResources.instance.ConsumeResource(consumptionResource, BuildingEfficiency() * consumptionQuantity * Time.deltaTime);

        // no energy or no space then idle
        if (!produced | !consumed)
            state = State.Idle;
    }

    public void ChangeSelectedResource(PlanetResource resource)
    {
        selectedResource = resource; // TODO add None resourceq

        // update production
        if (resource != null)
            productionResource = resource.resource;
        else
            productionQuantity = 0;

        // reset the laser
        if (selectedResource != null)
            laserDestination = selectedResource.extractionLocation.position;

        laserEndPosition = laserStart.position;
        //resourceParticles.Stop();

        selectedResource.laserLocation = laserStart;
    }

    bool CheckResourceDepleted()
    {
        // TODO react to condition
        if (selectedResource != null && selectedResource.resourceQuantity <= 0)
        {
            selectedResource.Depleted();
            return true;
        }
        else
            return false;
    }
}
