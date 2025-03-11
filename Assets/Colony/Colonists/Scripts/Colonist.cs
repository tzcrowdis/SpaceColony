using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Colonist : MonoBehaviour
{
    public string characterName; // becomes the object name on start
    
    // HEALTH/STATUS
    // limbs
    GameObject head;
    GameObject armR;
    GameObject armL;
    GameObject legR;
    GameObject legL;

    // mental status
    enum MentalStatus
    {
        Hungry,
        Bored,
        Sleepy
    }

    // NOTE: high level states
    enum State
    {
        Rest,
        Work
    }
    State state;


    // WORKING (rework later)
    enum WorkState
    {
        GoingToWork,
        AtWork
    }
    WorkState workState;

    [Header("Occupation")]
    public GameObject workStation;


    // ADMINISTRATIVE
    [Header("Administrative")]
    public NavMeshAgent agent;
    public Animator animator;

    void Start()
    {
        if (workStation == null)
            ColonyResources.instance.unemployedColonists.Add(this);

        state = State.Rest;
        workState = WorkState.GoingToWork;

        gameObject.name = characterName;
    }

    void Update()
    {
        switch (state)
        {
            case State.Rest:
                Rest();
                break;

            case State.Work:
                Work();
                break;
        }
    }

    void Rest()
    {
        ColonistAnimation("Idling");

        if (workStation != null)
        {
            state = State.Work;
        }
    }

    void Work()
    {
        // TODO check which substate?
        
        switch (workState)
        {
            case WorkState.GoingToWork:
                GoingToWork();
                break;

            case WorkState.AtWork:
                AtWork();
                break;
        }
    }

    void GoingToWork()
    {
        ColonistAnimation("Walking");
        
        agent.destination = workStation.transform.position;

        // if at work station
        if (!agent.pathPending & agent.remainingDistance < agent.stoppingDistance)
        {
            // face direction of work station
            transform.forward = workStation.transform.forward;

            workState = WorkState.AtWork;
        }
    }

    void AtWork()
    {
        ColonistAnimation("Working");
        
        if (workStation == null)
            state = State.Rest;
    }


    /*
     * HELPER FUNCTIONS
     */

    void ColonistAnimation(string animationName)
    {
        if (!animator.GetBool(animationName))
        {
            foreach (AnimatorControllerParameter parameter in animator.parameters)
                animator.SetBool(parameter.name, false);
            animator.SetBool(animationName, true);
        }
    }
}
