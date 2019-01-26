using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelselectScript : MonoBehaviour {


    public string selectedLevel;
    public GameObject LevelPreview;

    public void SelectLevel(GameObject thisLevel)
    {
        selectedLevel = thisLevel.name;
        LevelPreview.transform.Find(selectedLevel).gameObject.SetActive(true);
        //load respective leaderboard
    }

    public void PlayLevel()
    {
        SceneManager.LoadScene("Level_" + selectedLevel);
    }
}
