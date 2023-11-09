using System;
using System.Collections;
using UnityEngine;

public class FadeAnimation : MonoBehaviour
{
    public GameObject fadeInObject;
    public GameObject fadeOutObject;
    void Awake()
    {
        fadeInObject.SetActive(false);
        fadeOutObject.SetActive(false);

        StartCoroutine("FadeInAnimation");

        Debug.Log("FadeAnimation 설정 완료");
    }

    private IEnumerator FadeInAnimation()
    {
        var animation = fadeInObject.GetComponent<Animator>();
        fadeInObject.SetActive(true);

        while (animation.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        fadeInObject.SetActive(false);
    }

    private IEnumerator FadeOutAnimation(Action func = null)
    {
        var animation = fadeOutObject.GetComponent<Animator>();
        fadeOutObject.SetActive(true);

        while (animation.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }

        if (func != null)
        {
            func();
        }
    }

    public void ProcessFadeOut(Action func)
    {
        StartCoroutine("FadeOutAnimation", func);
    }

    public void ProcessFadeOut()
    {
        fadeOutObject.SetActive(true);
    }
}
