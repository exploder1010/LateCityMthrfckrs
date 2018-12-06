using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
// Creates an instance of a primitive depending on the option selected by the user.
public class CustomWindow : EditorWindow
{
    public string[] options = new string[] { "Straight Road", "Curved Road" };
    public int index = 0;

    private GameObject[] roadPrefabs;
    private string localPath = "Assets/Prefabs/LevelBlocks/";
    private string prefabName = "";
    private string increaseAmount = "";
    private string increasePitch = "";
    private bool is3D = false;

    private GameObject lastOrbUsed;
    private GameObject lastPrefabUsed;
    private GameObject clickedObject;

    [MenuItem("Tools/LevelEditor")]
    static void Init()
    {
        EditorWindow window = GetWindow(typeof(CustomWindow));
        window.Show();
    }
    void Update()
    {
        if (!Application.isPlaying)
        {
            //if (clickedObject != Selection.activeGameObject)
            {
                Repaint();
                clickedObject = Selection.activeGameObject;
            }
        }
    }
    void OnGUI()
    {
        GameObject levelEditor = null, selectedObject = null;
        if (GameObject.Find("LevelEditor") != null)
        {
            is3D = false;
            levelEditor = GameObject.Find("LevelEditor");
            levelEditor.SendMessage("DrawOrbs");
            selectedObject = Selection.activeGameObject;
        }
        else if (GameObject.Find("LevelEditor3D") != null)
        {
            is3D = true;
            localPath = "Assets/Resources/Prefabs/3dLevelBlocks/";
            levelEditor = GameObject.Find("LevelEditor3D");
            levelEditor.SendMessage("DrawOrbs");
            selectedObject = levelEditor.GetComponent<LevelEditor3D>().selectedObject;
        }

        //Display to Window
        GUILayout.Label("Level Editor", EditorStyles.boldLabel, GUILayout.Height(30));
        if (levelEditor == null)
        {
            if (GUILayout.Button("Add Level Editor to Scene"))
            {
                GameObject newEditor = Resources.Load<GameObject>("Prefabs/LevelEditor3D");
                newEditor = Instantiate(newEditor);
                newEditor.name = "LevelEditor3D";
                GameObject blank = Resources.Load<GameObject>("Prefabs/3dLevelBlocks/Blank");
                newEditor.SendMessage("GenerateRoad", blank);
            }
        }
        else if (selectedObject == null && is3D)
        {
            if (GUILayout.Button("Add Blank"))
            {
                GameObject blank = Resources.Load<GameObject>("Prefabs/3dLevelBlocks/Blank");
                blank = Instantiate(blank);
                Selection.activeGameObject = blank;
            }
        }
        else if (selectedObject.name.Contains("LevelEditorORB"))
        {
            Option_Create(selectedObject);
        }
        else if (selectedObject.name.Contains("Blank"))
        {
            Option_Save(selectedObject);
        }
        else if (selectedObject.tag.Contains("LevelBlock"))
        {
            Option_Rotate(selectedObject);
            Option_RepeatAction();
            Option_Replace(levelEditor, selectedObject);
            Option_CreateAtOrb(selectedObject);
            Option_RaiseBlock(selectedObject);
            Option_Pitch(selectedObject);
        }
        else if (selectedObject.name.Contains("LevelEditor3D"))
        {
            Option_Convert(levelEditor);
        }
        else if (selectedObject.name.Contains("LevelEditor"))
        {
            Option_Upgrade(levelEditor);
        }
        else
        {
            EditorGUILayout.LabelField("Please select an orb, then reselect this window");
        }
    }
    
    private void Option_Convert(GameObject levelEditor)
    {
        GUILayout.Label("Convert 2D prefab to 3D");
        GUILayout.Label("add the prefab to the 'Prefab 2D property");
        if (GUILayout.Button("Convert"))
        {
            levelEditor.SendMessage("Convert2d");
        }
    }
    private void Option_Upgrade(GameObject levelEditor)
    {
        GUILayout.Label("Upgrade to 3D LevelEditor");
        if (GUILayout.Button("Upgrade"))
        {
            DestroyImmediate(levelEditor);
            GameObject[] oldBlocks = GameObject.FindGameObjectsWithTag("LevelBlock");
           
            foreach (GameObject block in oldBlocks)
            {
                block.tag = "Untagged";
                GameObject empty = new GameObject();
                empty.name = "GeneratedBlock";
                empty.tag = "LevelBlock";
                Vector3 pos = block.transform.position;
                Debug.Log(pos);
                pos.y += 30;
                empty.transform.position = pos;
                block.transform.parent = empty.transform;

            }
            GameObject newEditor = Resources.Load<GameObject>("Prefabs/LevelEditor3D");
            newEditor = Instantiate(newEditor);
            newEditor.name = "LevelEditor3D";
        }
    }
    private void Option_Pitch(GameObject selectedObject)
    {
        bool pitch = true; //only do pitch change if all children can change
        foreach (Transform child in selectedObject.transform)
        {
            LevelBlock script = child.GetComponent<LevelBlock>();
            if (script == null || !script.Pitch)
            {
                pitch = false;
            }
        }
        if (pitch)
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Change Pitch");
            GUILayout.BeginHorizontal();
            increasePitch = GUILayout.TextField(increasePitch, GUILayout.Width(100));
            float increaseInt = 0;
            if (increasePitch.Contains("%"))
            {
                float.TryParse(increasePitch.Replace("%", ""), out increaseInt);
            }
            else
            {
                float.TryParse(increasePitch, out increaseInt);
            }

            if (GUILayout.Button("Change Pitch") && increaseInt != 0)
            {
                EditorUtility.DisplayDialog("SORRY",
                        "This feature is still a work in progress. I will update you when it is complete",
                        "OK");
                ////rotate
                //Vector3 byAngle = new Vector3(increaseInt, 0, 0);

                //foreach (Transform child in selectedObject.transform)
                //{
                //    if (increaseInt + child.eulerAngles.x > 45)
                //        increaseInt = 45 - child.eulerAngles.x;
                //    Vector3 scale = child.localScale;
                //    Vector3 pos = child.position;
                //    float cTheta = Mathf.Cos(increaseInt * Mathf.PI / 180);
                //    float h = Mathf.Abs(child.eulerAngles.x) < Mathf.Abs(byAngle.x) ? scale.z / cTheta : scale.z * cTheta;
                //    float o = Mathf.Tan(increaseInt * Mathf.PI / 180) * 60;

                //    scale.z = h;
                //    pos.y += o/2;

                //    child.localScale = scale;
                //    child.position = pos;
                //    Quaternion toAngle = Quaternion.Euler(child.eulerAngles + byAngle);
                //    child.rotation = toAngle;
                //}
                increasePitch = "";
            }
            GUILayout.EndHorizontal();
        }
    }
    private void Option_RaiseBlock(GameObject selectedObject)
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Raise Block");
        GUILayout.BeginHorizontal();
        increaseAmount = GUILayout.TextField(increaseAmount, GUILayout.Width(100));
        double increaseInt = 0;
        if (increaseAmount.Contains("%"))
        {
            Double.TryParse(increaseAmount.Replace("%", ""), out increaseInt);
            increaseInt = Math.Tan(increaseInt * Math.PI / 180) * 60;
        }
        else
        {
            Double.TryParse(increaseAmount, out increaseInt);
        }

        if (GUILayout.Button("Shift") && increaseInt != 0)
        {
            Vector3 pos = selectedObject.transform.position;
            pos.y += (float)increaseInt;
            selectedObject.transform.position = pos;
            increaseAmount = "";
        }
            GUILayout.EndHorizontal();
    }
    private void Option_RepeatAction()
    {
        if (GUILayout.Button("Repeat Last Action"))
        {
            lastOrbUsed.SendMessage("GenerateRoad", lastPrefabUsed);
        }
    }
    private void Option_Save(GameObject selectedObject)
    {
        GUILayout.Label("Create New Prefab", EditorStyles.boldLabel, GUILayout.Height(30));
        EditorGUILayout.LabelField("Prefab Name", GUILayout.Width(250));
        prefabName = GUILayout.TextField(prefabName, GUILayout.Width(300));
        if (GUILayout.Button("Create Prefab") && prefabName != "")
        {
            string fullPath = localPath + prefabName + ".prefab";
            if (AssetDatabase.LoadAssetAtPath(fullPath, typeof(GameObject)))
            {
                //Create dialog to ask if User is sure they want to overwrite existing prefab
                if (EditorUtility.DisplayDialog("Are you sure?",
                        "The prefab already exists. Do you want to overwrite it?",
                        "Yes",
                        "No"))
                //If the user presses the yes button, create the Prefab
                {
                    CreateNew(selectedObject, fullPath);
                }
            }
            //If the name doesn't exist, create the new Prefab
            else
            {
                CreateNew(selectedObject, fullPath);
            }
            prefabName = "";
        }
    }

    private void Option_Replace(GameObject levelEditor, GameObject selectedObject)
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Switch block");
        fillDropdownOptions();
        index = EditorGUILayout.Popup(index, options, GUILayout.Width(240));
        if (GUILayout.Button("Switch", GUILayout.Width(240)))
        {
            Vector3 pos = selectedObject.transform.position;
            DestroyImmediate(selectedObject);
            levelEditor.transform.position = pos;
            InstantiatePrimitive(levelEditor);
        }
    }

    private void Option_CreateAtOrb(GameObject selectedObject)
    {
        fillDropdownOptions();
        GUILayout.Space(10);
        GUILayout.Label("Spawn block at Orb");
        index = EditorGUILayout.Popup(index, options, GUILayout.Width(490));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("White", GUILayout.Width(120)))
        {
            GameObject orb = GameObject.Find("LevelEditorORB_North");
            if (orb != null)
                InstantiatePrimitive(orb);
        }
        if (GUILayout.Button("Blue", GUILayout.Width(120)))
        {
            GameObject orb = GameObject.Find("LevelEditorORB_East");
            if (orb != null)
                InstantiatePrimitive(orb);
        }
        if (GUILayout.Button("Red", GUILayout.Width(120)))
        {
            GameObject orb = GameObject.Find("LevelEditorORB_South");
            if (orb != null)
                InstantiatePrimitive(orb);
        }
        if (GUILayout.Button("Green", GUILayout.Width(120)))
        {
            GameObject orb = GameObject.Find("LevelEditorORB_West");
            if (orb != null)
                InstantiatePrimitive(orb);
        }
        GUILayout.EndHorizontal();
    }
    private void Option_Create(GameObject selectedObject)
    {
        fillDropdownOptions();
        index = EditorGUILayout.Popup(index, options, GUILayout.Width(240));
        if (GUILayout.Button("Create", GUILayout.Width(240)))
        {
            lastOrbUsed = selectedObject;
            InstantiatePrimitive(selectedObject);
        }
    }
    private void Option_Rotate(GameObject selectedObject)
    {
        if (GUILayout.Button("Rotate"))
        {
            Vector3 byAngle = new Vector3(0, 90, 0);
            Quaternion toAngle = Quaternion.Euler(selectedObject.transform.eulerAngles + byAngle);
            selectedObject.transform.rotation = toAngle;
        }
    }

    private void fillDropdownOptions()
    {
        GameObject levelEditor = is3D ? GameObject.Find("LevelEditor3D") : GameObject.Find("LevelEditor");
        if (is3D)
        {
            LevelEditor3D le = levelEditor.GetComponent<LevelEditor3D>();
            roadPrefabs = le.RoadPrefabs;
        }
        else
        {
            LevelEditor le = levelEditor.GetComponent<LevelEditor>();
            roadPrefabs = le.RoadPrefabs;
        }

        options = new string[roadPrefabs.Length];
        for (int i = 0; i < roadPrefabs.Length; i++)
        {
            options[i] = roadPrefabs[i].name;
        }
    }

    private void InstantiatePrimitive(GameObject selectedOrb)
    {
        GameObject newRoadType = roadPrefabs[index];
        lastPrefabUsed = newRoadType;
        selectedOrb.SendMessage("GenerateRoad", newRoadType);
    }

    private void CreateNew(GameObject obj, string localPath)
    {
        //Create a new prefab at the path given
        UnityEngine.Object prefab = PrefabUtility.CreatePrefab(localPath, obj);
        GameObject levelEditor = is3D ? GameObject.Find("LevelEditor3D") : GameObject.Find("LevelEditor");

        levelEditor.SendMessage("AddToPrefabs", prefab);
    }
}
#endif