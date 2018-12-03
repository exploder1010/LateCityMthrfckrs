using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFade : MonoBehaviour
{

    public float fadeTime;
    public float fadeGroupSpeed;
    public bool CanvasGroup;

    // Use this for initialization
    void Start()
    {
        if(CanvasGroup == false)
        {
            FadeOut();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CanvasGroup == true)
        {
            FadeOutGroup();
        }
    }

    //Fade time in seconds
    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }
    private IEnumerator FadeOutRoutine()
    {
        Text text = GetComponent<Text>();
        Color originalColor = text.color;
        for (float t = 0.01f; t < fadeTime; t += Time.deltaTime)
        {
            text.color = Color.Lerp(originalColor, Color.clear, Mathf.Min(1, t / fadeTime));
            yield return null;
        }
    }

    public void FadeOutGroup()
    {
        CanvasGroup canvas = this.GetComponent<CanvasGroup>();
        canvas.alpha -= fadeGroupSpeed * Time.deltaTime;
        if(canvas.alpha <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }
}