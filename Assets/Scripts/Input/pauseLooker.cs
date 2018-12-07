using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Luminosity.IO
{
    public class pauseLooker : MonoBehaviour
    {
        float prevTime;
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
            if (gameOverMenu.activeSelf == false && winMenu.activeSelf == false)
            {
                if (InputManager.GetButtonDown("Pause"))
                {
                    Pause();
                }
            }

            if (InputManager.GetButtonDown("Retry"))
            {
                SoundScript.PlaySound(uiSource, "UI Click");
                GameController.instance.resetScene();
            }
            
        }

        public void Pause()
        {
            SoundScript.PlaySound(uiSource, "UI Click");

            if (pauseMenu.activeSelf == true)
            {
                Time.timeScale = prevTime;
                pauseMenu.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            else if (pauseMenu.activeSelf == false)
            {
                prevTime = Time.timeScale;
                Time.timeScale = 0;
                pauseMenu.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}

