using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColonistAI
{
    // TODO functions that handle low level AI behavior and colonist behavior in general

    public static Building FindNewWorkplace(Colonist.JobType job)
    {
        switch (job)
        {
            case Colonist.JobType.Unemployed:
                break;

            case Colonist.JobType.Farmer:
                List<Building> farmList = ColonyResources.instance.GetBuildingListByType(Building.BuildingType.Farm);
                
                if (farmList.Count == 0)
                    break;

                foreach (Building bldg in farmList)
                {
                    if (bldg.OccupiedWorkStationCount() < bldg.workStations.Length)
                        return bldg;
                }
                break;

            case Colonist.JobType.Engineer:

                // TODO extraction and energy

                break;

            case Colonist.JobType.Medic:

                // TODO medical (none yet)

                break;

            case Colonist.JobType.Scientist:

                // TODO research/labs

                break;

            case Colonist.JobType.Chef:

                // TODO cafeteria

                break;
        }
        
        return null;
    }
}