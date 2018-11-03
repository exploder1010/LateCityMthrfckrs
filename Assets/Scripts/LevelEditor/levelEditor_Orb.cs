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
        mesh.enabled = false;
        
        if (Selection.activeGameObject == gameObject){
            mesh.enabled = true;
        }
    }

    //Looks to see if orb is already touching another levelblock
    public void Look()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("LevelBlock");
        Collider myCollider = GetComponent<Collider>();
        int count = 0;
        foreach (GameObject g in gameObjects)
        {
            Collider theirCollider = g.GetComponent<Collider>();
            if (myCollider.bounds.Intersects(theirCollider.bounds))
            {
                count++;
            }
        }
        if (count > 1)
        {
            ResetOrb();
        }
    }
    public void SetSize(float size){
        this.size = size;
    }
    public void GenerateRoad(GameObject roadType){
        GameObject parent = GameObject.Find("LevelBlocks");
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
        clone.transform.parent = parent.transform;
        Selection.activeGameObject = clone;
    }
}
#endif
