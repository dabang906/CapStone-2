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
    string[] luckyString = { "주사위 2배", "모두에게 코인 +3", "상점 열기", "하나뭐였지?" };
    Stone stone;
    void OnEnable()
    {
        stone = FindObjectOfType<Stone>();
        canvasGroup = GetComponent<CanvasGroup>();
        randomLucky = Random.Range(0, 4);
        switch (randomLucky)
        {
            case 0:
                stone.diceState = Stone.GameState.DicePlus; break;
            case 1:
                stone.diceState = Stone.GameState.AllUp; break;
            case 2:
                stone.diceState = Stone.GameState.OpenShop; break;
            default:
                break;
        }
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
        yield return new WaitForSecondsRealtime(2f);
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
