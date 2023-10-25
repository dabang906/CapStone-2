using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BackEnd;

public class BackendLogin
{
    private static BackendLogin _instance = null;

    public static BackendLogin Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new BackendLogin();
            }
            return _instance;
        }
    }

    public void CustomSignUp(string id, string pw)
    {
        Debug.Log("회원가입을 요청합니둥.");

        var bro = Backend.BMember.CustomSignUp(id, pw);

        if(bro.IsSuccess())
        {
            Debug.Log("회원가입에 성공했습니둥 : " + bro);
        }
        else
        {
            Debug.Log("회원가입에 실패했습니둥. : " + bro);
        }
    }

    public void CustomLogin(string id, string pw)
    {
        Debug.Log("로그인을 요청합니둥.");

        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("로그인에 성공했습니둥 : " + bro);
        }
        else
        {
            Debug.Log("로그인에 실패했습니둥. : " + bro);
        }
    }

    public void UpdateNickname(string nickname)
    {
        Debug.Log("닉네임 변경을 요청합니둥.");

        var bro = Backend.BMember.UpdateNickname(nickname);

        if (bro.IsSuccess())
        {
            Debug.Log("닉네임 변경에 성공했습니둥 : " + bro);
        }
        else
        {
            Debug.Log("닉네임 변경에 실패했습니둥. : " + bro);
        }
    }

    public void UpdateCountry()
    {
        Debug.Log("국가 코드를 변경합니둥.");

        var bro = Backend.BMember.UpdateCountryCode(BackEnd.GlobalSupport.CountryCode.SouthKorea);

        if (bro.IsSuccess())
        {
            Debug.Log("국가 코드 변경에 성공했습니둥 : " + bro);
        }
        else
        {
            Debug.Log("국가 코드 변경에 실패했습니둥. : " + bro);
        }
    }
}
