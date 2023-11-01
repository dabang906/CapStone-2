using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using BackEnd;

public class BackendManager : MonoBehaviour
{
    public Button LoginBtn, RegistBtn, ExitBtn;

    public GameObject Login, Regist;

    private static BackendManager _instance = null;

    public static BackendManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendManager();
            }
            return _instance;
        }
    }

    private void Start()
    {
        var bro = Backend.Initialize(true);

        if (bro.IsSuccess())
        {
            Debug.Log("초기화 성공 : " + bro);
        }
        else
        {
            Debug.LogError("초기화 실패 : " + bro);
        }

        LoginBtn.onClick.AddListener(GameLogin);
        RegistBtn.onClick.AddListener(GameRegist);
        ExitBtn.onClick.AddListener(GameExit);
    }

    private void GameLogin()
    {

        this.gameObject.SetActive(false);
        Login.gameObject.SetActive(true);

    }

    private void GameRegist()
    {
        this.gameObject.SetActive(false);
        Regist.gameObject.SetActive(true);
    }

    private void GameExit()
    {
        Debug.Log("게임 종료!");
        Application.Quit();
    }
}