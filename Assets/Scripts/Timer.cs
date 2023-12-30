using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using BackEnd;

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
    int result;
    Text signText;
    PlayerController playerController;
    GameController gameController;
    string userId;
    void Awake()
    {
        result = 0;
        signText = sign.GetComponentInChildren<Text>();
        playerController = FindObjectOfType<PlayerController>();
        gameController = FindObjectOfType<GameController>();
        if (SceneManager.GetActiveScene().name == "MatchingColorScene") sceneNumber = 0;
        else if( SceneManager.GetActiveScene().name == "FindCorrect") sceneNumber = 1;
        time = 60f;
        BackendReturnObject bro = Backend.BMember.GetUserInfo();
        if (bro.IsSuccess())
        {
            userId = bro.GetReturnValuetoJSON()["row"]["inDate"].ToString();
        }
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
                StartCoroutine(ResultOn());
            }

            // �ð��� �ؽ�Ʈ�� ǥ��
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
            result = gameController.score;
            signText.text = "YourScore :" + result.ToString(); 
        }
        if (sceneNumber == 1)
        {
            result = playerController.correct - playerController.wrong;
            signText.text = "YourScore : " + result.ToString();
        }
        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(1f);
        }
        if (userId == "2023-11-09T12:17:20.000Z") PlayerData.GetInstance().Player1Result(result);
        else PlayerData.GetInstance().Player2Result(result);
        PlayerData.GetInstance().Player1CoinUp();
        PlayerData.GetInstance().Player1CoinUp();
        PlayerData.GetInstance().GameDataUpdate();
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
