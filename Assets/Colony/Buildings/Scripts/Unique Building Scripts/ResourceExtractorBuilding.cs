using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceExtractorBuilding : Building
{
    [Header("Laser")]
    public LineRenderer laser;
    public Vector3 laserStartPosition;
    Vector3 laserEndPosition;
    public Vector3 laserDestiantion;
    public float laserSpeed;

    [Header("Resource Particle System")]
    public ParticleSystem resourceParticles;

    [Header("Resource")]
    public ColonyResources.ResourceTypes selectedResource;

    protected override void Start()
    {
        base.Start();

        laserEndPosition = laserStartPosition;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Operation()
    {
        efficiency = colonists.Count / workStations.Length; // TODO alter based on colonist state?

        if (efficiency != 0)
        {
            if (!laser.enabled)
                laser.enabled = true;
            
            if (Vector3.Distance(laserEndPosition, laserDestiantion) < 1f)
            {
                // move to destination
                laserEndPosition = (laserDestiantion - laserEndPosition).normalized * laserSpeed * Time.deltaTime;
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
                    float particleLifetimeAndScale = Vector3.Distance(laserDestiantion, laserStartPosition); // for lifetime probably assumes something about speed
                    particleMain.startLifetime = particleLifetimeAndScale;

                    particleShape.position = laserDestiantion;
                    particleShape.rotation = Quaternion.LookRotation(laserEndPosition - laserStartPosition).eulerAngles;
                    particleShape.scale = new Vector3(1f, 1f, particleLifetimeAndScale);
                }
                
                // update resource
                ColonyResources.instance.colonyResources[productionResource] += efficiency * productionQuantity * Time.deltaTime;

                // keep the laser and particle system tracking the deposit
                laserEndPosition = laserDestiantion;
                particleShape.position = laserDestiantion;
                particleShape.rotation = Quaternion.LookRotation(laserStartPosition - laserEndPosition).eulerAngles;  
            }

            // energy use
            ColonyResources.instance.colonyResources[consumptionResource] -= efficiency * consumptionQuantity * Time.deltaTime;
        }
        else
        {
            // cleanup vars if no one working
            laserEndPosition = laserStartPosition;
            if (resourceParticles.isPlaying)
                resourceParticles.Stop();
        }
    }
}
