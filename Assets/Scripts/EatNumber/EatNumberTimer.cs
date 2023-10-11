using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EatNumberTimer : MonoBehaviour
{
    public Text displayCount;
    public Text timerText;
    public Text text;
    public bool timerRunning = false;
    private float time = 30.0f;
    private float startTime;
    private float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        int rand1 = Random.Range(41, 100);

        ResetTimer();

        Invoke("DisplayRand2", 1.0f);

        Invoke("DisplayCount3", 2.0f);
        Invoke("DisplayCount2", 3.0f);
        Invoke("DisplayCount1", 4.0f);
        Invoke("DisplayReset", 5.0f);

        Invoke("StartTimer", 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            // 경과 시간 계산
            elapsedTime = Time.time - startTime;

            // 1분(60초)에서 경과 시간을 뺌
            float remainingTime = time - elapsedTime;

            if (remainingTime <= 0)
            {
                // 시간이 다 되면 타이머 정지
                remainingTime = 0;
                timerRunning = false;
                displayCount.fontSize = 30;
                displayCount.text = "";
                //SceneManager.LoadScene("MainScene");
            }

            // 시간을 텍스트로 표시
            timerText.text = "Time : " + FormatTime(remainingTime);
        }
    }

    void DisplayRand2()
    {
        //int rand2 = Random.Range(3, 8);
        //mul = rand2;
        //text.text = mul.ToString();
    }

    void DisplayCount3()
    {
        displayCount.text = "3";
    }
    void DisplayCount2()
    {
        displayCount.text = "2";
    }
    void DisplayCount1()
    {
        displayCount.text = "1";
    }
    void DisplayReset()
    {
        displayCount.text = "";
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
