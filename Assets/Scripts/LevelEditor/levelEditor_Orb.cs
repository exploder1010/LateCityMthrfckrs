using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
public class levelEditor_Orb : MonoBehaviour {

    public string choice;
    public string[] roadPrefabsNames;
    public int index = 0;

    private GameObject selectedObject;
    private MeshRenderer mesh;
    private float size;
    private GameObject[] roadPrefabs;

	// Use this for initialization
	void Start () {
        mesh = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void SetObject(GameObject selectedObj){
        mesh.enabled = true;
        selectedObject = selectedObj;
    }

    public void ResetOrb()
    {
        //if (Selection.activeGameObject == null || Selection.activeGameObject != gameObject){
            // this orb is not selected so hide it
            mesh.enabled = false;
        //}

        if (Selection.activeGameObject == gameObject){
            mesh.enabled = true;
        }
    }
    public void Look(){
        Vector3 direction = transform.position - selectedObject.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, size)){
            ResetOrb();
        }
    }
    public void SetSize(float size){
        this.size = size;
    }
    public void GenerateRoad(GameObject roadType){
        GameObject clone = PrefabUtility.InstantiatePrefab(roadType) as GameObject;
        clone.name = "GeneratedBlock";

        //Figure out position
        Vector3 orbPosition = transform.position;
        if (gameObject.name.Contains("North"))
        {
            orbPosition.x -= size;
        }
        else if (gameObject.name.Contains("South"))
        {
            orbPosition.x += size;
        }
        else if (gameObject.name.Contains("East"))
        {
            orbPosition.z += size;
        }
        else if (gameObject.name.Contains("West"))
        {
            orbPosition.z -= size;
        }
        else if (gameObject.name.Contains("Up"))
        {
            //place at orb
        }
        clone.transform.position = orbPosition;
        Selection.activeGameObject = clone;
    }
}
#endif
