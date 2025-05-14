using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;


public class SecretArea : MonoBehaviour
{
    public float fadeDuration = 1f;

    SpriteRenderer spriteRenderer;
    Color hiddenColor;
    Coroutine currentCoroutine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        hiddenColor = spriteRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(currentCoroutine != null)    //makes sure only one is running at the same time if the character happens to enter and exit before the color change happens
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(FadeSprite(true));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (currentCoroutine != null)    //makes sure only one is running at the same time if the character happens to enter and exit before the color change happens
            {
                StopCoroutine(currentCoroutine);
            }
            currentCoroutine = StartCoroutine(FadeSprite(false));
        }
    }

    private IEnumerator FadeSprite(bool fadeOut)
    {
        Color startColor = spriteRenderer.color;
        Color targetColor = fadeOut ? new Color(hiddenColor.r, hiddenColor.g, hiddenColor.b, 0f) : hiddenColor;
        float timeFading = 0f;

        while (timeFading < fadeDuration)
        {
            spriteRenderer.color = Color.Lerp(startColor, targetColor, timeFading / fadeDuration);
            timeFading += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = targetColor;
    }
}
