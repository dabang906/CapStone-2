using System.Collections;
using System.Collections.Generic;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine;

public partial class BackendMatchManager : MonoBehaviour
{
    private bool isSetHost = false;                 // 호스트 세션 결정했는지 여부

    private MatchGameResult matchGameResult;

    // 게임 로그
    private string FAIL_ACCESS_INGAME = "인게임 접속 실패 : {0} - {1}";
    private string SUCCESS_ACCESS_INGAME = "유저 인게임 접속 성공 : {0}";
    private string NUM_INGAME_SESSION = "인게임 내 세션 갯수 : {0}";

    // 게임 레디 상태일 때 호출됨
    public void OnGameReady()
    {
        if (isSetHost == false)
        {
            // 호스트가 설정되지 않은 상태이면 호스트 설정
            isSetHost = SetHostSession();
        }
        Debug.Log("호스트 설정 완료");

        if (IsHost() == true)
        {
            // 0.5초 후 ReadyToLoadRoom 함수 호출
            Invoke("ReadyToLoadRoom", 0.5f);
        }
    }

    private void OnGameReconnect()
    {
        isHost = false;
        localQueue = null;
        Debug.Log("재접속 프로세스 진행중... 호스트 및 로컬 큐 설정 완료");
    }

    // 현재 룸에 접속한 세션들의 정보
    // 최초 룸에 접속했을 때 1회 수신됨
    // 재접속 했을 때도 1회 수신됨
    private void ProcessMatchInGameSessionList(MatchInGameSessionListEventArgs args)
    {
        sessionIdList = new List<SessionId>();
        gameRecords = new Dictionary<SessionId, MatchUserGameRecord>();

        foreach (var record in args.GameRecords)
        {
            sessionIdList.Add(record.m_sessionId);
            gameRecords.Add(record.m_sessionId, record);
        }
        sessionIdList.Sort();
    }

    // 클라이언트 들의 게임 룸 접속에 대한 리턴값
    // 클라이언트가 게임 룸에 접속할 때마다 호출됨
    // 재접속 했을 때는 수신되지 않음
    private void ProcessMatchInGameAccess(MatchInGameSessionEventArgs args)
    {
        if (isReconnectProcess)
        {
            // 재접속 프로세스 인 경우
            // 이 메시지는 수신되지 않고, 만약 수신되어도 무시함
            Debug.Log("재접속 프로세스 진행중... 재접속 프로세스에서는 ProcessMatchInGameAccess 메시지는 수신되지 않습니다.\n" + args.ErrInfo);
            return;
        }

        Debug.Log(string.Format(SUCCESS_ACCESS_INGAME, args.ErrInfo));

        if (args.ErrInfo != ErrorCode.Success)
        {
            // 게임 룸 접속 실패
            var errorLog = string.Format(FAIL_ACCESS_INGAME, args.ErrInfo, args.Reason);
            Debug.Log(errorLog);
            LeaveInGameRoom();
            return;
        }

        // 게임 룸 접속 성공
        // 인자값에 방금 접속한 클라이언트(세션)의 세션ID와 매칭 기록이 들어있다.
        // 세션 정보는 누적되어 들어있기 때문에 이미 저장한 세션이면 건너뛴다.

        var record = args.GameRecord;
        Debug.Log(string.Format(string.Format("인게임 접속 유저 정보 [{0}] : {1}", args.GameRecord.m_sessionId, args.GameRecord.m_nickname)));
        if (!sessionIdList.Contains(args.GameRecord.m_sessionId))
        {
            // 세션 정보, 게임 기록 등을 저장
            sessionIdList.Add(record.m_sessionId);
            gameRecords.Add(record.m_sessionId, record);

            Debug.Log(string.Format(NUM_INGAME_SESSION, sessionIdList.Count));
        }
    }

    // 인게임 룸 접속
    private void AccessInGameRoom(string roomToken)
    {
        Backend.Match.JoinGameRoom(roomToken);
    }

    // 인게임 서버 접속 종료
    public void LeaveInGameRoom()
    {
        isConnectInGameServer = false;
        Backend.Match.LeaveGameServer();
    }
}
