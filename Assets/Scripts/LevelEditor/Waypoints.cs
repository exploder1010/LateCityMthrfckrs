using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour {

    public List<Transform> Lane1Waypoints;
    public List<Transform> Lane2Waypoints;
    public List<Transform> Lane3Waypoints;
    public List<Transform> Lane4Waypoints;
    public List<Transform> Lane5Waypoints;
    public List<Transform> Lane6Waypoints;
    public List<Transform> Lane7Waypoints;
    public List<Transform> Lane8Waypoints;
    public List<Transform> Lane9Waypoints;
    public List<Transform> Lane10Waypoints;
    public List<Transform> Lane11Waypoints;
    public List<Transform> Lane12Waypoints;

    public List<List<Transform>> Lanes = new List<List<Transform>>();

    // Use this for initialization
    void Start () {
        Lanes.Add(Lane1Waypoints);
        Lanes.Add(Lane2Waypoints);
        Lanes.Add(Lane3Waypoints);
        Lanes.Add(Lane4Waypoints);
        Lanes.Add(Lane5Waypoints);
        Lanes.Add(Lane6Waypoints);
        Lanes.Add(Lane7Waypoints);
        Lanes.Add(Lane8Waypoints);
        Lanes.Add(Lane9Waypoints);
        Lanes.Add(Lane10Waypoints);
        Lanes.Add(Lane11Waypoints);
        Lanes.Add(Lane12Waypoints);

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
