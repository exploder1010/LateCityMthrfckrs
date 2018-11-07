using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
public class LevelEditor : MonoBehaviour
{
    public bool Active = true;
    public float defaultBlockSize;
    public GameObject[] roadPrefabs;

    private GameObject northOrb;
    private GameObject southOrb;
    private GameObject eastOrb;
    private GameObject westOrb;
    private GameObject upOrb;
    private float blockSize;

    // Use this for initialization
    void Start()
    {
        northOrb = GameObject.Find("LevelEditorORB_North");
        southOrb = GameObject.Find("LevelEditorORB_South");
        eastOrb = GameObject.Find("LevelEditorORB_East");
        westOrb = GameObject.Find("LevelEditorORB_West");
        upOrb = GameObject.Find("LevelEditorORB_Up");
        InformOrbs("SetSize", defaultBlockSize);
        PrefabUtility.DisconnectPrefabInstance(gameObject);
    }

    public void DrawOrbs()
    {
        InformOrbs("ResetOrb", null);
        if (Active){
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null){
                return;
            }
            while(selectedObject.transform.parent != null 
                && selectedObject.transform.parent.name != "LevelBlocks"
                && !selectedObject.name.Contains("LevelEditor")){
                selectedObject = selectedObject.transform.parent.gameObject;
            }
            if (selectedObject.name.Contains("LevelEditor"))
            {
                return;
            }
            if (!selectedObject.CompareTag("LevelBlock"))
            {
                return;
            }
            LevelBlock blockScript = selectedObject.GetComponent<LevelBlock>();
            blockSize = blockScript == null ? defaultBlockSize : blockScript.BlockSize;

            SetOrbPosition(selectedObject);
            InformOrbs("SetObject", selectedObject);
            InformOrbs("Look", null);
        }
    }

    public GameObject[] RoadPrefabs { get { return roadPrefabs; }}

    private void SetOrbPosition(GameObject selectedObject)
    {
        Vector3 objPosition = selectedObject.transform.position;
        objPosition.x -= blockSize;
        northOrb.transform.position = objPosition;
        objPosition.x += 2 * blockSize;
        southOrb.transform.position = objPosition;
        objPosition = selectedObject.transform.position;
        objPosition.z += blockSize;
        eastOrb.transform.position = objPosition;
        objPosition.z -= 2 * blockSize;
        westOrb.transform.position = objPosition;
        objPosition = selectedObject.transform.position;
        objPosition.y += defaultBlockSize;
        upOrb.transform.position = objPosition;
    }

    private void InformOrbs(string methodName, object message){
        northOrb.SendMessage(methodName, message);
        southOrb.SendMessage(methodName, message);
        eastOrb.SendMessage(methodName, message);
        westOrb.SendMessage(methodName, message);
        upOrb.SendMessage(methodName, message);
    }
    public void GenerateRoad(GameObject roadType)
    {
        GameObject parent = GameObject.Find("LevelBlocks");
        if (parent == null)
        {
            parent = new GameObject();
            parent.transform.position = Vector3.zero;
            parent.name = "LevelBlocks";
        }
        GameObject clone = PrefabUtility.InstantiatePrefab(roadType) as GameObject;
        clone.name = "GeneratedBlock";
        Vector3 orbPosition = transform.position;
        clone.transform.position = orbPosition;
        clone.transform.parent = parent.transform;
        Selection.activeGameObject = clone;
    }
}
#endif