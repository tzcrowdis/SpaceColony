using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class Colonist : MonoBehaviour
{
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
    [Header("Status'")]
    public float health = 1f; // 0% dead / 100% healthy
    public float healthDelta = 0.1f;

    public float sleep = 1f; // 0% needs to sleep / 100% well rested
    public float sleepDelta = 0.01f;

    public float hunger = 1f; // 0% needs to eat or lose health / 100% well fed
    public float hungerDelta = 0.02f;

    [Header("Status Delta Modifiers")]
    public float workDeltaModifier = 2f;
    public float sleepDeltaModifier = 0.5f;
    public float eatDeltaModifier = 3f;
    public float idleDeltaModifier = 1f;

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

    [Header("Work State")]
    public Building workplace;
    public Station workStation;
    [HideInInspector]
    public float workEfficiency;

    [Header("Sleep State")]
    public Station colonistsBed;

    [Header("Eat State")]
    public Station eatingStation;

    [Header("Colonist UI Elements")]
    public string characterName; // becomes the object name on start
    public Sprite headshot;
    public GameObject colonistMenuPrefab;
    public Transform colonistMenuParent;
    public bool requiresPlayerAttention;
    public ColonistListItem colonistListItem;

    // ADMINISTRATIVE
    [Header("Administrative")]
    public NavMeshAgent agent;
    public Animator animator;
    public Collider clickCollider;


    void Start()
    {
        job = JobType.Unemployed; // TODO load from save file
        if (job == JobType.Unemployed)
            ColonyResources.instance.unemployedColonists.Add(this);

        workplace = ColonistAI.FindNewWorkplace(job);

        state = State.Rest;

        gameObject.name = characterName;

        LoadOrGenerateProficiencies(); // TODO logic to Load or Generate these (and all other colonist attributes)

        colonistMenuParent = GameObject.Find("Colonist Info Menu Canvas").transform;
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
            if (workplace != null)
                workplace.RemoveColonistFromWorkplace(this);
            workplace = null;
        }
        else
        {
            workplace = ColonistAI.FindNewWorkplace(job);
            if (workplace != null)
            {
                workStation = workplace.GetEmptyStation(Station.StationType.Work);
                workStation.stationedColonist = this;
            }  
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
     * STATUS'
     */
    void UpdateHealth(float delta)
    {
        health += delta * Time.deltaTime;
        health = Mathf.Clamp(health, 0f, 1f);
    }

    void UpdateSleep(float delta)
    {
        sleep += delta * Time.deltaTime;
        sleep = Mathf.Clamp(sleep, 0f, 1f);
    }

    void UpdateHunger(float delta)
    {
        hunger += delta * Time.deltaTime;
        hunger = Mathf.Clamp(hunger, 0f, 1f);

        if (delta > 0f & hunger != 1f)
            ColonyResources.instance.ConsumeResource(ColonyResources.ResourceTypes.Food, delta * Time.deltaTime);
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
                navDestination = colonistsBed.transform;
                break;
            case State.Eat:
                navDestination = eatingStation.transform;
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

        // - sleep & _ health & - hunger
        UpdateHealth(0 * idleDeltaModifier);
        UpdateSleep(-sleepDelta * idleDeltaModifier);
        UpdateHunger(-hungerDelta * idleDeltaModifier);

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
        UpdateHealth(0 * workDeltaModifier);
        UpdateSleep(-sleepDelta * workDeltaModifier);
        UpdateHunger(-hungerDelta * workDeltaModifier);

        state = ColonistAI.ExitWorkState(this);
    }

    void Sleep()
    {
        // go to bed
        // play sleep animation
        // + sleep & + health & - hunger

        if (colonistsBed == null)
        {
            colonistsBed = ColonistAI.FindBed(this);

            if (colonistsBed == null)
            {
                Debug.Log("couldnt find bed for colonist");
                // TODO player alerts

                state = State.Rest;
                return;
            }
        }

        if (!ColonistAI.AtDestination(colonistsBed.transform, agent))
        {
            navNextState = State.Sleep;
            state = State.Navigate;
            return;
        }

        ColonistAI.ColonistAnimation("Idling", animator);

        // TODO get sleep rate based on building

        UpdateHealth(healthDelta * sleepDeltaModifier);
        UpdateSleep(sleepDelta * sleepDeltaModifier);
        UpdateHunger(-hungerDelta * sleepDeltaModifier);

        state = ColonistAI.ExitSleepState(this);
        if (state != State.Sleep)
            colonistsBed = null;
    }

    void Eat()
    {
        // go to cafeteria
        // play eat animation
        // - sleep & + health & + hunger

        if (eatingStation == null)
        {
            eatingStation = ColonistAI.FindEatingStation(this);

            if (eatingStation == null)
            {
                Debug.Log("couldnt find eating spot for colonist");
                // TODO player alerts

                state = State.Rest;
                return;
            }
        }

        if (!ColonistAI.AtDestination(eatingStation.transform, agent))
        {
            navNextState = State.Eat;
            state = State.Navigate;
            return;
        }

        ColonistAI.ColonistAnimation("Idling", animator);

        // TODO get hunger rate based on building

        UpdateHealth(healthDelta * eatDeltaModifier);
        UpdateSleep(-sleepDelta * eatDeltaModifier);
        UpdateHunger(hungerDelta * eatDeltaModifier);

        state = ColonistAI.ExitEatState(this);
        if (state != State.Eat)
            eatingStation = null;
    }

    void Injured()
    {
        // TODO
        // go to hospital
        // play recovery animation
        // _ sleep & + health & _ hunger

        // TODO get healing rate based on building
    }

    /*
     * CLICK FOR INFO MENU
     */
    private void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            // track mouse
            Mouse mouse = Mouse.current;
            Vector3 mousePosition = mouse.position.ReadValue();

            // display info menu on click
            if (mouse.leftButton.wasPressedThisFrame)
            {
                // makes sure building menu isn't already open
                ColonistInfoMenu[] menus = colonistMenuParent.GetComponentsInChildren<ColonistInfoMenu>();
                foreach (ColonistInfoMenu clnstMenu in menus)
                    if (clnstMenu.colonist == this) return;

                // otherwise open menu
                GameObject menu = Instantiate(colonistMenuPrefab, colonistMenuParent);
                menu.GetComponent<ColonistInfoMenu>().colonist = this;
                if (menus.Length > 0)
                    menu.transform.position = menus[menus.Length - 1].transform.position + new Vector3(25f, -25f, 0);
            }
        }
    }
}
