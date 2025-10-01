//----------------------------------------------
//			Burnout Racers
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Used for holding a list for waypoints, and drawing gizmos for all of them. AI racers will be using this path. Also this will be used for race positions.
/// </summary>
public class BR_AIWaypointsContainer : MonoBehaviour {

    /// <summary>
    /// All waypoints.
    /// </summary>
    public List<BR_Waypoint> waypoints = new List<BR_Waypoint>();

    private void Awake() {

        //  Getting waypoints and adding them to the list.
        GetAllWaypoints();

    }

    /// <summary>
    /// Getting waypoints and adding them to the list.
    /// </summary>
    public void GetAllWaypoints() {

        if (waypoints == null)
            waypoints = new List<BR_Waypoint>();

        waypoints.Clear();

        waypoints = GetComponentsInChildren<BR_Waypoint>(true).ToList();

    }

    /// <summary>
    /// Used for drawing gizmos on Editor.
    /// </summary>
    private void OnDrawGizmos() {

        //  If waypoints list is null, return.
        if (waypoints == null)
            return;

        //  Counting all waypoints.
        for (int i = 0; i < waypoints.Count; i++) {

            //  If current waypoint is not null, continue.
            if (waypoints[i] != null && waypoints[i].gameObject.activeSelf) {

                //  Drawing gizmos.
                Gizmos.color = new Color(0.0f, 1.0f, 1.0f, 0.3f);
                Gizmos.DrawSphere(waypoints[i].transform.position, 2);
                Gizmos.DrawWireSphere(waypoints[i].transform.position, waypoints[i].radius);

                //  If current waypoint is not last waypoint...
                if (i < waypoints.Count - 1) {

                    //  if current waypoint has next waypoint...
                    if (waypoints[i] && waypoints[i + 1]) {

                        Gizmos.color = Color.green;

                        if (i < waypoints.Count - 1)
                            Gizmos.DrawLine(waypoints[i].transform.position, waypoints[i + 1].transform.position);
                        if (i < waypoints.Count - 2)
                            Gizmos.DrawLine(waypoints[waypoints.Count - 1].transform.position, waypoints[0].transform.position);

                    }

                }

            }

        }

    }

}
