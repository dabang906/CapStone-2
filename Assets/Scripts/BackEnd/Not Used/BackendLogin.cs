using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using BackEnd;
using BackEnd.Tcp;

public class BackendLogin : MonoBehaviour
{
    public InputField IDText, PWText;
    public Button Confirm, Back;
    public GameObject RoomList, Title;

    private static BackendLogin _instance = null;

    public static BackendLogin Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendLogin();
            }
            return _instance;
        }
    }

    private void Start()
    {
        Backend.Match.Poll();
        Confirm.onClick.AddListener(GetValue);
        Back.onClick.AddListener(GetBack);
    }

    private void GetValue()
    {
        string id = IDText.text;
        string pw = PWText.text;

        CustomLogin(id, pw);
    }

    private void GetBack()
    {
        ClearText();
        this.gameObject.SetActive(false);
        Title.gameObject.SetActive(true);
    }

    public void CustomLogin(string id, string pw)
    {
        Debug.Log("로그인을 요청합니둥.");

        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("로그인에 성공했습니둥 : " + bro);
            ClearText();
            this.gameObject.SetActive(false);
            RoomList.gameObject.SetActive(true);
            ErrorInfo errorInfo;
            Backend.Match.JoinMatchMakingServer(out errorInfo);
            Debug.Log(errorInfo);
        }
        else
        {
            Debug.Log("로그인에 실패했습니둥. : " + bro);
            ClearText();
        }
    }

    public void ClearText()
    {
        IDText.text = null;
        PWText.text = null;
    }
}
