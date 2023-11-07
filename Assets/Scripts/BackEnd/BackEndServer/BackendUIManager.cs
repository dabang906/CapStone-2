using BackEnd;
using UnityEngine;
using UnityEngine.UI;

public class BackendUIManager : MonoBehaviour
{
    private static BackendUIManager instance;

    // 전환 Object
    public GameObject titleObject;
    public GameObject loginObject;
    public GameObject signUpObject;
    public GameObject roomListObject;
    public GameObject makeRoomObject;
    public GameObject waitingRoomObject;

    // InputField 모음
    private InputField[] loginField;
    private InputField[] signUpField;

    // titleObject 하위 Object
    public Button titleLogin, titleSignUp, titleExit;

    // LoginObject 하위 Object
    public Button loginConfirm, loginBack;

    // SignUpObject 하위 Object
    public Button signUpConfirm, signUpBack;

    // RoomListObject 하위 Object
    public Button roomListConfirm, roomListBack;
    private Text roomListShowNickname;

    // makeRoomObject 하위 Object
    public Button makeRoomConfirm, makeRoomBack;

    public static BackendUIManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("BackendUIManager 인스턴스가 존재하지 않습니다.");
            return null;
        }
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {

        titleObject.SetActive(true);
        loginObject.SetActive(false);
        signUpObject.SetActive(false);
        roomListObject.SetActive(false);
        makeRoomObject.SetActive(false);
        waitingRoomObject.SetActive(false);

        loginField = loginObject.GetComponentsInChildren<InputField>();
        signUpField = signUpObject.GetComponentsInChildren<InputField>();

        roomListShowNickname = roomListObject.GetComponentInChildren<Text>();

        #region ButtonEvent
        /**
         * titleObject 버튼 이벤트
         * 각 버튼별 Object 전환 기능
         */
        titleLogin.onClick.AddListener(() =>
            {
                titleObject.SetActive(false);
                loginObject.SetActive(true);
            });
        titleSignUp.onClick.AddListener(() =>
        {
            titleObject.SetActive(false);
            signUpObject.SetActive(true);
        });

        titleExit.onClick.AddListener(() =>
        {
            Debug.Log("게임 종료!");
        });


        /**
         * loginObject 버튼 이벤트
         * 로그인 성공 시 roomListObject로 전환
         * 로그인 실패 시 텍스트필드 내용 삭제
         */
        loginConfirm.onClick.AddListener(() =>
        {
            string id = loginField[0].text;
            string pw = loginField[1].text;

            if (BackendServerManager.GetInstance().CustomLogin(id, pw))
            {
                BackendMatchManager.GetInstance().JoinMatchServer();
                loginField[0].text = null;
                loginField[1].text = null;
                loginObject.SetActive(false);
                roomListObject.SetActive(true);
                roomListShowNickname.text = "환영합니다, " + BackendServerManager.GetInstance().GetUserNickName() + "님!";
            }
            else
            {
                loginField[0].text = null;
                loginField[1].text = null;
            }
        });
        loginBack.onClick.AddListener(() =>
        {
            loginField[0].text = null;
            loginField[1].text = null;
            loginObject.SetActive(false);
            titleObject.SetActive(true);
        });


        /** 
         * signUpObject 버튼 이벤트
         * 회원가입 성공 시 TitleObject로 복귀
         * 회원가입 실패 시 텍스트필드 내용 삭제
         */
        signUpConfirm.onClick.AddListener(() =>
        {
            string id = signUpField[0].text;
            string nickname = signUpField[1].text;
            string pw = signUpField[2].text;
            string checkPw = signUpField[3].text;

            if (pw == checkPw)
            {
                if (BackendServerManager.GetInstance().CustomSignIn(id, nickname, pw))
                {
                    signUpField[0].text = null;
                    signUpField[1].text = null;
                    signUpField[2].text = null;
                    signUpField[3].text = null;
                    signUpObject.SetActive(false);
                    titleObject.SetActive(true);
                }
                else
                {
                    signUpField[0].text = null;
                    signUpField[1].text = null;
                    signUpField[2].text = null;
                    signUpField[3].text = null;
                }
            }
            else
            {
                signUpField[0].text = null;
                signUpField[1].text = null;
                signUpField[2].text = null;
                signUpField[3].text = null;
            }
        });
        signUpBack.onClick.AddListener(() =>
        {
            signUpField[0].text = null;
            signUpField[1].text = null;
            signUpField[2].text = null;
            signUpField[3].text = null;
            signUpObject.SetActive(false);
            titleObject.SetActive(true);
        });


        /**
         * roomList 버튼 이벤트
         * 방 만들기 클릭 시 방 제목 설정 화면 띄우고
         * 로그아웃 클릭 시 로그아웃
         */
        roomListConfirm.onClick.AddListener(() =>
        {
            makeRoomObject.SetActive(true);
        });
        roomListBack.onClick.AddListener(() =>
        {
            BackendServerManager.GetInstance().CustomLogout();
            BackendMatchManager.GetInstance().LeaveMatchServer();
            Backend.Match.LeaveMatchMakingServer();
            roomListObject.SetActive(false);
            makeRoomObject.SetActive(false);
            titleObject.SetActive(true);
        });


        /**
         * makeRoom 버튼 이벤트
         * 방 생성 시 waitingRoomObject로 넘어감
         * 생성 취소 시 makeRoomObject 비활성화
         */
        makeRoomConfirm.onClick.AddListener(() =>
        {
            BackendMatchManager.GetInstance().CreateMatchRoom();
            roomListObject.SetActive(false);
            makeRoomObject.SetActive(false);
            waitingRoomObject.SetActive(true);
        });
        makeRoomBack.onClick.AddListener(() =>
        {
            makeRoomObject.SetActive(false);
        });
        #endregion
    }
}
