using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class InGameUiManager : MonoBehaviourPun
{
    static private InGameUiManager instance;
    public Text scoreBoard;
    public GameObject startCountObject;
    public Text turnText;
    public Text player1coinText;
    public Text player2coinText;
    public GameObject luckyImage;
    //    public GameObject reconnectBoardObject;

    private Text startCountText;
    private Text reconnectBoardText;
    public int count;
    const string HostOfflineMsg = "호스트와의 연결이 끊어졌습니다.\n연결 대기중";
    const string PlayerReconnectMsg = "{0} 플레이어 재접속중...";
    List<int> numbers = new List<int>();

    int player1result = 0;
    int player2result = 0;
    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
        count = 0;
        PlayerData.GetInstance().GameDataUpdate();
        startCountText = startCountObject.GetComponent<Text>();
        startCountObject.SetActive(true);
//        reconnectBoardText = reconnectBoardObject.GetComponentInChildren<Text>();
        Debug.Log("인게임 UI 설정 완료");
        Debug.Log(PlayerData.GetInstance().GameDataGet("player1coin").ToString());
        Debug.Log(PlayerData.GetInstance().GameDataGet("turn").ToString());
        player1coinText.text = "Coin : " + PlayerData.GetInstance().GameDataGet("player1coin").ToString();
        player2coinText.text = "Coin : " + PlayerData.GetInstance().GameDataGet("player2coin").ToString();
        turnText.text = "Turn : " + PlayerData.GetInstance().GameDataGet("turn").ToString();
        RandNum();
        if((int)PlayerData.GetInstance().GameDataGet("turn") != 4)
        {
            UpdateReuslt();
        }
        if((int)PlayerData.GetInstance().GameDataGet("turn") < 0)
        {
            PlayerData.GetInstance().GameDataUpdate();
            SceneManager.LoadScene("ResultScene");
        }
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
        //scoreBoard.text = "Remain Player : " + score;
    }

    public void SetStartCount(int time, bool isEnable = true)
    {/*
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
        }*/
    }

    public void SetHostWaitBoard()
    {
        reconnectBoardText.text = HostOfflineMsg;
//        reconnectBoardObject.SetActive(true);
        // 4초 후 재접속 메시지 닫음
        Invoke("ReconnectBoardClose", 4.0f);
    }

    public void SetReconnectBoard(string playerName)
    {
        reconnectBoardText.text = string.Format(PlayerReconnectMsg, playerName);
//        reconnectBoardObject.SetActive(true);
        // 4초 후 재접속 메시지 닫음
        Invoke("ReconnectBoardClose", 4.0f);
    }

    private void ReconnectBoardClose()
    {
        //reconnectBoardObject.SetActive(false);
    }
    public void OnLucky()
    {
        luckyImage.SetActive(true);
    }
    public void player1CoinTextUpdate(int coin)
    {
        player1coinText.text = "Coin : " + coin.ToString();
    }
    public void player2CoinTextUpdate(int coin)
    {
        player2coinText.text = "Coin : " + coin.ToString();
    }
    [PunRPC]
    public void CountUpdate()
    {
        count++;
        if (count % 2 == 0)
        {
            PlayerData.GetInstance().TurnDown();
            PlayerData.GetInstance().GameDataUpdate();
            //PhotonNetwork.LoadLevel(numbers[Random.Range(0, 4)]);
            PhotonNetwork.LoadLevel(PlayerData.GetInstance().GameDataGet("turn") % 4 + 4);
        }
    }
    [PunRPC]
    public void ResultCoin()
    {
        player1result = (int)PlayerData.GetInstance().GameDataGet("player1result");
        player2result = (int)PlayerData.GetInstance().GameDataGet("player2result");
        Debug.Log(player1result + " " + player2result);
        if (player1result > player2result) PlayerData.GetInstance().Player1CoinUp();
        else if (player1result < player2result) PlayerData.GetInstance().Player2CoinUp();
        else if (player1result == player2result)
        {
            PlayerData.GetInstance().Player1CoinUp(); PlayerData.GetInstance().Player2CoinUp();
        }
    }
    public void UpdateReuslt()
    {
        photonView.RPC("ResultCoin", RpcTarget.All);
    }
    public void UpdateCount()
    {
        photonView.RPC("CountUpdate", RpcTarget.All);
    }
    void RandNum()
    {
        while (numbers.Count < 3)
        {
            int randomNumber = UnityEngine.Random.Range(4, 8);

            if (!numbers.Contains(randomNumber))
            {
                numbers.Add(randomNumber);
            }
        }
    }
}
