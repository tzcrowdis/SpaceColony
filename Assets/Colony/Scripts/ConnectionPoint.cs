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
    }

    void OnMouseExit()
    {
        controls.connectionLocation = null;
    }
}
