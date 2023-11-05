using UnityEngine;
using BackEnd;
using BackEnd.Tcp;

public class BackendServerManager : MonoBehaviour
{
    private static BackendServerManager instance;   // 인스턴스
    public bool isLogin { get; private set; }   // 로그인 여부

    public static string myNickName { get; private set; } = string.Empty;  // 로그인한 계정의 닉네임
    public static string myIndate { get; private set; } = string.Empty;    // 로그인한 계정의 inDate

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public static BackendServerManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("BackEndServerManager 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return instance;
    }

    private void Start()
    {
        isLogin = false;

        var bro = Backend.Initialize(true);

        if(bro.IsSuccess())
        {
            Debug.Log("초기화 성공 : " + bro);
        }
        else
        {
            Debug.LogError("초기화 실패 : " + bro);
        }
    }

    private void Update()
    {
        Backend.AsyncPoll();
    }

    public bool CustomLogin(string id, string pw)
    {
        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("로그인 성공 : " + bro);

            ErrorInfo errorInfo;
            Backend.Match.JoinMatchMakingServer(out errorInfo);
            OnPrevBackendAuthorized();
            return true;
        }
        else
        {
            Debug.Log("로그인 실패 : " + bro);
            return false;
        }
    }

    public void CustomLogout()
    {
        isLogin = false;
        Backend.Match.LeaveMatchMakingServer();
        Debug.Log(myNickName + " 로그아웃");
    }

    public bool CustomSignIn(string id, string nickname, string pw)
    {
        var bro = Backend.BMember.CustomSignUp(id, pw);

        if (bro.IsSuccess())
        {
            bro = Backend.BMember.CreateNickname(nickname);
            if (bro.IsSuccess())
            {
                Debug.Log("회원가입 성공 : " + bro);
                return true;
            }
            else
            {
                Debug.Log("회원가입 실패 : " + bro);
                return false;
            }
        }
        else
        {
            Debug.Log("회원가입 실패 : " + bro);
            return false;
        }
    }

    // 닉네임 Getter
    public string GetUserNickName()
    {
        return myNickName;
    }

    // 아이디 생성일자 Getter
    public string GetUserInDate()
    {
        return myIndate;
    }

    // 유저 정보 불러오기 사전작업
    private void OnPrevBackendAuthorized()
    {
        isLogin = true;

        OnBackendAuthorized();
    }

    // 실제 유저 정보 불러오기
    private void OnBackendAuthorized()
    {
        var bro = Backend.BMember.GetUserInfo();

        if (bro.IsSuccess())
        {
            Debug.Log("유저 정보 : " + bro);
            var info = bro.GetReturnValuetoJSON()["row"];

            myNickName = info["nickname"].ToString();
            myIndate = info["inDate"].ToString();
        }
        else
        {
            Debug.Log("유저 정보 불러오기 실패 : " + bro);
        }
    }
}
