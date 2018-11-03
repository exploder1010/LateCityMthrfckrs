using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
// Creates an instance of a primitive depending on the option selected by the user.
public class customWindow : EditorWindow
{
    public string[] options = new string[] { "Straight Road", "Curved Road" };
    public int index = 0;

    private GameObject[] roadPrefabs;

    [MenuItem("Tools/LevelEditor")]
    static void Init()
    {
        EditorWindow window = GetWindow(typeof(customWindow));
        window.Show();
    }

    void Update()
    {
        if (!Application.isPlaying)
        {
            Repaint();
            GameObject.Find("LevelEditor").SendMessage("DrawOrbs");
        }
    }
    void OnGUI()
    {
        if (Selection.activeGameObject != null && Selection.activeGameObject.name.Contains("LevelEditorORB"))
        {
            levelEditor le = GameObject.Find("LevelEditor").GetComponent<levelEditor>();
            roadPrefabs = le.RoadPrefabs;
            options = new string[roadPrefabs.Length];
            for (int i = 0; i < roadPrefabs.Length; i++)
            {
                options[i] = roadPrefabs[i].name;
            }
            index = EditorGUILayout.Popup(index, options);
            if (GUILayout.Button("Create"))
                InstantiatePrimitive(Selection.activeGameObject);
        }
        else if (Selection.activeGameObject != null && Selection.activeGameObject.name.Contains("GeneratedBlock"))
        {
            if (GUILayout.Button("Rotate")){
                Vector3 byAngle = new Vector3(0, 90, 0);
                Quaternion toAngle = Quaternion.Euler(Selection.activeTransform.eulerAngles + byAngle);
                Selection.activeTransform.rotation = toAngle;
            }
        }
        if (Selection.activeGameObject != null && Selection.activeGameObject.name.Contains("LevelEditor") && GameObject.FindGameObjectsWithTag("LevelBlock").Length == 0)
        {
            levelEditor le = GameObject.Find("LevelEditor").GetComponent<levelEditor>();
            roadPrefabs = le.RoadPrefabs;
            options = new string[roadPrefabs.Length];
            for (int i = 0; i < roadPrefabs.Length; i++)
            {
                options[i] = roadPrefabs[i].name;
            }
            index = EditorGUILayout.Popup(index, options);
            if (GUILayout.Button("Create"))
                InstantiatePrimitive(Selection.activeGameObject);
        }
        else
        {
            EditorGUILayout.LabelField("Please select an orb, then reselect this window");
        }
    }

    void InstantiatePrimitive(GameObject selectedOrb)
    {
        GameObject newRoadType = roadPrefabs[index];
        selectedOrb.SendMessage("GenerateRoad", newRoadType);
    }
}
#endif