using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class LeaderboardScript : MonoBehaviour {

    public Dictionary<string, float> leaderboard = new Dictionary<string, float>();
    public Text leaderboardText;
    public Text namePrompt;
    public timerScript Timer;

	// Use this for initialization
	void Awake () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SavetoDictionary()
    {
        //if(File.Exists(Application.dataPath + "leaderboard.json"))
        //{
        //    string leaderboardJSON = File.ReadAllText(Application.dataPath + "leaderboard.json");
        //    leaderboard = JsonUtility.FromJson<LeaderboardScript>(leaderboardJSON);
        //}
        print("saving");
        if (!leaderboard.ContainsKey(namePrompt.text))
        {
            leaderboard.Add(namePrompt.text, Timer.timeRemaining);
            print(leaderboard[namePrompt.text]);
        }
        else
        {
            leaderboard[namePrompt.text] = Timer.timeRemaining;
        }
        //string leaderboardJSON = JsonUtility.ToJson(leaderboard);
        //string filePath = Application.dataPath;
        //File.WriteAllText(filePath + "leaderboard.json", leaderboardJSON);
        print(leaderboard[namePrompt.text]);



        //SQL code
        StartCoroutine(SQL_Insert());
    }

    public void ShowLeaderboard()
    {
        List<KeyValuePair<string, float>> sortedDict = new List<KeyValuePair<string, float>>();
        foreach (var entry in leaderboard)
        {
            sortedDict.Add(entry);
        }
        sortedDict.Sort(delegate (KeyValuePair<string, float> pair1, KeyValuePair<string, float> pair2)
        {
            return pair1.Value.CompareTo(pair2.Value);
        }
        );
        foreach (var entry in sortedDict)
        {
            leaderboardText.text += entry;
            leaderboardText.text += "\n";
        }
    }

    IEnumerator SQL_Insert()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", "fakeNAme");
        form.AddField("time", Timer.timeRemaining.ToString());
        form.AddField("map", SceneManager.GetActiveScene().name);
        WWW server = new WWW("http://latecityriders.zapto.org/insert.php", form);
        Debug.Log(namePrompt.text);
        yield return server;
        Debug.Log("server response:");
        Debug.Log(server.text);
        Debug.Log("END");
    }
}
