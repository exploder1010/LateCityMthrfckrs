using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class LeaderboardScript : MonoBehaviour {
    public Text leaderboardText;
    public Text namePrompt;
    public timerScript Timer;

    public void SavetoDictionary()
    {
        if (namePrompt.text != "")
            StartCoroutine(SQL_Insert());
    }

    public void ShowLeaderboard()
    {
        StartCoroutine(SQL_GetScores());
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
        form.AddField("map", SceneManager.GetActiveScene().name);
        WWW server = new WWW("http://latecityriders.zapto.org/leaderboard/insert.php", form);
        yield return server;
        ShowLeaderboard();
    }

    IEnumerator SQL_GetScores()
    {
        leaderboardText.text = "";
        WWWForm form = new WWWForm();
        form.AddField("map", SceneManager.GetActiveScene().name);
        WWW server = new WWW("http://latecityriders.zapto.org/leaderboard/getscores.php", form);
        yield return server;
        foreach (string row in server.text.Split(';')){
            leaderboardText.text += row;
            leaderboardText.text += '\n';
        }
    }

    IEnumerator SQL_TimeCheck()
    {
        WWWForm form = new WWWForm();
        form.AddField("time", Timer.timeRemaining.ToString());
        form.AddField("map", SceneManager.GetActiveScene().name);
        WWW server = new WWW("http://latecityriders.zapto.org/leaderboard/topten.php", form);
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
