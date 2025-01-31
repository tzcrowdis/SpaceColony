using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float mass;
    public Vector3 acceleration;
    public Vector3 velocity;
    public Vector3 position;

    LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        //lr.Simplify(100);
    }

    void FixedUpdate()
    {
        transform.position = position;
        //lr.Simplify(100);
    }

    public void SetMotionVariables(Vector3 a, Vector3 v, Vector3 p)
    {
        acceleration = a;
        velocity = v;
        position = p;
    }

    public void DrawTrajectory(int index, int length, Vector3 point, Color color)// float duration = 0.2f)
    {
        lr.positionCount = length;
        
        lr.startColor = color;
        lr.endColor = color;

        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;

        lr.SetPosition(index, point);

        //lr.Simplify(1);
    }
}
