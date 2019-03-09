using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class LeaderboardScript : MonoBehaviour {
    public Transform leaderboardUI;
    public Text namePrompt;
    public timerScript Timer;
   
    public void SavetoDictionary()
    {
        if (namePrompt.text != "")
            StartCoroutine(SQL_Insert());
    }

    public void ShowLeaderboard()
    {
        string mapName = SceneManager.GetActiveScene().name;
        StartCoroutine(SQL_GetScores(mapName.Replace(" ", "_")));
        transform.Find("Leaderboard").Find("Title").GetComponent<Text>().text = mapName.Replace("Level_", "");
    }

    public void CheckTime()
    {
        StartCoroutine(SQL_TimeCheck());
    }

    IEnumerator SQL_Insert()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", namePrompt.text);
        form.AddField("time", Timer.timeRemaining.ToString());
        form.AddField("map", SceneManager.GetActiveScene().name.Replace(" ", "_"));
        WWW server = new WWW("http://latecityriders.com/leaderboard/insert.php", form);
        yield return server;
        ShowLeaderboard();
    }

    public IEnumerator SQL_GetScores(string mapName)
    {
        WWWForm form = new WWWForm();
        form.AddField("map", mapName);
        WWW server = new WWW("http://latecityriders.com/leaderboard/getscores.php", form);
        yield return server;
        Debug.Log(leaderboardUI);
        string[] rows = server.text.Split(';');
        for (int i = 0; i < 6; i++){
            Text nameText = leaderboardUI.Find("Name" + (i+1).ToString()).GetComponent<Text>();
            Text timeText = leaderboardUI.Find("Time" + (i+1).ToString()).GetComponent<Text>();
            nameText.text = (i+1) + ": ";
            timeText.text = "";

            if (i < rows.Length){
                string[] r = rows[i].Split('|');
                nameText.text += r[0];
                if (r.Length > 1)
                    timeText.text = r[1];
            }
        }
    }

    IEnumerator SQL_TimeCheck()
    {
        WWWForm form = new WWWForm();
        form.AddField("time", Timer.timeRemaining.ToString());
        form.AddField("map", SceneManager.GetActiveScene().name);
        WWW server = new WWW("http://latecityriders.com/leaderboard/topten.php", form);
        yield return server;
        //TODO Finish working on this
        //
        //transform.Find("InputField").GetComponent<InputField>().interactable = true;
        //transform.Find("Button").GetComponent<Button>().interactable = true;
        //if (int.Parse(server.text) >= 10)
        //{
        //    //deactivate text and submit
        //    Debug.Log("WTF");
        //    transform.Find("InputField").GetComponent<InputField>().interactable = false;
        //    transform.Find("Button").GetComponent<Button>().interactable = false;
        //}
    }
}
