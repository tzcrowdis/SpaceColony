using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using UnityEngine.Rendering;

public class GravitationalForces : MonoBehaviour
{
    // source: https://joebinns.com/documents/work_sample/n-body_simulator.pdf
    // hlepful: https://en.wikipedia.org/wiki/N-body_problem
    // planetary fact sheet: https://nssdc.gsfc.nasa.gov/planetary/factsheet/
    // planetary mass source: https://en.wikipedia.org/wiki/Planetary_mass

    // gravitational constant
    // source (and M_s = 330,000 M_E) https://physics.stackexchange.com/questions/112461/astronomical-constant-in-astronomical-units
    float G = 4 * Mathf.Pow(Mathf.PI, 2) / 330000; // AU^3 / (M_E * yr^2)
    float timeConv = 1 / 50f;
    public float distMod;

    Planet[] planetObjects;

    int trajectorySteps = 3000;

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

    Color defaultTrajectoryColor = Color.white;

    void Start()
    {
        planets = new PlanetAttr[transform.childCount];
        planetObjects = new Planet[transform.childCount];

        // get all planets (children of this empty object
        int i = 0;
        foreach (Transform child in transform)
        {
            Planet planet = child.GetComponent<Planet>();
            planetObjects[i] = planet;
            planets[i] = new PlanetAttr(planet.mass, planet.acceleration, planet.velocity, planet.position);

            // apply distance modifier (visual purposes)
            planets[i].position = new Vector3(child.position.x * distMod, 0, 0);

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

                Vector3 planet_i_pos = planets[i].position / distMod;
                Vector3 planet_j_pos = planets[j].position / distMod;
                float massConstant = G * planets[i].mass * planets[j].mass;
                float dist_cubed = Mathf.Pow(Vector3.Distance(planet_j_pos, planet_i_pos), 3);
                force += massConstant * (planet_j_pos - planet_i_pos) / dist_cubed;
                planets[i].acceleration = force / planets[i].mass;
            }
        }

        for (int i = 0; i < planets.Length; i++)
        {
            // update planet i's position
            planets[i].velocity += planets[i].acceleration * deltaTime * timeConv;
            planets[i].position += planets[i].velocity * deltaTime * timeConv;
            
            if (!drawing)
                planetObjects[i].SetMotionVariables(planets[i].acceleration, planets[i].velocity, planets[i].position);
        }
    }

    void DrawPredictedPaths(PlanetAttr[] planets, int stepCount, float deltaTime, Color[] colors = null)
    {
        PlanetAttr[] predictedPlanets = new PlanetAttr[planets.Length];
        planets.CopyTo(predictedPlanets, 0);

        // simulate steps ahead set by stepCount
        for (int step = 0; step < stepCount; step++)
        {
            UpdateCelestialBodies(predictedPlanets, deltaTime, true);
            
            // draw trajectory
            for (int i = 0; i < predictedPlanets.Length; i++)
            {
                //Planet p = predictedPlanets[i].GetComponent<Planet>();
                //Vector3 prevPos = predictedPlanets[i].position - predictedPlanets[i].velocity * deltaTime;

                if (colors == null)
                    planetObjects[i].DrawTrajectory(step, stepCount, predictedPlanets[i].position, defaultTrajectoryColor);
                else
                    planetObjects[i].DrawTrajectory(step, stepCount, predictedPlanets[i].position, colors[i]);
            }
        }

        foreach (Planet planet in planetObjects)
        {
            planet.GetComponent<LineRenderer>().Simplify(0.01f);
        }
    }
}
