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
    public Text comboMultiText;
    public Text comboTimerText;
    public Text bonusPrefab;

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
        comboUI.GetComponent<Image>().fillAmount =  time / startTime;
        comboMultiText.text = multiplier.ToString();
        comboTimerText.text = ((int)time).ToString();
    }

    public void comboStart()
    {
        comboUI.SetActive(true);
    }

    public void comboEnd(int multiplier)
    {
        GameObject.FindGameObjectWithTag("HUD").GetComponent<timerScript>().addTime(3 * multiplier);
        Text bonustext = Instantiate(bonusPrefab, new Vector3(233, 55, 0), GameObject.FindGameObjectWithTag("HUD").transform.rotation) as Text;
        bonustext.transform.SetParent(this.transform, false);
        bonustext.fontSize = 15 * multiplier;
        bonustext.text = (3 * multiplier).ToString() + " seconds added!";
        comboUI.SetActive(false);
        //comboUI.AddComponent<TextFade>();
        //comboUI.GetComponent<TextFade>().CanvasGroup = true;
    }
}