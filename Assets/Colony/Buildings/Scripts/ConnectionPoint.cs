using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    public Building building;

    void OnMouseOver()
    {
        ColonyControls.instance.connectionLocation = transform;
    }

    void OnMouseExit()
    {
        ColonyControls.instance.connectionLocation = null;
        ColonyControls.instance.overValidConnectionPoint = false;
    }
}
