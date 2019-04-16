using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VehicleShuffler : EditorWindow {
	string text = "Not Clicked";

    [MenuItem ("Window/Vehicle Shuffler")]
    public static void ShowWindow ()
    {
        EditorWindow.GetWindow(typeof(VehicleShuffler));
    }

	void OnGUI ()
    {
		
		if (GUILayout.Button("Shuffle Vehicle Types"))
		{
			List<CarSpawner> spawners = new List<CarSpawner>();

			#region Load Prefabs
			Object[] cars = Resources.LoadAll("StandardCars");
			#endregion

			#region Find Spawners
			GameObject[] objects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
			foreach(GameObject o in objects)
			{
				if(o.GetComponent<CarSpawner>() != null)
				{
					spawners.Add(o.GetComponent<CarSpawner>());
				}
			}
			#endregion

			#region Instantiate Prefabs
			foreach(CarSpawner cs in spawners)
			{
				Undo.RegisterCompleteObjectUndo(cs, "Shuffle Cars");
				for(int i=0; i<cs.CarToSpawn.Length; i++)
				{
					Object selectedCar = cars[Random.Range(0, cars.Length)];
					GameObject currentCar = Instantiate(selectedCar, Vector3.zero, Quaternion.identity) as GameObject;
					Undo.RegisterCreatedObjectUndo(currentCar, "Shuffle Cars");
					CopyCarFields(cs.CarToSpawn[i], currentCar);
					
					Undo.DestroyObjectImmediate(cs.CarToSpawn[i]);
					cs.CarToSpawn[i] = currentCar;
				}
			}
			#endregion

			text = spawners.Count.ToString();
		}

		GUILayout.Label(text);
	}

	void CopyCarFields(GameObject SourceCar, GameObject TargetCar)
	{
		TargetCar.name = SourceCar.name;

		#region Copy Transform
		TargetCar.transform.parent = SourceCar.transform.parent;
		TargetCar.transform.position = SourceCar.transform.position;
		TargetCar.transform.rotation = SourceCar.transform.rotation;
		TargetCar.transform.localScale = SourceCar.transform.localScale;
		#endregion

		#region  Copy RigidBody
		Rigidbody src = SourceCar.GetComponent<Rigidbody>();
		Rigidbody targ = TargetCar.GetComponent<Rigidbody>();
		targ.mass = src.mass;
		targ.drag = src.drag;
		targ.angularDrag = src.angularDrag;
		#endregion

		#region  Copy AiController
		AiController Ai_src = SourceCar.GetComponent<AiController>();
		AiController Ai_targ = TargetCar.GetComponent<AiController>();
		Ai_targ.disabled = Ai_src.disabled;
		Ai_targ.laneID = Ai_src.laneID;
		Ai_targ.DebugThis = Ai_src.DebugThis;
		Ai_targ.stopUpdate = Ai_src.stopUpdate;
		#endregion
		
		#region  Copy Basic Vehicle
		BasicVehicle BV_src = SourceCar.GetComponent<BasicVehicle>();
		BasicVehicle BV_targ = TargetCar.GetComponent<BasicVehicle>();
		BV_targ.DebugThis = BV_src.DebugThis;
		BV_targ.player = BV_src.player;
		BV_targ.broken = BV_src.broken;
		BV_targ.disabled = BV_src.disabled;
		BV_targ.breakInDistance = BV_src.breakInDistance;
		BV_targ.MotorTorque = BV_src.MotorTorque;
		BV_targ.MaxSteeringAngle = BV_src.MaxSteeringAngle;
		BV_targ.SteeringRate = BV_src.SteeringRate;
		BV_targ.GroundedStablizationRate = BV_src.GroundedStablizationRate;
		BV_targ.gravityDirection = BV_src.gravityDirection;
		BV_targ.normalMaxSpeed = BV_src.normalMaxSpeed;
		BV_targ.computerMaxSpeed = BV_src.computerMaxSpeed;
		BV_targ.tempMaxSpeed = BV_src.tempMaxSpeed;
		BV_targ.tempMaxSpeedLowerLimitPercent = BV_src.tempMaxSpeedLowerLimitPercent;
		BV_targ.potentialMaxSpeed = BV_src.potentialMaxSpeed;
		BV_targ.crashSpeed = BV_src.crashSpeed;
		BV_targ.boosting = BV_src.boosting;
		BV_targ.AutoCorrect = BV_src.AutoCorrect;
		#endregion
	}
}