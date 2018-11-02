using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timerScript : MonoBehaviour {

    public float startTime;
    private float timeRemaining;
    private Text timeDisplay;
    public GameObject gameOverUI;
    public GameObject pauseUI;

    // Use this for initialization
    void Start()
    {
        timeDisplay = GameObject.Find("Time").GetComponent<Text>();
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
            pauseUI.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            gameOverUI.SetActive(true);
        }
    }
}
