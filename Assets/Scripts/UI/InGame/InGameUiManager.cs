using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InGameUiManager : MonoBehaviour
{
    static private InGameUiManager instance;
    public Text scoreBoard;
    public GameObject startCountObject;
    public GameObject reconnectBoardObject;

    private Text startCountText;
    private Text reconnectBoardText;
    const string HostOfflineMsg = "호스트와의 연결이 끊어졌습니다.\n연결 대기중";
    const string PlayerReconnectMsg = "{0} 플레이어 재접속중...";

    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;

        startCountText = startCountObject.GetComponentInChildren<Text>();
        startCountObject.SetActive(true);
        reconnectBoardText = reconnectBoardObject.GetComponentInChildren<Text>();
        Debug.Log("인게임 UI 설정 완료");
    }

    public static InGameUiManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("InGameUiManager 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return instance;
    }

    public void SetScoreBoard(int score)
    {
        scoreBoard.text = "Remain Player : " + score;
    }

    public void SetStartCount(int time, bool isEnable = true)
    {
        startCountObject.SetActive(isEnable);
        if (isEnable)
        {
            if (time == 0)
            {
                startCountText.text = "Game Start!";
            }
            else
            {
                startCountText.text = string.Format("{0}", time);
            }
        }
    }

    public void SetHostWaitBoard()
    {
        reconnectBoardText.text = HostOfflineMsg;
        reconnectBoardObject.SetActive(true);
        // 4초 후 재접속 메시지 닫음
        Invoke("ReconnectBoardClose", 4.0f);
    }

    public void SetReconnectBoard(string playerName)
    {
        reconnectBoardText.text = string.Format(PlayerReconnectMsg, playerName);
        reconnectBoardObject.SetActive(true);
        // 4초 후 재접속 메시지 닫음
        Invoke("ReconnectBoardClose", 4.0f);
    }

    private void ReconnectBoardClose()
    {
        reconnectBoardObject.SetActive(false);
    }
}
