using UnityEngine;
using UnityEngine.UI;
using BackEnd;
using BackEnd.Tcp;
using System.Collections.Generic;

public class MatchResultUI : MonoBehaviour
{
    private static MatchResultUI instance;
    public GameObject gameResultObject;
    public GameObject baseResultObject;
    public GameObject meleeResultObject;

    private GameObject endLoadingObject;
    private GameObject returnLobbyObject;
    private FadeAnimation fadeObject;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }

    public static MatchResultUI GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("MatchResultUI 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return instance;
    }

    void Start()
    {
        var backGroundObject = gameResultObject.transform.GetChild(0).gameObject;
        endLoadingObject = backGroundObject.transform.GetChild(1).gameObject;
        returnLobbyObject = backGroundObject.transform.GetChild(4).gameObject;

        gameResultObject.SetActive(false);
        baseResultObject.SetActive(false);
        meleeResultObject.SetActive(false);
        endLoadingObject.SetActive(true);
        returnLobbyObject.SetActive(false);

        var fade = GameObject.FindGameObjectWithTag("Fade");
        if (fade != null)
        {
            fadeObject = fade.GetComponent<FadeAnimation>();
        }

        Debug.Log("MatchResultUI 설정 완료");
    }

    public void SetGameResult(MatchGameResult matchGameResult)
    {
        gameResultObject.SetActive(true);
        endLoadingObject.SetActive(true);
        returnLobbyObject.SetActive(false);
        baseResultObject.SetActive(false);
        meleeResultObject.SetActive(false);

        var matchInstance = BackEndMatchManager.GetInstance();
        if (matchInstance == null)
        {
            returnLobbyObject.SetActive(true);
            return;
        }

        if (matchInstance.nowModeType != MatchModeType.Melee)
        {
            var winData = baseResultObject.transform.GetChild(0).gameObject.GetComponentsInChildren<Text>();
            var loseData = baseResultObject.transform.GetChild(1).gameObject.GetComponentsInChildren<Text>();
            if (winData == null || loseData == null)
            {
                Debug.LogError("Result_Base UI 불러오기 실패");
                return;
            }

            string winner = "";
            string loser = "";
            foreach (var user in matchGameResult.m_winners)
            {
                winner += matchInstance.GetNickNameBySessionId(user) + "\n";
            }

            foreach (var user in matchGameResult.m_losers)
            {
                loser += matchInstance.GetNickNameBySessionId(user) + "\n";
            }

            winData[1].text = winner;
            loseData[1].text = loser;
            Invoke("ShowResultBase", 0.8f);
        }
        else
        {
            var data = meleeResultObject.GetComponentsInChildren<Text>();
            if (data == null)
            {
                Debug.LogError("Result_Melee UI 불러오기 실패");
                return;
            }

            string winner = "";
            foreach (var user in matchGameResult.m_winners)
            {
                winner += matchInstance.GetNickNameBySessionId(user) + "\n\n";
            }

            data[2].text = winner;
            Invoke("ShowResultMelee", 0.8f);
        }
    }

    private void ShowResultBase()
    {
        endLoadingObject.SetActive(false);
        baseResultObject.SetActive(true);
        meleeResultObject.SetActive(false);
        returnLobbyObject.SetActive(true);
    }

    private void ShowResultMelee()
    {
        endLoadingObject.SetActive(false);
        meleeResultObject.SetActive(true);
        baseResultObject.SetActive(false);
        returnLobbyObject.SetActive(true);
    }

    public void ReturnToMatchRobby()
    {
        if (fadeObject != null)
        {
            fadeObject.ProcessFadeOut(() =>
            {
                GameManager.GetInstance().ChangeState(GameManager.GameState.MatchLobby);
            });
        }
        else
        {
            GameManager.GetInstance().ChangeState(GameManager.GameState.MatchLobby);
        }
    }
}
