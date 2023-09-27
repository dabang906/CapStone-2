using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffect : MonoBehaviour
{
    [HideInInspector]
    public int randomLucky;

    public Text luckyText;
    CanvasGroup canvasGroup;
    string[] luckyString = { "더블 주사위", "골드 더블 주사위", "상점 열기", "하나뭐였지?" };
    void OnEnable()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        randomLucky = Random.Range(0, 4);
        luckyText.text = luckyString[randomLucky];
        StartCoroutine(FadeInStart());
    }

    public IEnumerator FadeInStart()
    {
        for (float f = 0f; f < 1; f += 0.005f)
        {
            canvasGroup.alpha = f;
            yield return null;
        }
        yield return new WaitForSecondsRealtime(3);
        StartCoroutine(FadeOutStart());
        yield break;
    }

    public IEnumerator FadeOutStart()
    {
        for (float f = 1f; f > 0; f -= 0.02f)
        {
            canvasGroup.alpha = f;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        canvasGroup.alpha = 0;
        this.gameObject.SetActive(false);
        yield break;
    }
}
