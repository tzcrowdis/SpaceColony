using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkStation : MonoBehaviour
{
    // TODO make this a generic location marker and have type as an option (Work, Eat, Sleep, etc.)
    
    public Colonist stationedColonist;

    public float GetWorkStationEfiiciency()
    {
        if (stationedColonist == null)
            return 0;
        
        return stationedColonist.workEfficiency;
    }
}
