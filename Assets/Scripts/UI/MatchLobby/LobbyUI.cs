using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BackEnd.Tcp;
using Battlehub.Dispatcher;

public partial class LobbyUI : MonoBehaviour
{
    private static LobbyUI instance;

    public GameObject nickNameObject;
    public GameObject errorObject;
    public GameObject reconnectObject;
    public GameObject selectObject;

    public GameObject modelObject;
    public GameObject requestProgressObject;
    public GameObject matchDoneObject;

    public GameObject recordObject;
    public Image[] recordChart = new Image[2];
    public Text[] recordContent = new Text[7];

    public ToggleGroup tabObject;
    public Text matchInfoText;

    public GameObject friendObject;

    private Toggle meleeToggle;
    private Text errorText;
    private GameObject loadingObject;
    private FadeAnimation fadeObject;
    private TabUI[] matchInfotabList;
    private TabUI[] matchRecordTabList;

    private Coroutine fillWinChart = null;
    private Coroutine fillLoseChart = null;
    private Coroutine countingWinRate = null;

    private Button selectOkBtn = null;
    private Button selectCancelBtn = null;
    private Text selectMsg = null;

    const string matchInfoStr = "매칭 인원 : {0} 명\n\n샌드박스 매칭 여부 : {1}\n\n매칭타입은 {2} 조건으로 유저를 매칭합니다.\n\n매칭모드는 {3} 대전을 실시합니다.";

    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;

        // 재접속 로직 제외
        BackEndMatchManager.GetInstance().IsMatchGameActivate();
    }

    public static LobbyUI GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("LobbyUI 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return instance;
    }

    void Start()
    {
        if (BackEndMatchManager.GetInstance() != null)
        {
            SetNickName();
        }

        errorText = errorObject.GetComponentInChildren<Text>();

        loadingObject = GameObject.FindGameObjectWithTag("Loading");

        selectMsg = selectObject.GetComponentInChildren<Text>();
        selectOkBtn = selectObject.GetComponentInChildren<Button>();
        selectCancelBtn = selectObject.GetComponentsInChildren<Button>()[1];

        var fade = GameObject.FindGameObjectWithTag("Fade");
        if (fade != null)
        {
            fadeObject = fade.GetComponent<FadeAnimation>();
        }

        recordObject.SetActive(true);

        matchInfotabList = tabObject.GetComponentsInChildren<TabUI>();
        matchRecordTabList = recordObject.GetComponentsInChildren<TabUI>();
        int index = 0;
        foreach (var info in BackEndMatchManager.GetInstance().matchInfos)
        {
            matchInfotabList[index].SetTabText(info.title);
            matchInfotabList[index].index = index;
            matchRecordTabList[index].SetTabText(info.title);
            matchRecordTabList[index].index = index;
            index += 1;
        }

        for (int i = BackEndMatchManager.GetInstance().matchInfos.Count; i < matchInfotabList.Length; ++i)
        {
            matchInfotabList[i].gameObject.SetActive(false);
            matchRecordTabList[i].gameObject.SetActive(false);
        }

        recordObject.SetActive(false);
        modelObject.SetActive(true);
        errorObject.SetActive(false);
        requestProgressObject.SetActive(false);
        matchDoneObject.SetActive(false);
        reconnectObject.SetActive(false);
        friendObject.SetActive(false);
        loadingObject.SetActive(false);
        selectObject.SetActive(false);
        readyRoomObject.SetActive(false);
        ChangeTab();
    }

    private void SetNickName()
    {
        var name = BackEndServerManager.GetInstance().myNickName;
        if (name.Equals(string.Empty))
        {
            Debug.LogError("닉네임 불러오기 실패");
            name = "test123";
        }
        Text nickname = nickNameObject.GetComponent<Text>();
        RectTransform rect = nickNameObject.GetComponent<RectTransform>();

        nickname.text = name;
        rect.sizeDelta = new Vector2(nickname.preferredWidth, nickname.preferredHeight);
    }

    public void RequestCancel()
    {
        if (loadingObject.activeSelf || errorObject.activeSelf || matchDoneObject.activeSelf)
        {
            return;
        }
        BackEndMatchManager.GetInstance().CancelRegistMatchMaking();
    }

    public void MatchRequestCallback(bool result)
    {
        if (!result)
        {
            requestProgressObject.SetActive(false);
            modelObject.SetActive(true);
            return;
        }

        requestProgressObject.SetActive(true);
        modelObject.SetActive(false);
    }

    public void MatchDoneCallback()
    {
        requestProgressObject.SetActive(false);
        matchDoneObject.SetActive(true);
    }

    public void MatchCancelCallback()
    {
        requestProgressObject.SetActive(false);
        matchDoneObject.SetActive(false);
    }

    public void ChangeRoomLoadScene()
    {
        if (fadeObject != null)
        {
            fadeObject.ProcessFadeOut(() =>
            {
                GameManager.GetInstance().ChangeState(GameManager.GameState.Ready);
            });
        }
        else
        {
            GameManager.GetInstance().ChangeState(GameManager.GameState.Ready);
        }
    }

    public void OpenGameRecord()
    {
        if (loadingObject.activeSelf || errorObject.activeSelf || requestProgressObject.activeSelf || matchDoneObject.activeSelf || reconnectObject.activeSelf)
        {
            return;
        }
        GetGameRecord();
    }

    public void GetGameRecord()
    {
        loadingObject.SetActive(true);

        int index = -1;
        foreach (var tab in matchRecordTabList)
        {
            if (tab.IsOn() == true)
            {
                index = tab.index;
                break;
            }
        }

        if (index < 0)
        {
            Debug.Log("활성화된 탭이 없습니다.");
            return;
        }

        BackEndMatchManager.GetInstance().GetMyMatchRecord(index, (MatchRecord record, bool isSuccess) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                loadingObject.SetActive(false);
                recordObject.SetActive(true);

                if (fillWinChart != null)
                {
                    StopCoroutine(fillWinChart);
                }
                if (fillLoseChart != null)
                {
                    StopCoroutine(fillLoseChart);
                }
                if (countingWinRate != null)
                {
                    StopCoroutine(countingWinRate);
                }

                recordContent[0].text = record.matchTitle;
                recordContent[2].text = record.matchType.ToString();
                recordContent[3].text = record.modeType.ToString();

                recordContent[1].text = "0%";
                recordContent[4].text = "-";
                recordContent[5].text = "0";
                recordContent[6].text = "0";

                recordChart[0].fillAmount = (float)0;
                recordChart[1].fillAmount = 0;

                if (isSuccess == false)
                {
                    // 조회 실패
                    SetErrorObject("매칭 기록 조회에 실패하였습니다.\n\n잠시 후 다시 시도해주세요.");
                    return;
                }

                if (record.win == -1)
                {
                    // 매칭 기록이 없음
                    SetErrorObject("매칭 기록이 존재하지 않습니다.\n\n해당 매칭을 먼저 시도해주세요.");
                    return;
                }

                recordContent[4].text = record.score;
                recordContent[5].text = record.win.ToString();
                recordContent[6].text = (record.numOfMatch - record.win).ToString();
                float winRate = (float)record.win / record.numOfMatch;
                fillWinChart = StartCoroutine(FillPieChart(recordChart[0], winRate));
                fillLoseChart = StartCoroutine(FillPieChart(recordChart[1], 1 - winRate));
                countingWinRate = StartCoroutine(FillWinRate((int)record.winRate));
            });
        });
    }

    public void SetErrorObject(string error)
    {
        errorObject.SetActive(true);
        errorText.text = error;
    }

    public void EnableReconnectObject()
    {
        Dispatcher.Current.BeginInvoke(() =>
        {
            loadingObject.SetActive(true);
            Invoke("SetReconnectObject", 1.0f);
        });
    }

    private void SetReconnectObject()
    {
        loadingObject.SetActive(false);
        reconnectObject.SetActive(true);
    }

    public void ReconnectInGameProcess()
    {
        var tmp = matchDoneObject.GetComponentInChildren<Text>();
        tmp.text = "재접속 중...";
        modelObject.SetActive(false);
        MatchDoneCallback();

        BackEndMatchManager.GetInstance().ProcessReconnect();
    }

    public void JoinMatchProcess()
    {
        BackEndMatchManager.GetInstance().JoinMatchServer();
    }

    public void ChangeTab()
    {
        int index = 0;
        foreach (var tab in matchInfotabList)
        {
            if (tab.IsOn() == true)
            {
                break;
            }
            index += 1;
        }
        var matchInfo = BackEndMatchManager.GetInstance().matchInfos[index];
        matchInfoText.text = string.Format(matchInfoStr, matchInfo.headCount, matchInfo.isSandBoxEnable.Equals(true) ? "활성화" : "비활성화",
            matchInfo.matchType, matchInfo.matchModeType);
    }

    public IEnumerator FillPieChart(Image chart, float value)
    {
        while (chart.fillAmount < value)
        {
            chart.fillAmount += 0.01f;
            yield return null;
            recordChart[1].transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -recordChart[0].fillAmount * 360f));
        }
        chart.fillAmount = value;
        recordChart[1].transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -recordChart[0].fillAmount * 360f));
    }

    public IEnumerator FillWinRate(int winRate)
    {
        int tmp = 0;
        while (tmp < winRate)
        {
            recordContent[1].text = string.Format("{0}%", tmp);
            yield return null;
            tmp += 2;
        }
        recordContent[1].text = string.Format("{0}%", winRate);
    }

    public void SetLoadingObjectActive(bool isActive)
    {
        loadingObject.SetActive(isActive);
    }

    public bool IsLoadingObjectActive()
    {
        return loadingObject.activeSelf;
    }

    public bool IsErrorObjectActive()
    {
        return errorObject.activeSelf;
    }

    public void SetSelectObject(string msg, System.Action okFunc, System.Action cancelFunc)
    {
        selectMsg.text = msg;
        selectOkBtn.onClick.RemoveAllListeners();
        selectCancelBtn.onClick.RemoveAllListeners();

        selectOkBtn.onClick.AddListener(() =>
        {
            okFunc.Invoke();
            selectObject.SetActive(false);
        });
        selectCancelBtn.onClick.AddListener(() =>
        {
            cancelFunc.Invoke();
            selectObject.SetActive(false);
        });

        selectObject.SetActive(true);
    }
}
