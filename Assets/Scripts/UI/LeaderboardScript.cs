using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        print(leaderboard[namePrompt.text]);
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
}
