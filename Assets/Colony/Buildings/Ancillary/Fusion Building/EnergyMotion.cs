using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyMotion : MonoBehaviour
{
    public GameObject loop;
    public float loopRotationSpeed;
    
    void Start()
    {
        
    }

    void Update()
    {
        loop.transform.Rotate(new Vector3(0, loopRotationSpeed, 0));
    }
}
