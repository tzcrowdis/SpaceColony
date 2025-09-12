# SpaceColony
 
### WORK IN PROGRESS

Long term goal is to develop this into a colony sim where you manage unstable aliens. 
Grow the ship, terrorize the inhabitants of the planets you orbit, keep your colony alive, etc.

## Code Highlight
For the Colonist's I've set up a static helper class to handle bulky lower level AI code.
The aim is to make reasoning about the colonist's behavior simpler from the colonist class.

[Colonist Class](Assets/Colony/Colonists/Scripts/Colonist.cs) and
[ColonistAI Static Class](Assets/Colony/Colonists/Scripts/ColonistAI.cs)

For example this function in the Colonist class handles working:
```csharp
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

    // -- sleep & _ health & -- hunger
    UpdateHealth(0 * workDeltaModifier);
    UpdateSleep(-sleepDelta * workDeltaModifier);
    UpdateHunger(-hungerDelta * workDeltaModifier);

    state = ColonistAI.ExitWorkState(this);
}
```

Without the ColonistAI class all of these functions would be cluttering the already large Colonist script:
```csharp
public static bool AtDestination(Transform destination, NavMeshAgent agent)
{
    return !agent.pathPending & Vector3.Distance(destination.position, agent.transform.position) < agent.stoppingDistance;
}
```
```csharp
public static void ColonistAnimation(string animationName, Animator animator)
{
    if (!animator.GetBool(animationName))
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
            animator.SetBool(parameter.name, false);
        animator.SetBool(animationName, true);
    }
}
```
```csharp
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
```
```csharp
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
```

The work on the colonists is far from done, as you can see from all of the TODO's.
I will be moving more bulky code from Colonist to ColonistAI as I progress.
The "FindNewWorkplace" function in ColonistAI probably illustrates this point better.
