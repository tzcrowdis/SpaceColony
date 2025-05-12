using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor.Experimental.GraphView;
using System.Linq;

public class Colonist : MonoBehaviour
{
    public string characterName; // becomes the object name on start
    public Sprite headshot;

    public GameObject colonistMenuPrefab;
    public bool requiresPlayerAttention;
    public ColonistListItem colonistListItem;

    public enum JobType
    {
        Unemployed,
        Farmer,
        Engineer,
        Medic,
        Scientist,
        Cook
    }
    public JobType job;

    public enum Suggestion
    {
        Work,
        Eat,
        Sleep,
        None
    }
    public Suggestion suggestion;

    public enum MentalStatus
    {
        Hungry,
        Bored,
        Sleepy
    }
    public MentalStatus mentalState;

    public enum Skill 
    {
        Farming,
        Engineering,
        Cooking,
        Medicine,
        Science
    }

    // developed over time with experience
    public Dictionary<Skill, int> skills = new Dictionary<Skill, int> // int here is a sum of values
    {
        {Skill.Farming, 0 },
        {Skill.Engineering, 0 },
        {Skill.Cooking, 0 },
        {Skill.Medicine, 0 },
        {Skill.Science, 0 },
        // etc.
    };
    
    // what they are naturally good or bad at
    public Dictionary<Skill, float> proficiencies = new Dictionary<Skill, float> // float here is a pos/neg percent modifier
    {
        {Skill.Farming, 0 },
        {Skill.Engineering, 0 },
        {Skill.Cooking, 0 },
        {Skill.Medicine, 0 },
        {Skill.Science, 0 },
        // etc.
    }; 


    // TODO vars beneath this comment subject to rework


    // HEALTH/STATUS
    // limbs
    GameObject head;
    GameObject armR;
    GameObject armL;
    GameObject legR;
    GameObject legL;

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
    public Building workplace;
    public WorkStation workStation;
    [HideInInspector]
    public float workEfficiency;


    // ADMINISTRATIVE
    [Header("Administrative")]
    public NavMeshAgent agent;
    public Animator animator;

    void Start()
    {
        job = JobType.Unemployed;
        
        if (job == JobType.Unemployed)
            ColonyResources.instance.unemployedColonists.Add(this);

        workplace = ColonistAI.FindNewWorkplace(job);

        state = State.Rest;
        workState = WorkState.GoingToWork;

        gameObject.name = characterName;

        GenerateProficiencies(); // TODO logic to Load or Generate these (and all other colonist attributes)
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

    
    public void ChangeColonistsJob(int value)
    {
        job = (JobType)value;

        workplace = ColonistAI.FindNewWorkplace(job);
    }

    public void MakeSuggestion(Suggestion suggest)
    {
        suggestion = suggest;
    }

    void GenerateProficiencies()
    {
        foreach (var key in proficiencies.Keys.ToList())
        {
            // options: -0.25, 0, 0.25 
            // respective chances: 0.25, 0.5, 0.25
            float chance = Random.Range(0f, 1f);
            if (chance <= 0.25f)
                proficiencies[key] = -0.25f;
            else if (chance <= 0.75f)
                proficiencies[key] = 0f;
            else
                proficiencies[key] = 0.25f;
        }
    }




    // TODO functions beneath this line subject to rework

    void Rest()
    {
        ColonistAnimation("Idling");

        if (workStation != null)
        {
            state = State.Work;
        }
    }

    /*
     *  WORK FUNCTIONS
     */

    void Work()
    {
        // TODO check if assigned building is in Idle or Operating
        
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

        UpdateWorkEfficiency();
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

    void UpdateWorkEfficiency()
    {
        if (state == State.Work && workState == WorkState.AtWork)
        {
            workEfficiency = 1f;
        }
        else
        {
            workEfficiency = 0f;
        }
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
