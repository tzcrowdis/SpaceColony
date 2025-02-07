using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionPoint : MonoBehaviour
{
    ColonyControls controls;
    
    void Start()
    {
        controls = GameObject.Find("Main Camera").GetComponent<ColonyControls>();
    }

    void OnMouseOver()
    {
        controls.connectionLocation = transform;
        controls.placeable = true;
    }

    // TODO on click destroy connection point???

    void OnMouseExit()
    {
        controls.connectionLocation = null;
        controls.placeable = false;
    }
}
