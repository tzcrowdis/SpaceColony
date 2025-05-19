using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    public Colonist stationedColonist;

    public enum StationType
    {
        Work,
        Sleep,
        Eat,
        Medical,
        Rest
    }
    public StationType type;

    public float GetWorkStationEfiiciency() // TODO make general so all stations have efficiency?
    {
        if (stationedColonist == null || type != StationType.Work)
            return 0;

        return stationedColonist.workEfficiency;
    }
}
