using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using LitJson;

public class BackendRoomList : MonoBehaviour
{

    private static UserInfoData data = new UserInfoData();
    public static UserInfoData Data => data;

    public Button Confirm, Back;
    public Text UsernameText;
    public GameObject MakeRoom, Title;

    private static BackendRoomList _instance = null;

    public static BackendRoomList Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendRoomList();
            }
            return _instance;
        }
    }

    private void Start()
    {
        GetUserInfoFromBackend();
        UsernameText.text = Data.nickname == null ? Data.gamerId : Data.nickname;
        Backend.Match.Poll();
        Confirm.onClick.AddListener(GetValue);
        Back.onClick.AddListener(GetBack);
    }

    private void GetValue()
    {
        MakeRoom.gameObject.SetActive(true);
    }

    private void GetBack()
    {
        Backend.Match.LeaveMatchMakingServer();
        this.gameObject.SetActive(false);
        MakeRoom.gameObject.SetActive(false);
        Title.gameObject.SetActive(true);
    }

    public void GetUserInfoFromBackend()
    {
        Backend.BMember.GetUserInfo(callback =>
        {
            if (callback.IsSuccess())
            {
                try
                {
                    JsonData json = callback.GetReturnValuetoJSON()["row"];

                    data.gamerId = json["gamerId"].ToString();
                    data.countryCode = json["countryCode"].ToString();
                    data.nickname = json["nickname"].ToString();
                    data.inDate = json["inDate"].ToString();
                    data.emailForFindPassword = json["emailForFindPassword"].ToString();
                    data.subscriptionType = json["subscriptionType"].ToString();
                    data.federationId = json["federationId"].ToString();
                    Debug.Log("데이터 불러오기 성공");
                }
                catch (System.Exception e)
                {
                    data.Reset();
                    Debug.LogError(e);
                }
            }
            else
            {
                data.Reset();
                Debug.LogError(callback.GetMessage());
            }
        });
    }
}

public class UserInfoData
{
    public string gamerId;
    public string countryCode;
    public string nickname;
    public string inDate;
    public string emailForFindPassword;
    public string subscriptionType;
    public string federationId;

    public void Reset()
    {
        gamerId = "Offline";
        countryCode = "Unknown";
        nickname = "Noname";
        inDate = string.Empty;
        emailForFindPassword = string.Empty;
        subscriptionType = string.Empty;
        federationId = string.Empty;
    }
}