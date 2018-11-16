using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiController : MonoBehaviour {

BasicVehicle vehicleScript;
	// Use this for initialization
	void Start () {
		vehicleScript = GetComponent<BasicVehicle>();
		vehicleScript.initializeSpeed(10,0,false);
	}
	
	// Update is called once per frame
	void Update () {
		if (vehicleScript != null && !vehicleScript.broken)
		{
			//input horizontal movement
			vehicleScript.inputHorz(0f);

			//input accelleration
			vehicleScript.inputAccel(.5f);
		}
	}
}
