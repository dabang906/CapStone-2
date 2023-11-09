using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using BackEnd.Tcp;

/*
 * 매치매니저 (인게임 관련 기능)
 * BackEndInGame.cs에서 정의된 기능들
 * 인게임에 필요한 변수들
 * 인게임서버 게임룸 접속하기 (인게임서버 접속은 BackEndMatch.cs에서 정의)
 * 인게임서버 접속종료
 * 게임시작
 * 게임결과값 전송
 * 게임결과값 조합 (1:1 / 개인전 / 팀전)
 * 서버로 데이터 패킷 전송
 */
public partial class BackEndMatchManager : MonoBehaviour
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

        if (isSandBoxGame == true && IsHost() == true)
        {
            SetAIPlayer();
        }

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

    // 서버에서 게임 시작 패킷을 보냈을 때 호출
    // 모든 세션이 게임 룸에 참여 후 "콘솔에서 설정한 시간" 후에 게임 시작 패킷이 서버에서 온다
    private void GameSetup()
    {
        Debug.Log("게임 시작 메시지 수신. 게임 설정 시작");
        // 게임 시작 메시지가 오면 게임을 레디 상태로 변경
        if (GameManager.GetInstance().GetGameState() != GameManager.GameState.Ready)
        {
            isHost = false;
            isSetHost = false;
            OnGameReady();
            //LobbyUI.GetInstance().ChangeRoomLoadScene();
        }
        // GameManager.GetInstance().ChangeState(GameManager.GameState.Start);
    }


    private void ReadyToLoadRoom()
    {
        if (BackEndMatchManager.GetInstance().isSandBoxGame == true)
        {
            Debug.Log("샌드박스 모드 활성화. AI 정보 송신");
            // 샌드박스 모드면 ai 정보 송신
            foreach (var tmp in gameRecords)
            {
                if ((int)tmp.Key > (int)SessionId.Reserve)
                {
                    continue;
                }
                Debug.Log("ai정보 송신 : " + (int)tmp.Key);
                SendDataToInGame(new Protocol.AIPlayerInfo(tmp.Value));
            }
        }

        Debug.Log("1초 후 룸 씬 전환 메시지 송신");
        Invoke("SendChangeRoomScene", 1f);
    }

    private void SendChangeRoomScene()
    {
        Debug.Log("룸 씬 전환 메시지 송신");
        SendDataToInGame(new Protocol.LoadRoomSceneMessage());
    }

    private void SendChangeGameScene()
    {
        Debug.Log("게임 씬 전환 메시지 송신");
        SendDataToInGame(new Protocol.LoadGameSceneMessage());
    }

    // 서버로 게임 결과 전송
    // 서버에서 각 클라이언트가 보낸 결과를 종합
    public void MatchGameOver(Stack<SessionId> record)
    {
        if (nowModeType == MatchModeType.OneOnOne)
        {
            matchGameResult = OneOnOneRecord(record);
        }
        else if (nowModeType == MatchModeType.Melee)
        {
            matchGameResult = MeleeRecord(record);
        }
        else if (nowModeType == MatchModeType.TeamOnTeam)
        {
            matchGameResult = TeamRecord(record);
        }
        else
        {
            Debug.LogError("게임 결과 종합 실패 - 알수없는 매치모드타입입니다.\n" + nowModeType);
            return;
        }

        MatchResultUI.GetInstance().SetGameResult(matchGameResult);
        RemoveAISessionInGameResult();
        Backend.Match.MatchEnd(matchGameResult);
    }

    private void RemoveAISessionInGameResult()
    {
        string str = string.Empty;
        List<SessionId> aiSession = new List<SessionId>();
        if (matchGameResult.m_winners != null)
        {
            str += "승자 : ";
            foreach (var tmp in matchGameResult.m_winners)
            {
                if ((int)tmp < (int)SessionId.Reserve)
                {
                    aiSession.Add(tmp);
                }
                else
                {
                    str += tmp + " : ";
                }
            }
            str += "\n";
            matchGameResult.m_winners.RemoveAll(aiSession.Contains);
        }

        aiSession.Clear();
        if (matchGameResult.m_losers != null)
        {
            str += "패자 : ";
            foreach (var tmp in matchGameResult.m_losers)
            {
                if ((int)tmp < (int)SessionId.Reserve)
                {
                    aiSession.Add(tmp);
                }
                else
                {
                    str += tmp + " : ";
                }
            }
            str += "\n";
            matchGameResult.m_losers.RemoveAll(aiSession.Contains);
        }
        Debug.Log(str);
    }


    // 1:1 게임 결과
    private MatchGameResult OneOnOneRecord(Stack<SessionId> record)
    {
        MatchGameResult nowGameResult = new MatchGameResult();

        nowGameResult.m_winners = new List<SessionId>();
        nowGameResult.m_winners.Add(record.Pop());

        nowGameResult.m_losers = new List<SessionId>();
        nowGameResult.m_losers.Add(record.Pop());

        nowGameResult.m_draws = null;

        return nowGameResult;
    }

    // 개인전 게임 결과
    private MatchGameResult MeleeRecord(Stack<SessionId> record)
    {
        MatchGameResult nowGameResult = new MatchGameResult();
        nowGameResult.m_draws = null;
        nowGameResult.m_losers = null;
        nowGameResult.m_winners = new List<SessionId>();
        int size = record.Count;
        for (int i = 0; i < size; ++i)
        {
            nowGameResult.m_winners.Add(record.Pop());
        }

        return nowGameResult;
    }

    // 팀전 게임 결과
    private MatchGameResult TeamRecord(Stack<SessionId> record)
    {
        var winnerSession = record.Pop();
        var teamNumber = GetTeamInfo(winnerSession);

        MatchGameResult nowGameResult = new MatchGameResult();
        nowGameResult.m_draws = null;
        nowGameResult.m_losers = new List<SessionId>();
        nowGameResult.m_winners = new List<SessionId>();

        foreach (var user in gameRecords)
        {
            if (user.Value.m_teamNumber == teamNumber)
            {
                nowGameResult.m_winners.Add(user.Key);
            }
            else
            {
                nowGameResult.m_losers.Add(user.Key);
            }
        }

        return nowGameResult;
    }

    // 호스트에서 보낸 세션리스트로 갱신
    public void SetPlayerSessionList(List<SessionId> sessions)
    {
        sessionIdList = sessions;
    }

    // 서버로 데이터 패킷 전송
    // 서버에서는 이 패킷을 받아 모든 클라이언트(패킷 보낸 클라이언트 포함)로 브로드캐스팅 해준다.
    public void SendDataToInGame<T>(T msg)
    {
        var byteArray = DataParser.DataToJsonData<T>(msg);
        Backend.Match.SendDataToInGameRoom(byteArray);
    }

    private void ProcessSessionOffline(SessionId sessionId)
    {
        if (hostSession.Equals(sessionId))
        {
            // 호스트 연결 대기를 띄움
            InGameUiManager.GetInstance().SetHostWaitBoard();
        }
        else
        {
            // 호스트가 아니면 단순히 UI 만 띄운다.
        }
    }

    private void ProcessSessionOnline(SessionId sessionId, string nickName)
    {
        InGameUiManager.GetInstance().SetReconnectBoard(nickName);
        // 호스트가 아니면 아무 작업 안함 (호스트가 해줌)
        if (isHost)
        {
            // 재접속 한 클라이언트가 인게임 씬에 접속하기 전 게임 정보값을 전송 시 nullptr 예외가 발생하므로 조금
            // 2초정도 기다린 후 게임 정보 메시지를 보냄
            Invoke("SendGameSyncMessage", 2.0f);
        }
    }

    // Invoke로 실행됨
    private void SendGameSyncMessage()
    {
        // 현재 게임 상황 (위치, hp 등등...)
        var message = WorldManager.instance.GetNowGameState(hostSession);
        SendDataToInGame(message);
    }

    private void SetAIPlayer()
    {
        int aiCount = numOfClient - sessionIdList.Count;
        int numOfTeamOne = 0;
        int numOfTeamTwo = 0;

        Debug.Log("AI 플레이어 설정 : aiCount : " + aiCount);

        if (nowModeType == MatchModeType.TeamOnTeam)
        {
            foreach (var tmp in gameRecords)
            {
                if (tmp.Value.m_teamNumber == 0)
                {
                    numOfTeamOne += 1;
                }
                else
                {
                    numOfTeamTwo += 1;
                }
            }
        }
        int index = 0;
        for (int i = 0; i < aiCount; ++i)
        {
            MatchUserGameRecord aiRecord = new MatchUserGameRecord();
            aiRecord.m_nickname = "AIPlayer" + index;
            aiRecord.m_sessionId = (SessionId)index;
            aiRecord.m_numberOfMatches = 0;
            aiRecord.m_numberOfWin = 0;
            aiRecord.m_numberOfDefeats = 0;
            aiRecord.m_numberOfDraw = 0;
            if (nowMatchType == MatchType.MMR)
            {
                aiRecord.m_mmr = 1000;
            }
            else if (nowMatchType == MatchType.Point)
            {
                aiRecord.m_points = 1000;
            }

            if (nowModeType == MatchModeType.TeamOnTeam)
            {
                if (numOfTeamOne > numOfTeamTwo)
                {
                    aiRecord.m_teamNumber = 1;
                    numOfTeamTwo += 1;
                }
                else
                {
                    aiRecord.m_teamNumber = 0;
                    numOfTeamOne += 1;
                }
            }
            gameRecords.Add((SessionId)index, aiRecord);
            sessionIdList.Add((SessionId)index);
            index += 1;
        }
    }

    public bool PrevGameMessage(byte[] BinaryUserData)
    {
        Protocol.Message msg = DataParser.ReadJsonData<Protocol.Message>(BinaryUserData);
        if (msg == null)
        {
            return false;
        }

        // 게임 설정 사전 작업 패킷 검사 
        switch (msg.type)
        {
            case Protocol.Type.AIPlayerInfo:
                Protocol.AIPlayerInfo aiPlayerInfo = DataParser.ReadJsonData<Protocol.AIPlayerInfo>(BinaryUserData);
                ProcessAIDate(aiPlayerInfo);
                return true;
            case Protocol.Type.LoadRoomScene:
                LobbyUI.GetInstance().ChangeRoomLoadScene();
                if (IsHost() == true)
                {
                    Debug.Log("5초 후 게임 씬 전환 메시지 송신");
                    Invoke("SendChangeGameScene", 5f);
                }
                return true;
            case Protocol.Type.LoadGameScene:
                GameManager.GetInstance().ChangeState(GameManager.GameState.Start);
                return true;
        }
        return false;
    }

    private void ProcessAIDate(Protocol.AIPlayerInfo aIPlayerInfo)
    {
        MatchInGameSessionEventArgs args = new MatchInGameSessionEventArgs();
        args.GameRecord = aIPlayerInfo.GetMatchRecord();

        ProcessMatchInGameAccess(args);
    }
}
