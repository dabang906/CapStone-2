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
    public GameObject sign;

    private float time;
    private float startTime;
    private float elapsedTime;
    int sceneNumber;
    Text signText;
    PlayerController playerController;
    GameController gameController;
    void Awake()
    {
        signText = sign.GetComponentInChildren<Text>();
        playerController = FindObjectOfType<PlayerController>();
        gameController = FindObjectOfType<GameController>();
        if (SceneManager.GetActiveScene().name == "MatchingColorScene") sceneNumber = 0;
        else if( SceneManager.GetActiveScene().name == "FindCorrect") sceneNumber = 1;
        time = 60f;
    }
    // Start is called before the first frame update
    void Start()
    {
        ResetTimer();
        StartCoroutine("ExplanationTime");
    }

    // Update is called once per frame
    void Update()
    {
        if (timerRunning)
        {
            // 경과 시간 계산
            elapsedTime = Time.time - startTime;
            // time에서 경과 시간을 뺌
            float remainingTime = time - elapsedTime;
            if (remainingTime <= 0)
            {
                // 시간이 다 되면 타이머 정지
                remainingTime = 0;
                timerRunning = false;
                displayCount.fontSize = 30;
                displayCount.text = "";
                StartCoroutine(ResultOn());
            }

            // 시간을 텍스트로 표시
            timerText.text = "Time : " + FormatTime(remainingTime);
        }
    }

    IEnumerator ExplanationTime()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(1f);
        }
        sign.SetActive(false);
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

    IEnumerator ResultOn()
    {
        sign.SetActive(true);
        if(sceneNumber == 0)
        {
            signText.text = "YourScore :" + gameController.score.ToString(); 
        }
        if (sceneNumber == 1)
        {
            signText.text = "YourScore : " + (playerController.correct - playerController.wrong).ToString();
        }
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1f);
        }
        SceneManager.LoadScene("MainScene");
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
