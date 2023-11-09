using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;
    private static bool isCreate = false;

    #region Scene
    private const string LOGIN = "0. Login";
    private const string LOBBY = "1. MatchLobby";
    private const string READY = "2. LoadRoom";
    private const string INGAME = "3. InGame";
    #endregion


    #region Actions-Events
    //public static event Action OnRobby = delegate { };
    public static event Action OnGameReady = delegate { };
    //public static event Action OnGameStart = delegate { };
    public static event Action InGame = delegate { };
    public static event Action AfterInGame = delegate { };
    public static event Action OnGameOver = delegate { };
    public static event Action OnGameResult = delegate { };
    public static event Action OnGameReconnect = delegate { };

    private string asyncSceneName = string.Empty;
    private IEnumerator InGameUpdateCoroutine;

    public enum GameState { Login, MatchLobby, Ready, Start, InGame, Over, Result, Reconnect };
    private GameState gameState;
    #endregion

    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("GameManager 인스턴스가 존재하지 않습니다.");
            return null;
        }
        return instance;
    }
    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        // 60프레임 고정
        Application.targetFrameRate = 60;
        // 게임중 슬립모드 해제
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        InGameUpdateCoroutine = InGameUpdate();

        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        if (isCreate)
        {
            DestroyImmediate(gameObject, true);
            return;
        }
        gameState = GameState.Login;
        isCreate = true;
    }
    IEnumerator InGameUpdate()
    {
        while (true)
        {
            if (gameState != GameState.InGame)
            {
                StopCoroutine(InGameUpdateCoroutine);
                yield return null;
            }
            InGame();
            AfterInGame();
            yield return new WaitForSeconds(.1f); //1초 단위
        }
    }

    private void Login()
    {
        // OnLogin();
        // ChangeScene(LOGIN);
    }
    private void MatchLobby(Action<bool> func)
    {
        if (func != null)
        {
            ChangeSceneAsync(LOBBY, func);
        }
        else
        {
            ChangeScene(LOBBY);
        }
    }

    private void GameReady()
    {
        Debug.Log("게임 레디 상태 돌입");
        ChangeScene(READY);
        OnGameReady();
    }

    private void GameStart()
    {
        //delegate 초기화
        InGame = delegate { };
        AfterInGame = delegate { };
        OnGameOver = delegate { };
        OnGameResult = delegate { };

        //OnGameStart();
        // 게임씬이 로드되면 Start에서 OnGameStart 호출
        ChangeScene(INGAME);
    }

    private void GameOver()
    {
        OnGameOver();
    }

    private void GameResult()
    {
        OnGameResult();
    }

    private void GameReconnect()
    {
        //delegate 초기화
        InGame = delegate { };
        AfterInGame = delegate { };
        OnGameOver = delegate { };
        OnGameResult = delegate { };

        OnGameReconnect();
        ChangeScene(INGAME);
        ChangeState(GameManager.GameState.InGame);
    }

    public GameState GetGameState()
    {
        return gameState;
    }

    public void ChangeState(GameState state, Action<bool> func = null)
    {
        gameState = state;
        switch (gameState)
        {
            case GameState.Login:
                Login();
                break;
            case GameState.MatchLobby:
                MatchLobby(func);
                break;
            case GameState.Ready:
                GameReady();
                break;
            case GameState.Start:
                GameStart();
                break;
            case GameState.Over:
                GameOver();
                break;
            case GameState.Result:
                GameResult();
                break;
            case GameState.InGame:
                // 코루틴 시작
                StartCoroutine(InGameUpdateCoroutine);
                break;
            case GameState.Reconnect:
                GameReconnect();
                break;
            default:
                Debug.Log("알수없는 스테이트입니다. 확인해주세요.");
                break;
        }
    }

    public bool IsLobbyScene()
    {
        return SceneManager.GetActiveScene().name == LOBBY;
    }

    private void ChangeScene(string scene)
    {
        if (scene != LOGIN && scene != INGAME && scene != LOBBY && scene != READY)
        {
            Debug.Log("알수없는 씬 입니다.");
            return;
        }

        SceneManager.LoadScene(scene);
    }

    private void ChangeSceneAsync(string scene, Action<bool> func)
    {
        asyncSceneName = string.Empty;
        if (scene != LOGIN && scene != INGAME && scene != LOBBY && scene != READY)
        {
            Debug.Log("알수없는 씬 입니다.");
            return;
        }
        asyncSceneName = scene;

        StartCoroutine("LoadScene", func);
    }

    private IEnumerator LoadScene(Action<bool> func)
    {
        var asyncScene = SceneManager.LoadSceneAsync(asyncSceneName);
        asyncScene.allowSceneActivation = true;

        bool isCallFunc = false;
        while (asyncScene.isDone == false)
        {
            if (asyncScene.progress <= 0.9f)
            {
                func(false);
            }
            else if (isCallFunc == false)
            {
                isCallFunc = true;
                func(true);
            }
            yield return null;
        }
    }
}
