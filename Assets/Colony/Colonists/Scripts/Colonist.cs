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
    float health; // 0% dead / 100% healthy

    // TODO function that updates this
    float sleep; // 0% needs to sleep / 100% well rested

    // TODO function that updates this
    float hunger; // 0% needs to eat or lose health / 100% well fed

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
        Eat
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
        }
    }

    /*
     * PLAYER INPUTS
     */
    public void ChangeColonistsJob(int value)
    {
        job = (JobType)value;

        workplace = ColonistAI.FindNewWorkplace(job);
    }

    public void MakeSuggestion(Suggestion suggest)
    {
        suggestion = suggest;

        // TODO react to suggestions
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
        ColonistAI.ColonistAnimation("Walking", animator);

        if (ColonistAI.GoToDestination(navDestination, agent))
        {
            state = navNextState;
        }
    }

    void Rest()
    {
        ColonistAI.ColonistAnimation("Idling", animator);

        if (workStation != null) // TODO more complex exit rest condition
        {
            state = State.Navigate;
            navNextState = State.Work;
        }
    }

    void Work()
    {
        // TODO check if assigned building is in Idle or Operating (done?)

        ColonistAI.ColonistAnimation("Working", animator);

        if (workStation == null) // TODO go to recreation area or eat or sleep depending
            state = State.Rest;

        ColonistAI.UpdateWorkEfficiency(this);
    }

    void Sleep()
    {

    }

    void Eat()
    {

    }
}
