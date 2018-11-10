using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScripts : MonoBehaviour {

    public GameObject pauseUI;
    public GameObject gameOverUI;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }

    public void GameOver()
    {
        pauseUI.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameOverUI.SetActive(true);
    }
}
