using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class ColonistAI
{
    // functions that handle low level AI behavior and colonist behavior in general

    /*
     * STATES
     */
    // TODO integrate suggestions!!!
    public static bool NavigateToDestination(Transform destination, NavMeshAgent agent) // true if at destination
    {
        if (agent.destination != destination.position)
            agent.destination = destination.position;

        // check if at destination
        if (!agent.pathPending & agent.remainingDistance < agent.stoppingDistance)
        {
            agent.transform.forward = destination.forward; // face direction of destination object
            return true;
        }

        return false;
    }

    public static bool AtDestination(Transform destination, NavMeshAgent agent)
    {
        return !agent.pathPending & Vector3.Distance(destination.position, agent.transform.position) < agent.stoppingDistance;
    }

    public static Colonist.State ExitStateBasicNeeds(Colonist colonist)
    {
        if (colonist.health < 0.1f)
            return Colonist.State.Injured;

        if (colonist.sleep < colonist.hunger && colonist.sleep < 0.1f)
            return Colonist.State.Sleep;

        if (colonist.hunger < colonist.sleep && colonist.hunger < 0.1f)
            return Colonist.State.Eat;

        return colonist.state;
    }

    public static Colonist.State ExitRestState(Colonist colonist)
    {
        Colonist.State state = ExitStateBasicNeeds(colonist);
        if (state != colonist.state)
            return state;

        if (colonist.job != Colonist.JobType.Unemployed & colonist.workStation != null)
            return Colonist.State.Work;

        return colonist.state;
    }

    public static Colonist.State ExitWorkState(Colonist colonist)
    {
        Colonist.State state = ExitStateBasicNeeds(colonist);
        if (state != colonist.state)
            return state;

        // TODO emotion exits

        if (colonist.workplace.state == Building.State.Idle)
            return Colonist.State.Rest;

        return colonist.state;
    }

    public static Colonist.State ExitSleepState(Colonist colonist)
    {
        if (colonist.hunger < 0.1f && colonist.sleep > 0.5f)
            return Colonist.State.Eat;

        if (colonist.sleep >= 1f)
        {
            if (colonist.hunger < 0.5f)
                return Colonist.State.Eat;

            return Colonist.State.Work;
        }

        return colonist.state;
    }

    public static Colonist.State ExitEatState(Colonist colonist)
    {
        if (colonist.hunger >= 1f)
        {
            if (colonist.sleep < 0.1f)
                return Colonist.State.Sleep;

            if (colonist.job != Colonist.JobType.Unemployed & colonist.workStation != null)
                return Colonist.State.Work;
            else
                return Colonist.State.Rest;
        }

        return colonist.state;
    }

    public static Colonist.State ExitInjuredState(Colonist colonist)
    {
        // TODO

        return colonist.state;
    }

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
                    if (bldg.OccupiedStationCount() < bldg.stations.Count) return bldg;
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
                        if (bldg.OccupiedStationCount() < bldg.stations.Count) return bldg;
                }

                // both sectors available -> go with sector with less workers unless full
                int extractionWorkerCount = 0;
                foreach (Building bldg in extractionList)
                    extractionWorkerCount += bldg.OccupiedStationCount();

                int energyWorkerCount = 0;
                foreach (Building bldg in energyList)
                    energyWorkerCount += bldg.OccupiedStationCount();

                // less extraction workers
                if (extractionWorkerCount > 0 && extractionWorkerCount < energyWorkerCount)
                {
                    // find total number of available stations
                    int extractionWorkerCapacity = 0;
                    foreach (Building bldg in extractionList)
                        extractionWorkerCapacity += bldg.stations.Count;

                    if (extractionWorkerCount / extractionWorkerCapacity < 1f)
                    {
                        foreach (Building bldg in extractionList)
                            if (bldg.OccupiedStationCount() < bldg.stations.Count) return bldg;
                    }
                    else
                    {
                        foreach (Building bldg in energyList)
                            if (bldg.OccupiedStationCount() < bldg.stations.Count) return bldg;
                    }
                }
                
                // less energy workers
                if (energyWorkerCount > 0 && energyWorkerCount < extractionWorkerCount)
                {
                    int energyWorkerCapacity = 0;
                    foreach (Building bldg in energyList)
                        energyWorkerCapacity += bldg.stations.Count;

                    if (energyWorkerCount / energyWorkerCapacity < 1f)
                    {
                        foreach (Building bldg in energyList)
                            if (bldg.OccupiedStationCount() < bldg.stations.Count) return bldg;
                    }
                    else
                    {
                        foreach (Building bldg in extractionList)
                            if (bldg.OccupiedStationCount() < bldg.stations.Count) return bldg;
                    }
                }
                break;

            case Colonist.JobType.Medic:
                // find medical building
                List<Building> medicalList = ColonyResources.instance.GetBuildingListByType(Building.BuildingType.Medical);

                if (medicalList.Count == 0)
                    break;

                foreach (Building bldg in medicalList)
                    if (bldg.OccupiedStationCount() < bldg.stations.Count) return bldg;
                break;

            case Colonist.JobType.Scientist:
                // find research building
                List<Building> researchList = ColonyResources.instance.GetBuildingListByType(Building.BuildingType.Research);

                if (researchList.Count == 0)
                    break;

                foreach (Building bldg in researchList)
                    if (bldg.OccupiedStationCount() < bldg.stations.Count) return bldg;
                break;

            case Colonist.JobType.Cook:
                // find cafeteria building
                List<Building> cafeList = ColonyResources.instance.GetBuildingListByType(Building.BuildingType.Cafeteria);

                if (cafeList.Count == 0) break;

                foreach (Building bldg in cafeList)
                    if (bldg.OccupiedStationCount() < bldg.stations.Count) return bldg;
                break;
        }
        
        return null;
    }

    public static void UpdateWorkEfficiency(Colonist colonist)
    {
        // TODO change based on skills/proficiencies/status

        if (colonist.state == Colonist.State.Work)
        {
            colonist.workEfficiency = 1f;
        }
        else
        {
            colonist.workEfficiency = 0f;
        }
    }

    /*
     * SLEEPING
     */
    public static Station FindBed(Colonist colonist)
    {
        List<Building> sleepBuildings = ColonyResources.instance.GetBuildingListByType(Building.BuildingType.Sleep);

        foreach (Building bldg in sleepBuildings)
        {
            if (bldg.OccupiedStationCount() < bldg.stations.Count)
                return bldg.GetEmptyStation(Station.StationType.Sleep);
        }

        return null;
    }

    /*
     * ANIMATIONS
     */
    public static void ColonistAnimation(string animationName, Animator animator)
    {
        if (!animator.GetBool(animationName))
        {
            foreach (AnimatorControllerParameter parameter in animator.parameters)
                animator.SetBool(parameter.name, false);
            animator.SetBool(animationName, true);
        }
    }
}