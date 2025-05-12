using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkStation : MonoBehaviour
{
    public Colonist stationedColonist;

    public float GetWorkStationEfiiciency()
    {
        return stationedColonist.workEfficiency;
    }
}
