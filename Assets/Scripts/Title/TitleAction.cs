using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleAction : MonoBehaviour
{
    public Button LoginBtn, RegistBtn, ShowRankBtn, ExitBtn;

    private void Start()
    {
        LoginBtn.onClick.AddListener(Login);
        RegistBtn.onClick.AddListener(Regist);
        ShowRankBtn.onClick.AddListener(ShowRank);
        ExitBtn.onClick.AddListener(GameExit);
    }

    private void Login()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }

    private void Regist()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("RegistScene");
    }

    private void ShowRank()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("ShowRankScene");
    }

    private void GameExit()
    {
        Debug.Log("게임 종료!");
        Application.Quit();
    }
}
