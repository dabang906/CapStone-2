using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public Text displayCount;
    public Text timerText;
    public bool timerRunning = false;
    private float time;
    private float startTime;
    private float elapsedTime;

    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "BoxHit") time = 10f;
        else if (SceneManager.GetActiveScene().name == "EatNumber") time = 30f;
        else if (SceneManager.GetActiveScene().name == "MatchingColorScene" ||
            SceneManager.GetActiveScene().name == "FindCorrect") time = 60f;
    }
    // Start is called before the first frame update
    void Start()
    {
        ResetTimer();
        StartCoroutine("DisplayCount");
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            // ��� �ð� ���
            elapsedTime = Time.time - startTime;
            // time���� ��� �ð��� ��
            float remainingTime = time - elapsedTime;
            if (remainingTime <= 0)
            {
                // �ð��� �� �Ǹ� Ÿ�̸� ����
                remainingTime = 0;
                timerRunning = false;
                displayCount.fontSize = 30;
                displayCount.text = "";
                SceneManager.LoadScene("MainScene");
            }

            // �ð��� �ؽ�Ʈ�� ǥ��
            timerText.text = "Time : " + FormatTime(remainingTime);
        }
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
    }
    public void ResetTimer()
    {
        startTime = 0f;
        elapsedTime = 0f;
        timerRunning = false;
        timerText.text = "Time : " + FormatTime(time);
    }
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
