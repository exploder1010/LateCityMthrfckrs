using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonScripts : MonoBehaviour {

    public GameObject pauseUI;
    public GameObject gameOverUI;
    public GameObject winUI;
    public GameObject comboUI;
    public GameObject comboCircle;
    public Text comboMultiText;
    public Text comboTimerText;
    public Text bonusPrefab;
    public int scoreMultiplier = 1000;

	// Use this for initialization
	void Start () {
        comboUI.SetActive(false);
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

    public void Win()
    {
        pauseUI.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        winUI.SetActive(true);
        gameOverUI.SetActive(false);
    }

    public void comboUpdate(float time, float startTime, int multiplier)
    {
        comboCircle.GetComponent<Image>().fillAmount =  time / startTime;
        comboMultiText.text = (multiplier * scoreMultiplier).ToString() + " X COMBO!";
        comboMultiText.fontSize = (int)(45f * (1f + (multiplier * 0.3f)));
        comboTimerText.text = ((int)time).ToString() + " S";
    }

    public void comboStart()
    {
        comboUI.SetActive(true);
    }

    public void comboEnd(int multiplier)
    {
        if(multiplier > 0)
        {
            //print(multiplier);
            GameObject.FindGameObjectWithTag("HUD").GetComponent<timerScript>().addTime(multiplier);
            Text bonustext = Instantiate(bonusPrefab, bonusPrefab.rectTransform.position, GameObject.FindGameObjectWithTag("HUD").transform.rotation) as Text;
            bonustext.transform.SetParent(this.transform, false);
            bonustext.fontSize = (int)(45f * (1f + (multiplier * 0.4f)));
            bonustext.text = "TIME EXTENDED!!! " + (multiplier).ToString() + "s";
            //comboUI.AddComponent<TextFade>();
            //comboUI.GetComponent<TextFade>().CanvasGroup = true;
        }

        comboUI.SetActive(false);
    }
}