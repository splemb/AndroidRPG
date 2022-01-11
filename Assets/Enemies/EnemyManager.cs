using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] List<Vector3> waypoints = new List<Vector3>();

    private void Start()
    {
        foreach (Transform point in GetComponentsInChildren<Transform>())
        {
            if (point.tag == "Waypoint") waypoints.Add(point.position);
        }
    }

    public Vector3 RequestDestination()
    {
        if (waypoints.Count == 0) return transform.position;
        int i = Random.Range(0, waypoints.Count);
        return waypoints[i];
    }

    public Vector3 RequestRespawn()
    {
        if (waypoints.Count == 0) return transform.position;

        Vector3 playerPos = GameObject.Find("Player").transform.position;

        Vector3 furthest = playerPos;

        foreach (Vector3 waypoint in waypoints)
        {
            if (Vector3.Distance(playerPos, waypoint) > Vector3.Distance(playerPos, furthest))
            {
                furthest = waypoint;
            }
        }

        return furthest;
    }
}
