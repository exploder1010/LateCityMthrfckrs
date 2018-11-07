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

    [MenuItem("Tools/LevelEditor")]
    static void Init()
    {
        EditorWindow window = GetWindow(typeof(CustomWindow));
        window.Show();
    }

    void Update()
    {
        if (!Application.isPlaying && GameObject.Find("LevelEditor") != null)
        {
            Repaint();
            GameObject.Find("LevelEditor").SendMessage("DrawOrbs");
        }
    }
    void OnGUI()
    {
        GameObject levelEditor = GameObject.Find("LevelEditor");
        GUILayout.Label("Level Editor", EditorStyles.boldLabel, GUILayout.Height(30));
        if (Selection.activeGameObject != null && Selection.activeGameObject.name.Contains("LevelEditorORB"))
        {
            fillDropdownOptions();
            index = EditorGUILayout.Popup(index, options, GUILayout.Width(240));
            if (GUILayout.Button("Create", GUILayout.Width(240)))
                InstantiatePrimitive(Selection.activeGameObject);
        }
        else if (Selection.activeGameObject != null && Selection.activeGameObject.name.Contains("GeneratedBlock"))
        {
            if (GUILayout.Button("Rotate")){
                Vector3 byAngle = new Vector3(0, 90, 0);
                Quaternion toAngle = Quaternion.Euler(Selection.activeTransform.eulerAngles + byAngle);
                Selection.activeTransform.rotation = toAngle;
            }

            EditorGUILayout.LabelField("", GUILayout.Height(60));
            EditorGUILayout.LabelField("Switch block");
            fillDropdownOptions();
            index = EditorGUILayout.Popup(index, options, GUILayout.Width(240));
            if (GUILayout.Button("Switch", GUILayout.Width(240)))
            {
                Vector3 pos = Selection.activeTransform.position;
                DestroyImmediate(Selection.activeGameObject);
                levelEditor.transform.position = pos;
                InstantiatePrimitive(levelEditor);
            }
        }
        else if (Selection.activeGameObject != null && Selection.activeGameObject.name.Contains("LevelEditor") && GameObject.FindGameObjectsWithTag("LevelBlock").Length == 0)
        {
            fillDropdownOptions();
            index = EditorGUILayout.Popup(index, options);
            if (GUILayout.Button("Create"))
                InstantiatePrimitive(Selection.activeGameObject);
        }
        else
        {
            EditorGUILayout.LabelField("Please select an orb, then reselect this window");
        }
    }

    private void fillDropdownOptions()
    {
        LevelEditor le = GameObject.Find("LevelEditor").GetComponent<LevelEditor>();
        roadPrefabs = le.RoadPrefabs;
        options = new string[roadPrefabs.Length];
        for (int i = 0; i < roadPrefabs.Length; i++)
        {
            options[i] = roadPrefabs[i].name;
        }
    }

    private void InstantiatePrimitive(GameObject selectedOrb)
    {
        GameObject newRoadType = roadPrefabs[index];
        selectedOrb.SendMessage("GenerateRoad", newRoadType);
    }
}
#endif