using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class VehicleRigger : EditorWindow {

    public GameObject g;

    public Object CarMesh;
    public Object Root;
    public bool IsFrontWheel;
    string text = "Not Rigged";


    [MenuItem ("Window/Vehicle Rigger")]
    public static void ShowWindow ()
    {
        EditorWindow.GetWindow(typeof(VehicleRigger));
    }

	void OnGUI ()
    {
        CarMesh = EditorGUILayout.ObjectField("CarMesh", CarMesh, typeof(GameObject), true);
        Root = EditorGUILayout.ObjectField("Root", Root, typeof(GameObject), true);
        IsFrontWheel = EditorGUILayout.Toggle("Front Wheel", IsFrontWheel);

        if (GUILayout.Button("Rig Body"))
        {
            CarMesh = Selection.activeGameObject;
            Object baseCar = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Vehicles/PrefabPieces/VehicleRoot.prefab", typeof(GameObject));

            Root = Instantiate(baseCar, (CarMesh as GameObject).transform.position, (CarMesh as GameObject).transform.rotation);
            (CarMesh as GameObject).transform.parent = (Root as GameObject).transform;

        }
        if (GUILayout.Button("Rig Wheel"))
        {
            GameObject WheelMesh = Selection.activeGameObject;

            // Front Wheel
            if (IsFrontWheel)
            {
                // Create Wheel Collider
                Object Wheel = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Vehicles/PrefabPieces/FrontWheel.prefab", typeof(GameObject));
                GameObject wheelCollider = Instantiate(Wheel, WheelMesh.transform.position, WheelMesh.transform.rotation) as GameObject;

                // Set parent/child relationships in hierarchy
                wheelCollider.transform.parent = WheelMesh.transform.parent;
                WheelMesh.transform.parent = wheelCollider.transform;


                AxleInfo ax = (Root as GameObject).GetComponent<BasicVehicle>().axleInfos[0];
                if (ax.leftWheel == null)
                    ax.leftWheel = wheelCollider.GetComponent<WheelCollider>();
                else
                    ax.rightWheel = wheelCollider.GetComponent<WheelCollider>();

            }

            // Back Wheel
            else
            {
                // Create Wheel Collider
                Object Wheel = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Vehicles/PrefabPieces/RearWheel.prefab", typeof(GameObject));
                GameObject wheelCollider = Instantiate(Wheel, WheelMesh.transform.position, WheelMesh.transform.rotation) as GameObject;

                // Set parent/child relationships in hierarchy
                wheelCollider.transform.parent = WheelMesh.transform.parent;
                WheelMesh.transform.parent = wheelCollider.transform;


                AxleInfo ax = (Root as GameObject).GetComponent<BasicVehicle>().axleInfos[1];
                if (ax.leftWheel == null)
                    ax.leftWheel = wheelCollider.GetComponent<WheelCollider>();
                else
                    ax.rightWheel = wheelCollider.GetComponent<WheelCollider>();
            }



            }


            GUILayout.Label(text);
        
    }
}
