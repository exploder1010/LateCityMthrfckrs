using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
public class LevelEditor3D : MonoBehaviour
{
    public bool Active = true;
    public float defaultBlockSize;
    public GameObject prefab2D;

    [HideInInspector]
    public GameObject[] roadPrefabs;
    [HideInInspector]
    public GameObject selectedObject = null;

    private GameObject northOrb;
    private GameObject southOrb;
    private GameObject eastOrb;
    private GameObject westOrb;
    private GameObject upOrb;
    
    private float blockSize;
    private bool showBox = false;
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
        LoadPrefabs();
    }

    private void Update()
    {
        LoadPrefabs();
    }

    private void OnDrawGizmos()
    {
        if (selectedObject != null && showBox)
        {
            Vector3 boxPos = selectedObject.transform.position;
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(boxPos, new Vector3(blockSize * 2, blockSize * 2, blockSize * 2));
        }
    }

    public void DrawOrbs()
    {
        InformOrbs("ResetOrb", null);
        if (Active){
            showBox = true;
            FindSelectedObject();
            if (selectedObject == null || selectedObject.name.Contains("LevelEditor")){
                showBox = false;
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

    private void FindSelectedObject()
    {
        selectedObject = Selection.activeGameObject;

        //If nothing is selected
        if (selectedObject == null)
        {
            return;
        }

        //Find parent node
        while (selectedObject.transform.parent != null
            && selectedObject.transform.parent.name != "LevelBlocks"
            && selectedObject.transform.parent.name != "LevelEditor3D")
        {
            selectedObject = selectedObject.transform.parent.gameObject;
        }
    }
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
        if(northOrb && southOrb && eastOrb && westOrb && upOrb)
        {
            northOrb.SendMessage(methodName, message);
            southOrb.SendMessage(methodName, message);
            eastOrb.SendMessage(methodName, message);
            westOrb.SendMessage(methodName, message);
            upOrb.SendMessage(methodName, message);
        }
   
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

    private void AddToPrefabs(GameObject obj)
    {
        LoadPrefabs();
    }
    private void LoadPrefabs()
    {
        roadPrefabs = Resources.LoadAll<GameObject>("Prefabs/3dLevelBlocks/");
    }
    private void Convert2d()
    {
        if (prefab2D != null)
        {
            //spawn blank
            GameObject blank = new GameObject();

            //spawn prefab2d then parent in blank
            GameObject newPart = Instantiate(prefab2D, blank.transform);
            newPart.tag = "Untagged";
            blank.tag = "LevelBlock";

            //position prefab2d down -29.5
            Vector3 pos = Vector3.zero;
            pos.y = -29.5f;
            newPart.transform.position = pos;

            //save new prefab using old name
            blank.name = prefab2D.name;
            string fullPath = "Assets/Resources/Prefabs/3dLevelBlocks/" + blank.name + ".prefab";
            UnityEngine.Object prefab = PrefabUtility.CreatePrefab(fullPath, blank);

            //delete blank
            DestroyImmediate(blank);
            LoadPrefabs();
            prefab2D = null;
        }
    }
}
#endif