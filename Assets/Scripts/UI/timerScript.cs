using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class timerScript : MonoBehaviour {

    //A stage floor is included in the hud, as a hud will always be present.
    public int StageFloor;
    public float startTime;
    public float timeRemaining;
    public Text timeDisplay;
    public Text timePreview;
    public ButtonScripts buttonScripts;

    public bool spawning;
    public bool win;

    // Use this for initialization
    void Start()
    {
        timeRemaining = startTime;
    }

    // Update is called once per frame
    void Update()
    {

            //if( !GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>().win)
            if (!win && !spawning)
            {
            if (timeRemaining >= 0)
            {
                timeRemaining -= Time.deltaTime;
                timeDisplay.enabled = true;
                timeDisplay.color = Color.white;
                if (timeRemaining < 0)
                {
                    timeRemaining = 0;
                    timeDisplay.color = Color.red;
                }
            }
                

            }
            else
            {
                timeDisplay.enabled = false;
            }

            int minutes = (int)timeRemaining / 60;
            int seconds = (int)timeRemaining % 60;

            string textTime = string.Format("{0:00} : {1:00}", minutes, seconds);
            timeDisplay.text = textTime;

            if (timeRemaining < 0)
            {
                buttonScripts.GameOver();
            }
        
        
    }

    public void addTime(int additionalTime)
    {
        timeRemaining += additionalTime;
    }

    public void setSpawning()
    {
        spawning = true;
    }
    public void setWin()
    {
        timePreview.text = timeRemaining.ToString();
        if (timeRemaining > PlayerPrefs.GetFloat("BestTime " + SceneManager.GetActiveScene().name))
        {
            PlayerPrefs.SetFloat("BestTime " + SceneManager.GetActiveScene().name, timeRemaining);
        }
        win = true;
    }
    public void setGame()
    {
        spawning = false;
        win = false;
    }
}
