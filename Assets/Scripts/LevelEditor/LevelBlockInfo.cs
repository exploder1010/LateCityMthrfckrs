using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBlockInfo : MonoBehaviour {

    public Vector3 VehicleCameraOffset = new Vector3(0,8, -20);
    public Vector3 VehicleCameraEulerAngles = new Vector3(25,0,0);

    public Vector3 RiderCameraOffset = new Vector3(0, 6, -7);
    public Vector3 RiderCameraEulerAngles = new Vector3(35, 0, 0);

    public float CameraFollowSpeed = 30f; //move
    public float CameraTrackSpeed = 30f; //rotate

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
