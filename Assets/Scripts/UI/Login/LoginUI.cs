using UnityEngine;
using UnityEngine.UI;
using Battlehub.Dispatcher;

public class LoginUI : MonoBehaviour
{
    private static LoginUI instance;    // 인스턴스

    public GameObject mainTitle;
    public GameObject subTitle;
    public GameObject touchStart;
    public GameObject loginObject;
    public GameObject customLoginObject;
    public GameObject signUpObject;
    public GameObject errorObject;
    public GameObject nicknameObject;
    public GameObject updateObject;

    private InputField[] loginField;
    private InputField[] signUpField;
    private InputField nicknameField;
    private Text errorText;
    private GameObject loadingObject;
    private FadeAnimation fadeObject;

    private const byte ID_INDEX = 0;
    private const byte PW_INDEX = 1;
    private const string VERSION_STR = "Ver {0}";

    const string PlayStoreLink = "market://details?id=io.thebackend.backendMatch";
    const string AppStoreLink = "itms-apps://itunes.apple.com/app/id1497264852";

    void Awake()
    {
        instance = this;
    }

    public static LoginUI GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("LoginUI 인스턴스가 존재하지 않습니다.");
            return null;
        }
        return instance;
    }

    void Start()
    {
        mainTitle.SetActive(false);
        touchStart.SetActive(false);
        subTitle.SetActive(true);
        loginObject.SetActive(false);
        customLoginObject.SetActive(true);
        signUpObject.SetActive(false);
        errorObject.SetActive(false);
        nicknameObject.SetActive(false);
        updateObject.SetActive(false);

        loginField = customLoginObject.GetComponentsInChildren<InputField>();
        signUpField = signUpObject.GetComponentsInChildren<InputField>();
        nicknameField = nicknameObject.GetComponentInChildren<InputField>();
        errorText = errorObject.GetComponentInChildren<Text>();

        loadingObject = GameObject.FindGameObjectWithTag("Loading");
        loadingObject.SetActive(false);

        var fade = GameObject.FindGameObjectWithTag("Fade");
        if (fade != null)
        {
            fadeObject = fade.GetComponent<FadeAnimation>();
        }

        mainTitle.GetComponentInChildren<Text>().text = string.Format(VERSION_STR, Application.version);

        var google = loginObject.transform.GetChild(0).gameObject;
        var apple = loginObject.transform.GetChild(1).gameObject;
#if UNITY_ANDROID
        google.SetActive(true);
        apple.SetActive(false);
#elif UNITY_IOS
        google.SetActive(false);
        apple.SetActive(true);
#endif
    }

    public void OpenUpdatePopup()
    {
        updateObject.SetActive(true);
    }

    public void OpenStoreLink()
    {
#if UNITY_ANDROID
        Application.OpenURL(PlayStoreLink);
#elif UNITY_IOS
        Application.OpenURL(AppStoreLink);
#else
        Debug.LogError("지원하지 않는 플랫폼 입니다.");
#endif
        Debug.Log("스토어 링크 열기 버튼 클릭");
    }

    public void TouchStart()
    {
        // 업데이트 팝업이 떠있으면 진행 X
        if (updateObject.activeSelf == true)
        {
            return;
        }

        loadingObject.SetActive(true);
        BackEndServerManager.GetInstance().BackendTokenLogin((bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                if (result)
                {
                    ChangeLobbyScene();
                    return;
                }
                loadingObject.SetActive(false);
                if (!error.Equals(string.Empty))
                {
                    errorText.text = "유저 정보 불러오기 실패\n\n" + error;
                    errorObject.SetActive(true);
                    return;
                }
                mainTitle.SetActive(false);
                touchStart.SetActive(false);
                subTitle.SetActive(true);
                //customLoginObject.SetActive(true);
                loginObject.SetActive(true);
            });
        });
    }

    public void Login()
    {
        if (errorObject.activeSelf)
        {
            return;
        }
        string id = loginField[ID_INDEX].text;
        string pw = loginField[PW_INDEX].text;

        if (id.Equals(string.Empty) || pw.Equals(string.Empty))
        {
            errorText.text = "ID 혹은 PW 를 먼저 입력해주세요.";
            errorObject.SetActive(true);
            return;
        }

        loadingObject.SetActive(true);
        BackEndServerManager.GetInstance().CustomLogin(id, pw, (bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                if (!result)
                {
                    loadingObject.SetActive(false);
                    errorText.text = "로그인 에러\n\n" + error;
                    errorObject.SetActive(true);
                    return;
                }
                ChangeLobbyScene();
            });
        });
    }

    public void SignUp()
    {
        if (errorObject.activeSelf)
        {
            return;
        }
        string id = signUpField[ID_INDEX].text;
        string pw = signUpField[PW_INDEX].text;

        if (id.Equals(string.Empty) || pw.Equals(string.Empty))
        {
            errorText.text = "ID 혹은 PW 를 먼저 입력해주세요.";
            errorObject.SetActive(true);
            return;
        }

        loadingObject.SetActive(true);
        BackEndServerManager.GetInstance().CustomSignIn(id, pw, (bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                if (!result)
                {
                    loadingObject.SetActive(false);
                    errorText.text = "회원가입 에러\n\n" + error;
                    errorObject.SetActive(true);
                    return;
                }
                ChangeLobbyScene();
            });
        });
    }

    public void ActiveNickNameObject()
    {
        Dispatcher.Current.BeginInvoke(() =>
        {
            mainTitle.SetActive(false);
            touchStart.SetActive(false);
            subTitle.SetActive(true);
            loginObject.SetActive(false);
            customLoginObject.SetActive(false);
            signUpObject.SetActive(false);
            errorObject.SetActive(false);
            loadingObject.SetActive(false);
            nicknameObject.SetActive(true);
        });
    }

    public void UpdateNickName()
    {
        if (errorObject.activeSelf)
        {
            return;
        }
        string nickname = nicknameField.text;
        if (nickname.Equals(string.Empty))
        {
            errorText.text = "닉네임을 먼저 입력해주세요";
            errorObject.SetActive(true);
            return;
        }
        loadingObject.SetActive(true);
        BackEndServerManager.GetInstance().UpdateNickname(nickname, (bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                if (!result)
                {
                    loadingObject.SetActive(false);
                    errorText.text = "닉네임 생성 오류\n\n" + error;
                    errorObject.SetActive(true);
                    return;
                }
                ChangeLobbyScene();
            });
        });
    }

    public void GuestLogin()
    {
        if (errorObject.activeSelf)
        {
            return;
        }

        loadingObject.SetActive(true);
        BackEndServerManager.GetInstance().GuestLogin((bool result, string error) =>
        {
            Dispatcher.Current.BeginInvoke(() =>
            {
                if (!result)
                {
                    loadingObject.SetActive(false);
                    errorText.text = "로그인 에러\n\n" + error;
                    errorObject.SetActive(true);
                    return;
                }
                ChangeLobbyScene();
            });
        });
    }

    void ChangeLobbyScene()
    {
        if (fadeObject != null)
        {
            GameManager.GetInstance().ChangeState(GameManager.GameState.MatchLobby, (bool isDone) =>
            {
                Dispatcher.Current.BeginInvoke(() => loadingObject.transform.Rotate(0, 0, -10));
                if (isDone)
                {
                    fadeObject.ProcessFadeOut();
                }
            });
        }
        else
        {
            GameManager.GetInstance().ChangeState(GameManager.GameState.MatchLobby);
        }
    }
}
