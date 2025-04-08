using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionBuilding : Building
{
    [Header("Energy Motion")]
    public GameObject loop;
    public float loopRotationSpeed;

    public override float BuildingEfficiency()
    {
        return 1f;
    }

    protected override void Update()
    {
        base.Update();

        if (state == State.Operating)
            loop.transform.Rotate(new Vector3(0, loopRotationSpeed, 0));
    }
}
