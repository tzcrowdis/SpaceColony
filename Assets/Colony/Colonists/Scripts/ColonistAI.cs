using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColonistAI
{
    // TODO functions that handle low level AI behavior and colonist behavior in general
    
    /*
     * WORKING
     */
    public static Building FindNewWorkplace(Colonist.JobType job) // NOTE may need to refactor as each job type has more sectors to choose from
    {
        switch (job)
        {
            case Colonist.JobType.Unemployed:
                // do nothing
                break;

            case Colonist.JobType.Farmer:
                // find farming building
                List<Building> farmList = ColonyResources.instance.GetBuildingListByType(Building.BuildingType.Farm);
                
                if (farmList.Count == 0)
                    break;

                foreach (Building bldg in farmList)
                    if (bldg.OccupiedWorkStationCount() < bldg.workStations.Length) return bldg;
                break;

            case Colonist.JobType.Engineer:
                // extraction and energy
                // goal is to go to sector with less workers
                List<Building> extractionList = ColonyResources.instance.GetBuildingListByType(Building.BuildingType.Extraction);
                List<Building> energyList = ColonyResources.instance.GetBuildingListByType(Building.BuildingType.Energy);
                List<Building> chosenList = null;

                // no buildings in colony of these types
                if (extractionList.Count == 0 & energyList.Count == 0)
                    break;

                // one sector is empty and other isn't
                if (energyList.Count == 0 & extractionList.Count > 0)
                    chosenList = extractionList;

                if (extractionList.Count == 0 & energyList.Count > 0)
                    chosenList = energyList;

                if (chosenList != null)
                {
                    foreach (Building bldg in chosenList)
                        if (bldg.OccupiedWorkStationCount() < bldg.workStations.Length) return bldg;
                }

                // both sectors available -> go with sector with less workers unless full
                int extractionWorkerCount = 0;
                foreach (Building bldg in extractionList)
                    extractionWorkerCount += bldg.OccupiedWorkStationCount();

                int energyWorkerCount = 0;
                foreach (Building bldg in energyList)
                    energyWorkerCount += bldg.OccupiedWorkStationCount();

                // less extraction workers
                if (extractionWorkerCount > 0 && extractionWorkerCount < energyWorkerCount)
                {
                    // find total number of available stations
                    int extractionWorkerCapacity = 0;
                    foreach (Building bldg in extractionList)
                        extractionWorkerCapacity += bldg.workStations.Length;

                    if (extractionWorkerCount / extractionWorkerCapacity < 1f)
                    {
                        foreach (Building bldg in extractionList)
                            if (bldg.OccupiedWorkStationCount() < bldg.workStations.Length) return bldg;
                    }
                    else
                    {
                        foreach (Building bldg in energyList)
                            if (bldg.OccupiedWorkStationCount() < bldg.workStations.Length) return bldg;
                    }
                }
                
                // less energy workers
                if (energyWorkerCount > 0 && energyWorkerCount < extractionWorkerCount)
                {
                    int energyWorkerCapacity = 0;
                    foreach (Building bldg in energyList)
                        energyWorkerCapacity += bldg.workStations.Length;

                    if (energyWorkerCount / energyWorkerCapacity < 1f)
                    {
                        foreach (Building bldg in energyList)
                            if (bldg.OccupiedWorkStationCount() < bldg.workStations.Length) return bldg;
                    }
                    else
                    {
                        foreach (Building bldg in extractionList)
                            if (bldg.OccupiedWorkStationCount() < bldg.workStations.Length) return bldg;
                    }
                }
                break;

            case Colonist.JobType.Medic:
                // find medical building
                List<Building> medicalList = ColonyResources.instance.GetBuildingListByType(Building.BuildingType.Medical);

                if (medicalList.Count == 0)
                    break;

                foreach (Building bldg in medicalList)
                    if (bldg.OccupiedWorkStationCount() < bldg.workStations.Length) return bldg;
                break;

            case Colonist.JobType.Scientist:
                // find research building
                List<Building> researchList = ColonyResources.instance.GetBuildingListByType(Building.BuildingType.Research);

                if (researchList.Count == 0)
                    break;

                foreach (Building bldg in researchList)
                    if (bldg.OccupiedWorkStationCount() < bldg.workStations.Length) return bldg;
                break;

            case Colonist.JobType.Cook:
                // find cafeteria building
                List<Building> cafeList = ColonyResources.instance.GetBuildingListByType(Building.BuildingType.Cafeteria);

                if (cafeList.Count == 0) break;

                foreach (Building bldg in cafeList)
                    if (bldg.OccupiedWorkStationCount() < bldg.workStations.Length) return bldg;
                break;
        }
        
        return null;
    }
}