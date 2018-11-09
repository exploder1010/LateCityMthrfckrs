using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pauseLooker : MonoBehaviour {

    public GameObject pauseMenu;
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
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
}

