﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timerScript : MonoBehaviour {

    public float startTime;
    public float timeRemaining;
    public Text timeDisplay;
    public ButtonScripts buttonScripts;

    // Use this for initialization
    void Start()
    {
        timeRemaining = startTime;
    }

    // Update is called once per frame
    void Update()
    {
        //if( !GameObject.FindGameObjectWithTag("Player").GetComponent<playerController>().win)
        timeRemaining -= Time.deltaTime;
        int minutes = (int)timeRemaining / 60;
        int seconds = (int)timeRemaining % 60;

        string textTime = string.Format("{0:00} : {1:00}", minutes, seconds);
        timeDisplay.text = textTime;

        if(timeRemaining < 0)
        {
            buttonScripts.GameOver();
        }
    }
}
