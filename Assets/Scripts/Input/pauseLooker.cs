using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseLooker : MonoBehaviour {

    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject gameOverMenu;
    public AudioSource uiSource;

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameOverMenu.activeSelf == false && winMenu.activeSelf == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //print("hello");
                SoundScript.PlaySound(uiSource, "UI Click");

                if (pauseMenu.activeSelf == true)
                {
                    pauseMenu.SetActive(false);
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }

                else if (pauseMenu.activeSelf == false)
                {
                    pauseMenu.SetActive(true);
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.None;
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            SoundScript.PlaySound(uiSource, "UI Click");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}

