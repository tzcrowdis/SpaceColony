using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor.Experimental.GraphView;
using System.Linq;

public class Colonist : MonoBehaviour
{
    [Header("Colonist UI Elements")]
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

    // HEALTH/STATUS

    // TODO function that updates this
    public float health = 1f; // 0% dead / 100% healthy

    // TODO function that updates this
    public float sleep = 1f; // 0% needs to sleep / 100% well rested

    // TODO function that updates this
    public float hunger = 1f; // 0% needs to eat or lose health / 100% well fed

    // TODO consider status effects like sick, injured, etc.

    enum Emotion
    {
        Bored,
        Angry,
        Happy,
        Sad
        // etc.
    }
    Emotion emotion; // TODO getters/setters

    // NOTE: high level states
    public enum State
    {
        Navigate,
        Rest,
        Work,
        Sleep,
        Eat,
        Injured
        // Kill
        // Stalk
        // Sabotage
    }
    public State state;

    [Header("Navigate State")]
    public Transform navDestination;
    public State navNextState;

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
        job = JobType.Unemployed; // TODO load from save file
        if (job == JobType.Unemployed)
            ColonyResources.instance.unemployedColonists.Add(this);

        workplace = ColonistAI.FindNewWorkplace(job);

        state = State.Rest;

        gameObject.name = characterName;

        LoadOrGenerateProficiencies(); // TODO logic to Load or Generate these (and all other colonist attributes)
    }

    void Update()
    {
        switch (state)
        {
            case State.Navigate:
                Navigate();
                break;

            case State.Rest:
                Rest();
                break;

            case State.Work:
                Work();
                break;

            case State.Sleep:
                Sleep();
                break;

            case State.Eat:
                Eat();
                break;

            case State.Injured:
                Injured();
                break;
        }
    }

    /*
     * PLAYER INPUTS
     */
    public void ChangeColonistsJob(int value)
    {
        job = (JobType)value;

        if (job == JobType.Unemployed)
        {
            workplace.RemoveColonistFromWorkplace(this);
            workplace = null;
        }
        else
        {
            workplace = ColonistAI.FindNewWorkplace(job);
            workStation = workplace.GetEmptyWorkStation();
        }  
    }

    public void MakeSuggestion(Suggestion suggest)
    {
        suggestion = suggest;
    }

    void LoadOrGenerateProficiencies()
    {
        // TODO check for save file
        
        // generate proficiencies
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


    /*
     * STATES
     */

    void Navigate()
    {
        switch (navNextState)
        {
            case State.Rest:
                // TODO find rest location
                break;
            case State.Work:
                navDestination = workStation.transform;
                break;
            case State.Sleep:
                // TODO find bed
                break;
            case State.Eat:
                // TODO find cafeteria
                break;
            case State.Injured:
                // TODO find hospital
                break;

            default:
                navDestination = transform;
                break;
        }
        
        ColonistAI.ColonistAnimation("Walking", animator);

        if (ColonistAI.NavigateToDestination(navDestination, agent))
            state = navNextState;
    }

    void Rest()
    {
        // TODO go to recreation area then idle
        
        ColonistAI.ColonistAnimation("Idling", animator);

        // TODO
        // - sleep & _ health & - hunger

        state = ColonistAI.ExitRestState(this);
    }

    void Work()
    {
        if (!ColonistAI.AtDestination(workStation.transform, agent))
        {
            navNextState = State.Work;
            state = State.Navigate;
            return;
        }

        ColonistAI.ColonistAnimation("Working", animator);
        ColonistAI.UpdateWorkEfficiency(this);

        // TODO
        // -- sleep & _ health & -- hunger

        state = ColonistAI.ExitWorkState(this);
    }

    void Sleep()
    {
        // TODO
        // go to bed
        // play sleep animation
        // + sleep & + health & - hunger

        // TODO get sleep rate based on building
    }

    void Eat()
    {
        // TODO
        // go to cafeteria
        // play eat animation
        // - sleep & + health & + hunger

        // TODO get hunger rate based on building
    }

    void Injured()
    {
        // TODO
        // go to hospital
        // play recovery animation
        // _ sleep & + health & _ hunger

        // TODO get healing rate based on building
    }
}
