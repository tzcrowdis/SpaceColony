using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalForces : MonoBehaviour
{
    // source: https://en.wikipedia.org/wiki/N-body_problem

    // planetary mass source: https://en.wikipedia.org/wiki/Planetary_mass

    // gravitational constant
    // source (and M_s = 330,000 M_E) https://physics.stackexchange.com/questions/112461/astronomical-constant-in-astronomical-units
    float G = 4 * Mathf.Pow(Mathf.PI, 2) / 330000; // AU^3 / (M_E * yr^2)

    public float distMod;
    List <GameObject> planets = new List<GameObject>();

    // planetary fact sheet: https://nssdc.gsfc.nasa.gov/planetary/factsheet/

    void Start()
    {
        // get all planets (children of this empty object
        foreach (Transform child in transform)
        {
            planets.Add(child.gameObject);

            // apply distance modifier (visual purposes)
            child.position = new Vector3(child.position.x * distMod, 0, 0);

            // apply starting velocity (AU/yr)
            child.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0.1f);
        }
    }

    void Update()
    {
        // apply the forces of all other planets on each planet
        foreach (GameObject planet_i in planets)
        {
            Rigidbody planet_i_rb = planet_i.GetComponent<Rigidbody>();

            foreach (GameObject planet_j in planets)
            {
                if (planet_i == planet_j)
                    continue;

                Rigidbody planet_j_rb = planet_j.GetComponent<Rigidbody>();

                float constant = G * planet_i_rb.mass * planet_j_rb.mass;
                float dist_cubed = Mathf.Pow(Vector3.Distance(planet_j.transform.position / distMod, planet_i.transform.position / distMod), 3);
                Vector3 force = constant * (planet_j.transform.position / distMod - planet_i.transform.position / distMod) / dist_cubed;

                planet_i_rb.AddForce(force, ForceMode.Force);
            }
        }
    }
}
