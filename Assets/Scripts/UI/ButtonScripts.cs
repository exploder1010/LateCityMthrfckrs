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

    public Text winText;
    public Text loseText;
    float winResize = 45;

    public Text bonusPrefab;
    public int scoreMultiplier = 1000;

	// Use this for initialization
	void Start () {
        if(comboUI)
        {
            comboUI.SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(winText!= null && loseText != null)
        {
            if (winText.gameObject.activeSelf || loseText.gameObject.activeSelf)
            {
                winResize += 360 * Time.deltaTime;
                loseText.fontSize = (int)winResize;
                winText.fontSize = (int)winResize;
            }
        }
		
	}

    public void Retry()
    {
        winText.gameObject.SetActive(false);
        loseText.gameObject.SetActive(false);
        pauseUI.SetActive(false);
        StopAllCoroutines();
        GameController.instance.resetScene();
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
        //pauseUI.SetActive(false);
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;
        //gameOverUI.SetActive(true);
    }

    public void Win(bool won)
    {
        if (winText != null && loseText != null)
        {
            if (won)
            {
                winText.gameObject.SetActive(true);
            }
            else
            {
                loseText.gameObject.SetActive(true);
            }
        }
      
        StartCoroutine(WinCountdown());
    }

    IEnumerator WinCountdown()
    {
        
        yield return new WaitForSeconds(3f);

        if (loseText.isActiveAndEnabled)
        {
            ResetThis();
            Retry();
        }
        else
        {
            ResetThis();
            pauseUI.SetActive(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            winUI.SetActive(true);
            winUI.SendMessage("ShowLeaderboard");
            winUI.SendMessage("CheckTime");
            gameOverUI.SetActive(false);
        }
           
        
        
    }

    public void ResetThis()
    {
        if (winText != null && loseText != null)
        {
            winText.gameObject.SetActive(false);
            loseText.gameObject.SetActive(false);

            winResize = 5;

            comboEnd(0);
        }
    }

    public void comboUpdate(float time, float startTime, int multiplier)
    {
        comboCircle.GetComponent<Image>().fillAmount =  time / startTime;
        comboMultiText.text = (multiplier * scoreMultiplier).ToString() + " X COMBO!";
        comboMultiText.fontSize = (int)(45f * (1f + (multiplier * 0.2f)));
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
            print(multiplier);
            GameObject.FindGameObjectWithTag("HUD").GetComponent<timerScript>().addTime(multiplier);
            Text bonustext = Instantiate(bonusPrefab, bonusPrefab.rectTransform.position, GameObject.FindGameObjectWithTag("HUD").transform.rotation) as Text;
            bonustext.transform.SetParent(this.transform, false);
            bonustext.fontSize = (int)(30f * (1f + (multiplier * 0.3f)));
            bonustext.text = "TIME EXTENDED!!! " + (multiplier).ToString() + "s";
            //comboUI.AddComponent<TextFade>();
            //comboUI.GetComponent<TextFade>().CanvasGroup = true;
        }

        comboUI.SetActive(false);
    }
}