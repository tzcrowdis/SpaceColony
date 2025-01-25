using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalForces : MonoBehaviour
{
    // source: https://joebinns.com/documents/work_sample/n-body_simulator.pdf
    // hlepful: https://en.wikipedia.org/wiki/N-body_problem

    // planetary mass source: https://en.wikipedia.org/wiki/Planetary_mass

    // gravitational constant
    // source (and M_s = 330,000 M_E) https://physics.stackexchange.com/questions/112461/astronomical-constant-in-astronomical-units
    float G = 4 * Mathf.Pow(Mathf.PI, 2) / 330000; // AU^3 / (M_E * yr^2)

    public float distMod;
    List <GameObject> planets = new List<GameObject>();

    // planetary fact sheet: https://nssdc.gsfc.nasa.gov/planetary/factsheet/

    float timeConv = 1/ 50f;

    void Start()
    {
        // get all planets (children of this empty object
        foreach (Transform child in transform)
        {
            planets.Add(child.gameObject);

            // apply distance modifier (visual purposes)
            child.position = new Vector3(child.position.x * distMod, 0, 0);
        }
    }

    void FixedUpdate()
    {
        // apply the forces of all other planets on each planet
        foreach (GameObject i in planets)
        {
            Planet planet_i = i.GetComponent<Planet>();

            // add up all forces acting on planet i
            Vector3 force_i = Vector3.zero;
            foreach (GameObject j in planets)
            {
                if (i == j)
                    continue;

                Planet planet_j = j.GetComponent<Planet>();

                Vector3 planet_i_pos = planet_i.transform.position / distMod;
                Vector3 planet_j_pos = planet_j.transform.position / distMod;
                float massConstant = G * planet_i.mass * planet_j.mass;
                float dist_cubed = Mathf.Pow(Vector3.Distance(planet_j_pos, planet_i_pos), 3);
                force_i += massConstant * (planet_j_pos - planet_i_pos) / dist_cubed;
            }

            // update planet i's position
            Vector3 acceleration_i = force_i / planet_i.mass;
            planet_i.velocity += acceleration_i * Time.fixedDeltaTime * timeConv;
            planet_i.transform.position += planet_i.velocity * Time.fixedDeltaTime * timeConv;
        }
    }
}
