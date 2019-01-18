using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VehicleRigger : EditorWindow {

    public GameObject g;

    GameObject Body;
    string text = "Not Rigged";


    [MenuItem ("Window/Vehicle Rigger")]
    public static void ShowWindow ()
    {
        EditorWindow.GetWindow(typeof(VehicleRigger));
    }

	void OnGUI ()
    {
        if(GUILayout.Button("Body"))
        {
            Body = Selection.activeGameObject;
            Object baseCar = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Vehicles/BV_Muscle.prefab", typeof(GameObject));
            GameObject RiggedCar = Instantiate(baseCar, Body.transform.position, Body.transform.rotation) as GameObject;

            RiggedCar.GetComponentInChildren<MeshFilter>().mesh = Body.GetComponent<MeshFilter>().sharedMesh;
            text = Body.name;
        }
        GUILayout.Label(text);
        
    }
}
