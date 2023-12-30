using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd;
public class ResultText : MonoBehaviour
{
    Text resulttext;
    int player1;
    int player2;
    string userId;
    void Start()
    {
        resulttext = GetComponent<Text>();

        PlayerData.GetInstance().GameDataUpdate();
        player1 = PlayerData.GetInstance().GameDataGet("player1coin");
        player2 = PlayerData.GetInstance().GameDataGet("player2coin");
        BackendReturnObject bro = Backend.BMember.GetUserInfo();
        /*
        if (player1 > player2) resulttext.text = "Player1 Win!";
        if (player2 > player1) resulttext.text = "Player2 Win!";
        if (player1 == player2) resulttext.text = "DRAW!";
        */

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
    }
}
