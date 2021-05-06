using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Fade : MonoBehaviour
{
    public static Fade _instance;

    public Image fadeImage;
    public UnityEvent OnFadeOut, OnFadeIn;
    public float fadeTime = .5f;

    private float fadePercentage = 1;
    private bool isFading = false;

    private void Awake()
    {
        _instance = this;
    }

    public void FadeOut()
    {
        if (isFading)
            return;

        isFading = true;
        StartCoroutine(FadeOutLoop());
    }

    public void FadeIn()
    {
        if (isFading)
            return;

        isFading = true;
        StartCoroutine(FadeInLoop());
    }

    private IEnumerator FadeOutLoop()
    {
        while (isFading)
        {
            if (fadePercentage < 1)
                fadePercentage += Time.deltaTime / fadeTime;

            fadeImage.color = new Color(0,0,0,fadePercentage);
            yield return null;

            if (fadePercentage >= 1)
            {
                if (OnFadeOut != null)
                {
                    OnFadeOut.Invoke();
                    OnFadeOut.RemoveAllListeners();
                }

                isFading = false;
            }
        }
    }

    private IEnumerator FadeInLoop()
    {
        while (isFading)
        {
            if (fadePercentage > 0)
                fadePercentage -= Time.deltaTime / fadeTime;

            fadeImage.color = new Color(0, 0, 0, fadePercentage);
            yield return null;

            if (fadePercentage <= 0)
            {
                if (OnFadeIn != null)
                {
                    OnFadeIn.Invoke();
                    OnFadeIn.RemoveAllListeners();
                }

                isFading = false;
            }
        }
    }
}
