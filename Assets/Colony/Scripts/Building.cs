using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class Building : MonoBehaviour
{
    bool selected;

    void Start()
    {
        
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider connection)
    {
        Vector3 connectionOffset = connection.bounds.center;
        connection.transform.position = transform.position + connectionOffset;
    }
}
