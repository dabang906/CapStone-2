using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // SceneManager 사용을 위한 네임스페이스 추가


public class BoxHitCollisionEvent : MonoBehaviour
{
    public Text displayCount;
    public Text timerText;
    public Text text;
    public GameObject explanation;
    public bool timerRunning = false;

    private float startTime;
    private float elapsedTime;
    
    int count;
    int mul;

    void Start()
    {
        ResetTimer();

        int rand1 = Random.Range(41, 100);

        count = rand1;
        text.text = count.ToString();

        StartCoroutine(ExplanationTime());
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            count = count - mul;
        }
    }

    private void Update()
    {
        if (timerRunning)
        {
            // 경과 시간 계산
            elapsedTime = Time.time - startTime;

            // 1분(60초)에서 경과 시간을 뺌
            float remainingTime = 10.0f - elapsedTime;

            if (remainingTime <= 0)
            {
                // 시간이 다 되면 타이머 정지
                remainingTime = 0;
                timerRunning = false;
                displayCount.fontSize = 30;
                displayCount.text = "남은 횟수 : " + count.ToString();
                Invoke("LoadScene",3f);
            }
            // 시간을 텍스트로 표시
            timerText.text = "Time : " + FormatTime(remainingTime);
        }
    }

    void DisplayRand2()
    {
        int rand2 = Random.Range(3, 8);
        mul = rand2;
        text.text = mul.ToString();
    }

    IEnumerator ExplanationTime()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(1f);
        }
        explanation.SetActive(false);
        StartCoroutine(DisplayCount());
    }
    IEnumerator DisplayCount()
    {
        for (int i = 3; i > 0; i--)
        {
            displayCount.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        displayCount.text = "";
        StartTimer();
        yield return null;
    }

    public void StartTimer()
    {
        startTime = Time.time;
        timerRunning = true;
        DisplayRand2();
    }

    public void ResetTimer()
    {
        startTime = 0f;
        elapsedTime = 0f;
        timerRunning = false;
        timerText.text = "Time : " + FormatTime(10.0f);
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void LoadScene()
    {
        SceneManager.LoadScene("MainScene");
    }
}
