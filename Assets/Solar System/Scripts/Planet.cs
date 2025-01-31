using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float mass;
    public Vector3 acceleration;
    public Vector3 velocity;
    public Vector3 position;

    private void Start()
    {
        position = transform.position;
    }
}
