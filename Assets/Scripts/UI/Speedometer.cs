using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour {

    public static float minAngle = -0.78f;
    public static float maxAngle = -271.465f;
    private static Speedometer thisSpeedometer;

	// Use this for initialization
	void Start () {
        thisSpeedometer = this;
	}
	
	// Update is called once per frame
    public static void ShowSpeed(float speed, float min, float max)
    {
        float ang = Mathf.Lerp(minAngle, maxAngle, Mathf.InverseLerp(min, max, speed));
        thisSpeedometer.transform.eulerAngles = new Vector3(0, 0, ang);
    }
}
