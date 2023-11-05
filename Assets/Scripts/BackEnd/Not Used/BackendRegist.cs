using System.Collections;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;
using UnityEngine.UI;

public class BackendRegist : MonoBehaviour
{
    public InputField IDText, NicknameText, PWText, PWCheckText;
    public Button Confirm, Back;
    public GameObject Title;

    private static BackendRegist _instance = null;

    public static BackendRegist Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendRegist();
            }
            return _instance;
        }
    }

    private void Start()
    {
        Confirm.onClick.AddListener(GetValue);
        Back.onClick.AddListener(GetBack);
    }

    private void GetValue()
    {
        string id = IDText.text;
        string nickname = NicknameText.text;
        string pw = PWText.text;
        string checkPw = PWCheckText.text;

        if(pw == checkPw)
        {
            CustomRegist(id, pw, nickname);
        }
        else
        {
            ClearText();
        }
    }

    private void GetBack()
    {
        ClearText();
        this.gameObject.SetActive(false);
        Title.gameObject.SetActive(true);
    }

    public void CustomRegist(string id, string pw, string nickname)
    {
        Debug.Log("회원가입을 요청합니둥.");

        var bro = Backend.BMember.CustomSignUp(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("회원가입에 성공했습니둥 : " + bro);
            UpdateNickname(nickname);
            Invoke("ClearText", 2.0f);
            Invoke("GetBack", 2.0f);
        }
        else
        {
            Debug.Log("회원가입에 실패했습니둥. : " + bro);
            ClearText();
        }
    }

    public void UpdateNickname(string nickname)
    {
        Debug.Log("닉네임을 추가합니다.");

        var bro = Backend.BMember.CreateNickname(nickname);

        if (bro.IsSuccess())
        {
            Debug.Log("닉네임 추가 성공 : " + bro);
        }
        else
        {
            Debug.Log("닉네임 추가 실패 : " + bro);
        }
    }

    public void ClearText()
    {
        IDText.text = null;
        NicknameText.text = null;
        PWText.text = null;
        PWCheckText.text = null;
    }
}
