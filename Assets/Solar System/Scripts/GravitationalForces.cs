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

    public float distMod;
    GameObject[] planets;

    float timeConv = 1/ 50f;

    Color defaultTrajectoryColor = Color.white;

    void Start()
    {
        planets = new GameObject[transform.childCount];

        // get all planets (children of this empty object
        int i = 0;
        foreach (Transform child in transform)
        {
            planets[i] = child.gameObject;

            // apply distance modifier (visual purposes)
            child.position = new Vector3(child.position.x * distMod, 0, 0);
            child.gameObject.GetComponent<Planet>().position = child.position;

            i++;
        }
    }

    void FixedUpdate()
    {
        DrawPredictedPaths(planets, 100, Time.fixedDeltaTime);
        UpdateCelestialBodies(planets, Time.fixedDeltaTime);
    }

    void UpdateCelestialBodies(GameObject[] planets, float deltaTime)
    {
        // apply the forces of all other planets on each planet
        foreach (GameObject i in planets)
        {
            Planet planet_i = i.GetComponent<Planet>();

            // add up all forces acting on planet i
            Vector3 force = Vector3.zero;
            foreach (GameObject j in planets)
            {
                if (i == j)
                    continue;

                Planet planet_j = j.GetComponent<Planet>();

                Vector3 planet_i_pos = planet_i.position / distMod;
                Vector3 planet_j_pos = planet_j.position / distMod;
                float massConstant = G * planet_i.mass * planet_j.mass;
                float dist_cubed = Mathf.Pow(Vector3.Distance(planet_j_pos, planet_i_pos), 3);
                force += massConstant * (planet_j_pos - planet_i_pos) / dist_cubed;
                planet_i.acceleration = force / planet_i.mass;
            }
        }

        foreach (GameObject i in planets)
        {
            // update planet i's position
            Planet planet_i = i.GetComponent<Planet>();
            planet_i.velocity += planet_i.acceleration * deltaTime * timeConv;
            planet_i.position += planet_i.velocity * deltaTime * timeConv;
            planet_i.transform.position = planet_i.position;
        }
    }

    void DrawPredictedPaths(GameObject[] planets, int stepCount, float deltaTime, Color[] colors = null)
    {
        GameObject[] predictedPlanets = new GameObject[planets.Length];
        planets.CopyTo(predictedPlanets, 0);

        // simulate steps ahead set by stepCount
        for (int step = 0; step < stepCount; step++)
        {
            UpdateCelestialBodies(predictedPlanets, deltaTime);
            
            // draw trajectory
            for (int i = 0; i < predictedPlanets.Length; i++)
            {
                Planet p = predictedPlanets[i].GetComponent<Planet>();
                Vector3 prevPos = p.position - p.velocity * deltaTime;

                if (colors == null)
                    DrawLine(prevPos, p.position, defaultTrajectoryColor);
                else
                    DrawLine(prevPos, p.position, colors[i]);
            }
        }
    }

    void DrawLine(Vector3 start, Vector3 end, Color color)// float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        //lr.material = new Material("Default Line");
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        //GameObject.Destroy(myLine, duration);
    }
}
