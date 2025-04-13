using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchBuilding : Building
{
    [Header("Quantum Particle System")]
    public ParticleSystem particles;

    protected override void Update()
    {
        base.Update();

        if (BuildingEfficiency() > 0 & !particles.isPlaying)
            particles.Play();
        else
            particles.Pause();
    }
}
