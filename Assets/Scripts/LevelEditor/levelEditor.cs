using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
public class levelEditor : MonoBehaviour
{
    public bool Active = true;
    public float blockSize;
    public GameObject[] roadPrefabs;

    private GameObject northOrb;
    private GameObject southOrb;
    private GameObject eastOrb;
    private GameObject westOrb;
    private GameObject upOrb;

    // Use this for initialization
    void Start()
    {
        northOrb = GameObject.Find("LevelEditorORB_North");
        southOrb = GameObject.Find("LevelEditorORB_South");
        eastOrb = GameObject.Find("LevelEditorORB_East");
        westOrb = GameObject.Find("LevelEditorORB_West");
        upOrb = GameObject.Find("LevelEditorORB_Up");
    }

    // Update is called once per frame
    void Update()
    {

        InformOrbs("ResetOrb", null);
        if (Active){
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null){
                return;
            }
            while(selectedObject.transform.parent != null){
                selectedObject = selectedObject.transform.parent.gameObject;
            }

            if (selectedObject.name.Contains("LevelEditor")){
                return;
            }
            InformOrbs("SetSize", blockSize);

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
        objPosition.y += blockSize;
        upOrb.transform.position = objPosition;
    }

    private void InformOrbs(string methodName, object message){
        northOrb.SendMessage(methodName, message);
        southOrb.SendMessage(methodName, message);
        eastOrb.SendMessage(methodName, message);
        westOrb.SendMessage(methodName, message);
        upOrb.SendMessage(methodName, message);
    }
}
#endif