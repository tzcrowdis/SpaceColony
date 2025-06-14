using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class BuildingManager : MonoBehaviour
{
    [Header("NavMesh Surface")]
    public NavMeshSurface navSurface;
    
    public static BuildingManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    private void Start()
    {
        UpdateActiveConnectionPoints();
    }

    public void UpdateActiveConnectionPoints()
    {
        ConnectionPoint[] allConnectionPoints = transform.GetComponentsInChildren<ConnectionPoint>();

        foreach (ConnectionPoint connection_i in allConnectionPoints)
        {
            // ignore if already disabled
            //if (!connection_i.GetComponent<Collider>().enabled)
            //    continue;

            // compare against all other connection points
            bool overlap = false;
            foreach (ConnectionPoint connection_j in allConnectionPoints)
            {
                // ignore i=i case
                if (connection_i == connection_j)
                    continue;

                // disable both if in same world position
                if (connection_i.transform.position == connection_j.transform.position)
                {
                    connection_i.GetComponent<Collider>().enabled = false;
                    connection_j.GetComponent<Collider>().enabled = false;
                    overlap = true;
                    break;
                }
            }

            // no overlap so enable
            if (!overlap)
                connection_i.GetComponent<Collider>().enabled = true;
        }
    }
}
