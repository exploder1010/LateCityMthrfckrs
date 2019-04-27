using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelselectScript : MonoBehaviour {


    public string selectedLevel;
    public GameObject LevelPreview;
    public GameObject BreezyStreet;
    public GameObject MachinationWay;
    public GameObject Bravura;
    public GameObject WallStreet;

    //Added changes to Title for level preview.
    public Text Title;

    public void Start()
    {
        LoadTimes();
    }

    public void LoadTimes()
    {
        print("Loading times");
        GameObject[] levels;
        levels = GameObject.FindGameObjectsWithTag("Level");
        foreach(GameObject level in levels)
        {
            level.transform.Find("BestTime").GetComponent<Text>().text = PlayerPrefs.GetFloat("BestTime " + "Level_" + level.name).ToString("F2");
            print("BestTime " + "Level_" + level.name);
        }
        HideLevels();
    }

    private void HideLevels()
    {
        if (Bravura)
        {
            Bravura.SetActive(false);
        }
        if (BreezyStreet)
        {
            BreezyStreet.SetActive(false);
        }
        if (MachinationWay)
        {
            MachinationWay.SetActive(false);
        }
        if (WallStreet)
        {
            WallStreet.SetActive(false);
        }
    }

    public void SelectLevel(GameObject thisLevel)
    {
        selectedLevel = thisLevel.name;
        LevelPreview.transform.Find(selectedLevel).gameObject.SetActive(true);
        Title.text = selectedLevel;
        string mapname = "Level_" + selectedLevel.Replace(" ", "_");
        LeaderboardScript lbscript = new LeaderboardScript();
        lbscript.leaderboardUI = LevelPreview.transform.parent.Find("Leaderboard");
        StartCoroutine(lbscript.SQL_GetScores(mapname));
    }

    public void PlayLevel()
    {
        SceneManager.LoadScene("Level_" + selectedLevel);
    }

    //Brady Jacobson
    //Used to get rid of the currently used preview.
    public void DeactivatePreview()
    {
        LevelPreview.transform.Find(selectedLevel).gameObject.SetActive(false);
    }

    public void HighlightButton(UnityEngine.UI.Button button)
    {
        button.Select();
    }
}
