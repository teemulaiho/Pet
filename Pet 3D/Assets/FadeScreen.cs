using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
    private Image screen;

    private float fadeTimer = 0f;
    private float fadeDuration = 1.0f;
    private float targetAlpha = 0f;

    private bool fading = false;
    public bool IsFading() { return fading; }

    private void Awake()
    {
        screen = GetComponent<Image>();

        SetAlpha(1.0f);
    }

    private void Update()
    {
        if (fading)
        {
            float alpha = 0f;
            if (fadeTimer < fadeDuration)
            {
                fadeTimer += Time.deltaTime;

                if (targetAlpha == 0f)
                    alpha = 1 - (fadeTimer / fadeDuration);
                else
                    alpha = (fadeTimer / fadeDuration);
            }
            else
            {
                fadeTimer = 0f;
                fading = false;
                alpha = targetAlpha;
            }

            SetAlpha(alpha);
        }
    }

    public void FadeIn()
    {
        SetAlpha(1.0f);

        targetAlpha = 0f;
        fading = true;
    }

    public void FadeOut()
    {
        SetAlpha(0.0f);

        targetAlpha = 1.0f;
        fading = true;
    }

    public void SetAlpha(float alpha)
    {
        Color newColor = screen.color;
        newColor.a = alpha;
        screen.color = newColor;
    }
}
