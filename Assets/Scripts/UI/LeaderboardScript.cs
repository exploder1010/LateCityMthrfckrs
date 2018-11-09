using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaderboardScript : MonoBehaviour {

    public Dictionary<string, int> leaderboard;
    public Text leaderboardText;
    public GameObject namePrompt;
    public timerScript Timer;

	// Use this for initialization
	void Awake () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SavetoDictionary()
    {
        namePrompt.SetActive(true);
        //leaderboard.Add(namePrompt.SubmitEvent(), Timer.timeRemaining);
    }

    public void ShowLeaderboard()
    {
        foreach (var entry in leaderboard)
        {
            leaderboardText.text += entry;
            leaderboardText.text += "\n";
        }
    }

    public void SaveScore()
    {
        //using (StreamWriter file = new StreamWriter(SceneManager.GetActiveScene().name))
        //{
        //    //read player score and ask for name
        //    file.WriteLine("name" + "score");
        //}
    }
}
