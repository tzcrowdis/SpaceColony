using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceExtractorBuilding : Building
{
    [Header("Laser")]
    public LineRenderer laser;
    public Transform laserStart;
    Vector3 laserEndPosition;
    public Vector3 laserDestination;
    public float laserSpeed;

    [Header("Resource Particle System")]
    public ParticleSystem resourceParticles;

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

    protected override void Operation()
    {
        efficiency = colonists.Count / workStations.Length; // TODO alter based on colonist state?

        if (efficiency != 0 & selectedResource != null)
        {
            if (!laser.enabled)
                laser.enabled = true;
            
            if (Vector3.Distance(laserEndPosition, laserDestination) > 1f)
            {
                // move end of laser to destination
                laserEndPosition = (laserDestination - laserStart.position).normalized * laserSpeed * Time.deltaTime;
            }
            else
            {
                var particleShape = resourceParticles.shape;

                // resource extraction
                if (!resourceParticles.isPlaying)
                {
                    resourceParticles.Play();

                    // first time resource particle system setup
                    var particleMain = resourceParticles.main;
                    float particleLifetimeAndScale = Vector3.Distance(laserDestination, laserStart.position); // for lifetime probably assumes something about speed
                    particleMain.startLifetime = particleLifetimeAndScale;

                    particleShape.position = laserDestination;
                    particleShape.rotation = Quaternion.LookRotation(laserDestination - laserStart.position).eulerAngles;
                    particleShape.scale = new Vector3(1f, 1f, particleLifetimeAndScale);
                }
                
                // update resource
                ColonyResources.instance.colonyResources[productionResource] += efficiency * productionQuantity * Time.deltaTime;

                // keep the laser and particle system tracking the deposit
                laserEndPosition = laserDestination;
                particleShape.position = laserDestination;
                particleShape.rotation = Quaternion.LookRotation(laserStart.position - laserEndPosition).eulerAngles;  
            }

            // energy use
            ColonyResources.instance.colonyResources[consumptionResource] -= efficiency * consumptionQuantity * Time.deltaTime;
        }
        else
        {
            // cleanup vars if no one working
            laserEndPosition = laserStart.position;
            if (resourceParticles.isPlaying)
                resourceParticles.Stop();
        }

        laser.SetPosition(laser.positionCount - 1, laserEndPosition);
    }

    public void ChangeSelectedResource(PlanetResource resource)
    {
        selectedResource = resource;

        // reset the laser
        if (selectedResource != null)
            laserDestination = selectedResource.transform.position;
        laserEndPosition = laserStart.position;
        resourceParticles.Stop();
    }

    bool CheckResourceDepleted()
    {
        // TODO
        return false;
    }
}
