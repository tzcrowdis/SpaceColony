using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetResource : MonoBehaviour
{
    public ColonyResources.ResourceTypes resource;
    public float resourceQuantity;
    bool extracting = false;

    public Transform extractionLocation;
    public Transform laserLocation; // set by extractor building

    [Header("Resource Particle System")]
    public ParticleSystem resourceParticles;

    private void Start()
    {
        resourceParticles = GetComponent<ParticleSystem>();
    }

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
        /*extracting = true;

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
        }*/
    }

    public void StopExtractingResource()
    {
        //resourceParticles.Stop();
        extracting = false;
    }

    public void Depleted()
    {
        Destroy(gameObject);
    }
}
