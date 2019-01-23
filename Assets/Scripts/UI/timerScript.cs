using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timerScript : MonoBehaviour {

    public float startTime;
    public float timeRemaining;
    public Text timeDisplay;
    public ButtonScripts buttonScripts;

    bool spawning;
    bool win;

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
                timeRemaining -= Time.deltaTime;
                timeDisplay.enabled = true;

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
        win = true;
    }
    public void setGame()
    {
        spawning = false;
        win = false;
    }
}
