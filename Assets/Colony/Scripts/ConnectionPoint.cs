using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    void OnMouseOver()
    {
        ColonyControls.instance.connectionLocation = transform;
        ColonyControls.instance.overConnectionPoint = true;
    }

    // TODO on click destroy connection point???

    void OnMouseExit()
    {
        ColonyControls.instance.connectionLocation = null;
        ColonyControls.instance.overConnectionPoint = false;
    }
}
