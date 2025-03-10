using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Colonist : MonoBehaviour
{
    public string characterName;
    
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
        Resting,
        Working
    }
    State state;


    // WORKING (rework later)
    enum WorkState
    {
        GoingToWork,
        Working
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

        state = State.Resting;
        workState = WorkState.GoingToWork;
    }

    void Update()
    {
        switch (state)
        {
            case State.Resting:
                Idle();
                break;

            case State.Working:
                Working();
                break;
        }
    }

    void Idle()
    {
        if (workStation != null)
        {
            // exit idle
            state = State.Working;

            // activate walking animation
            foreach (AnimatorControllerParameter parameter in animator.parameters)
                animator.SetBool(parameter.name, false);
            animator.SetBool("Walking", true);
        }
    }

    void Working()
    {
        switch (workState)
        {
            case WorkState.GoingToWork:

                agent.destination = workStation.transform.position;

                // if at work station
                if (!agent.pathPending & agent.remainingDistance < agent.stoppingDistance)
                {
                    // face direction of work station
                    transform.forward = workStation.transform.forward;

                    // activate working animation
                    foreach (AnimatorControllerParameter parameter in animator.parameters)
                        animator.SetBool(parameter.name, false);
                    animator.SetBool("Working", true);

                    workState = WorkState.Working;
                }

                break;

            case WorkState.Working:
                break;
        }
    }
}
