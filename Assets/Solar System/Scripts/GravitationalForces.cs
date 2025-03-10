using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static Unity.VisualScripting.Member;
using System;

public class GravitationalForces : MonoBehaviour
{
    // source: https://joebinns.com/documents/work_sample/n-body_simulator.pdf
    // helpful: https://en.wikipedia.org/wiki/N-body_problem
    // planetary fact sheet: https://nssdc.gsfc.nasa.gov/planetary/factsheet/
    // planetary mass source: https://en.wikipedia.org/wiki/Planetary_mass

    // gravitational constant
    // source (and M_s = 330,000 M_E) https://physics.stackexchange.com/questions/112461/astronomical-constant-in-astronomical-units
    //float G = 4 * Mathf.Pow(Mathf.PI, 2) / 330000; // AU^3 / (M_E * yr^2)
    [Tooltip("Should be small (< 0.1)")]
    public float G;

    Planet[] planetObjects;

    public int trajectorySteps;

    // struct for deep copy for trajectory
    struct PlanetAttr
    {
        public PlanetAttr(float m, Vector3 a, Vector3 v, Vector3 p)
        {
            mass = m;
            acceleration = a;
            velocity = v;
            position = p;
        }
        
        public float mass;
        public Vector3 acceleration;
        public Vector3 velocity;
        public Vector3 position;
    }
    PlanetAttr[] planets;

    PlanetAttr[] predictedPlanets; // memory structure to reduce compute on trajectories

    Color defaultTrajectoryColor = Color.white;


    void Start()
    {
        planets = new PlanetAttr[transform.childCount];
        planetObjects = new Planet[transform.childCount];

        // get all planets (children of this empty object)
        int i = 0;
        foreach (Transform child in transform)
        {
            Planet planet = child.GetComponent<Planet>();
            planetObjects[i] = planet;
            planets[i] = new PlanetAttr(planet.mass, planet.acceleration, planet.velocity, planet.transform.position);

            i++;
        }
    }

    void FixedUpdate()
    {
        DrawPredictedPaths(planets, trajectorySteps, Time.fixedDeltaTime);
        UpdateCelestialBodies(planets, Time.fixedDeltaTime, false);
    }

    void UpdateCelestialBodies(PlanetAttr[] planets, float deltaTime, bool drawing)
    {
        // apply the forces of all other planets on each planet
        for (int i = 0; i < planets.Length; i++)
        {
            // add up all forces acting on planet i
            Vector3 force = Vector3.zero;
            for (int j = 0; j < planets.Length; j++)
            {
                if (i == j)
                    continue;

                float massConstant = G * planets[i].mass * planets[j].mass;
                float dist_cubed = Mathf.Pow(Vector3.Distance(planets[j].position, planets[i].position), 3);
                force += massConstant * (planets[j].position - planets[i].position) / dist_cubed;
                planets[i].acceleration = force / planets[i].mass;
            }
        }

        // update each planets velocity and position
        for (int i = 0; i < planets.Length; i++)
        {
            planets[i].velocity += planets[i].acceleration * deltaTime;
            planets[i].position += planets[i].velocity * deltaTime;
            
            // dont update objects position if just drawing trajectory
            if (!drawing)
                planetObjects[i].SetMotionVariables(planets[i].acceleration, planets[i].velocity, planets[i].position);
        }
    }

    void DrawPredictedPaths(PlanetAttr[] planets, int stepCount, float deltaTime, Color[] colors = null)
    {
        if (predictedPlanets == null)
        {
            // create predicted planets object
            predictedPlanets = new PlanetAttr[planets.Length];
            planets.CopyTo(predictedPlanets, 0);

            // simulate steps ahead set by stepCount
            for (int step = 0; step < stepCount; step++)
            {
                UpdateCelestialBodies(predictedPlanets, deltaTime, true);

                // draw trajectory
                for (int i = 0; i < predictedPlanets.Length; i++)
                {
                    if (colors == null)
                        planetObjects[i].DrawTrajectory(step, stepCount, predictedPlanets[i].position, defaultTrajectoryColor);
                    else
                        planetObjects[i].DrawTrajectory(step, stepCount, predictedPlanets[i].position, colors[i]);
                }
            }
        }
        else
        {
            // calculate one prediction step
            UpdateCelestialBodies(predictedPlanets, deltaTime, true);

            // shift trajectory positions down by one and add next prediction step
            for (int i = 0; i< predictedPlanets.Length; i++)
            {
                LineRenderer trajectory = planetObjects[i].GetComponent<LineRenderer>();
                Vector3[] oldPoints = new Vector3[trajectory.positionCount];
                Vector3[] newPoints = new Vector3[trajectory.positionCount];

                trajectory.GetPositions(oldPoints);
                Array.Copy(oldPoints, 1, newPoints, 0, trajectory.positionCount - 1);
                newPoints[trajectory.positionCount - 1] = predictedPlanets[i].position;
                trajectory.SetPositions(newPoints);
            }
        }
    }
}
