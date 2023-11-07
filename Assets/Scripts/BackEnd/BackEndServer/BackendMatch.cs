using System.Collections;
using System.Collections.Generic;
using BackEnd;
using BackEnd.Tcp;
using UnityEngine;

public partial class BackendMatchManager : MonoBehaviour
{
    public MatchType nowMatchType { get; private set; } = MatchType.None;     // 현재 선택된 매치 타입
    public MatchModeType nowModeType { get; private set; } = MatchModeType.None; // 현재 선택된 매치 모드 타입

    // 디버그 로그
    private string NOTCONNECT_MATCHSERVER = "매치 서버에 연결되어 있지 않습니다.";
    private string RECONNECT_MATCHSERVER = "매치 서버에 접속을 시도합니다.";
    private string FAIL_CONNECT_MATCHSERVER = "매치 서버 접속 실패 : {0}";
    private string SUCCESS_CONNECT_MATCHSERVER = "매치 서버 접속 성공";
    private string SUCCESS_MATCHMAKE = "매칭 성공 : {0}";
    private string SUCCESS_REGIST_MATCHMAKE = "매칭 대기열에 등록되었습니다.";
    private string FAIL_REGIST_MATCHMAKE = "매칭 실패 : {0}";
    private string CANCEL_MATCHMAKE = "매칭 신청 취소 : {0}";
    private string INVAILD_MATCHTYPE = "잘못된 매치 타입입니다.";
    private string INVALID_MODETYPE = "잘못된 모드 타입입니다.";
    private string INVALID_OPERATION = "잘못된 요청입니다\n{0}";
    private string EXCEPTION_OCCUR = "예외 발생 : {0}\n다시 매칭을 시도합니다.";

    // 매칭된 매치 타입,모드 타입 설정
    public void SetNowMatchInfo(MatchType matchType, MatchModeType matchModeType)
    {
        this.nowMatchType = matchType;
        this.nowModeType = matchModeType;

        Debug.Log(string.Format("매칭 타입/모드 : {0}/{1}", nowMatchType, nowModeType));
    }

    public void JoinMatchServer()
    {
        // 매칭 서버 접속
        if (isConnectMatchServer)
        {
            return;
        }
        ErrorInfo errorInfo;
        isConnectMatchServer = true;
        if (!Backend.Match.JoinMatchMakingServer(out errorInfo))
        {
            var errorLog = string.Format(FAIL_CONNECT_MATCHSERVER, errorInfo.ToString());
            Debug.Log(errorLog);
        }
    }

    // 매칭 서버 접속종료
    public void LeaveMatchServer()
    {
        isConnectMatchServer = false;
        Backend.Match.LeaveMatchMakingServer();
    }

    // 매칭 대기 방 만들기
    // 혼자 매칭을 하더라도 무조건 방을 생성한 뒤 매칭을 신청해야 함
    public bool CreateMatchRoom()
    {
        // 매청 서버에 연결되어 있지 않으면 매칭 서버 접속
        if (!isConnectMatchServer)
        {
            Debug.Log(NOTCONNECT_MATCHSERVER);
            Debug.Log(RECONNECT_MATCHSERVER);
            JoinMatchServer();
            return false;
        }
        Debug.Log("방 생성 요청을 서버로 보냄");
        Backend.Match.CreateMatchRoom();
        return true;
    }

    // 매칭 대기 방 나가기
    public void LeaveMatchLoom()
    {
        Backend.Match.LeaveMatchRoom();
    }

    // 매칭 신청하기
    // MatchType (1:1 / 개인전 / 팀전)
    // MatchModeType (랜덤 / 포인트 / MMR)
    public void RequestMatchMaking(int index)
    {
        // 매청 서버에 연결되어 있지 않으면 매칭 서버 접속
        if (!isConnectMatchServer)
        {
            Debug.Log(NOTCONNECT_MATCHSERVER);
            Debug.Log(RECONNECT_MATCHSERVER);
            JoinMatchServer();
            return;
        }
        // 변수 초기화
        isConnectInGameServer = false;

        Backend.Match.RequestMatchMaking(matchInfos[index].matchType, matchInfos[index].matchModeType, matchInfos[index].inDate);
        if (isConnectInGameServer)
        {
            Backend.Match.LeaveGameServer(); //인게임 서버 접속되어 있을 경우를 대비해 인게임 서버 리브 호출
        }

        //nowMatchType = matchInfos[index].matchType;
        //nowModeType = matchInfos[index].matchModeType;
        //numOfClient = int.Parse(matchInfos[index].headCount);
    }

    // 매칭 신청 취소하기
    public void CancelRegistMatchMaking()
    {
        Backend.Match.CancelMatchMaking();
    }

    // 매칭 서버 접속에 대한 리턴값
    private void ProcessAccessMatchMakingServer(ErrorInfo errInfo)
    {
        if (errInfo != ErrorInfo.Success)
        {
            // 접속 실패
            isConnectMatchServer = false;
        }

        if (!isConnectMatchServer)
        {
            var errorLog = string.Format(FAIL_CONNECT_MATCHSERVER, errInfo.ToString());
            // 접속 실패
            Debug.Log(errorLog);
        }
        else
        {
            //접속 성공
            Debug.Log(SUCCESS_CONNECT_MATCHSERVER);
        }
    }

    /*
	 * 매칭 신청에 대한 리턴값 (호출되는 종류)
	 * 매칭 신청 성공했을 때
	 * 매칭 성공했을 때
	 * 매칭 신청 실패했을 때
	*/
    private void ProcessMatchMakingResponse(MatchMakingResponseEventArgs args)
    {
        string debugLog = string.Empty;
        bool isError = false;
        switch (args.ErrInfo)
        {
            case ErrorCode.Success:
                // 매칭 성공했을 때
                debugLog = string.Format(SUCCESS_MATCHMAKE, args.Reason);
                //LobbyUI.GetInstance().MatchDoneCallback();
                //ProcessMatchSuccess(args);
                break;
            case ErrorCode.Match_InProgress:
                // 매칭 신청 성공했을 때 or 매칭 중일 때 매칭 신청을 시도했을 때

                // 매칭 신청 성공했을 때
                if (args.Reason == string.Empty)
                {
                    debugLog = SUCCESS_REGIST_MATCHMAKE;

                    //LobbyUI.GetInstance().MatchRequestCallback(true);
                }
                break;
            case ErrorCode.Match_MatchMakingCanceled:
                // 매칭 신청이 취소되었을 때
                debugLog = string.Format(CANCEL_MATCHMAKE, args.Reason);

                ////LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Match_InvalidMatchType:
                isError = true;
                // 매치 타입을 잘못 전송했을 때
                debugLog = string.Format(FAIL_REGIST_MATCHMAKE, INVAILD_MATCHTYPE);

                ////LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Match_InvalidModeType:
                isError = true;
                // 매치 모드를 잘못 전송했을 때
                debugLog = string.Format(FAIL_REGIST_MATCHMAKE, INVALID_MODETYPE);

                //LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.InvalidOperation:
                isError = true;
                // 잘못된 요청을 전송했을 때
                debugLog = string.Format(INVALID_OPERATION, args.Reason);
                //LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Match_Making_InvalidRoom:
                isError = true;
                // 잘못된 요청을 전송했을 때
                debugLog = string.Format(INVALID_OPERATION, args.Reason);
                //LobbyUI.GetInstance().MatchRequestCallback(false);
                break;
            case ErrorCode.Exception:
                isError = true;
                // 매칭 되고, 서버에서 방 생성할 때 에러 발생 시 exception이 리턴됨
                // 이 경우 다시 매칭 신청해야 됨
                debugLog = string.Format(EXCEPTION_OCCUR, args.Reason);

                //LobbyUI.GetInstance().RequestMatch();
                break;
        }

        if (!debugLog.Equals(string.Empty))
        {
            Debug.Log(debugLog);
            if (isError == true)
            {
                //LobbyUI.GetInstance().SetErrorObject(debugLog);
            }
        }
    }

    // 매칭 성공했을 때
    // 인게임 서버로 접속해야 한다.
    private void ProcessMatchSuccess(MatchMakingResponseEventArgs args)
    {
        ErrorInfo errorInfo;
        if (sessionIdList != null)
        {
            Debug.Log("이전 세션 저장 정보");
            sessionIdList.Clear();
        }

        if (!Backend.Match.JoinGameServer(args.RoomInfo.m_inGameServerEndPoint.m_address, args.RoomInfo.m_inGameServerEndPoint.m_port, false, out errorInfo))
        {
            var debugLog = string.Format(FAIL_ACCESS_INGAME, errorInfo.ToString(), string.Empty);
            Debug.Log(debugLog);
        }
        // 인자값에서 인게임 룸토큰을 저장해두어야 한다.
        // 인게임 서버에서 룸에 접속할 때 필요
        // 1분 내에 모든 유저가 룸에 접속하지 않으면 해당 룸은 파기된다.
        isConnectInGameServer = true;
        isJoinGameRoom = false;
        isReconnectProcess = false;
        inGameRoomToken = args.RoomInfo.m_inGameRoomToken;
        isSandBoxGame = args.RoomInfo.m_enableSandbox;
        var info = GetMatchInfo(args.MatchCardIndate);
        if (info == null)
        {
            Debug.LogError("매치 정보를 불러오는 데 실패했습니다.");
            return;
        }

        nowMatchType = info.matchType;
        nowModeType = info.matchModeType;
        numOfClient = int.Parse(info.headCount);
    }

    // 서버에서 게임 시작 패킷을 보냈을 때 호출
    // 모든 세션이 게임 룸에 참여 후 "콘솔에서 설정한 시간" 후에 게임 시작 패킷이 서버에서 온다
    private void GameSetup()
    {
        Debug.Log("게임 시작 메시지 수신. 게임 설정 시작");
        // 게임 시작 메시지가 오면 게임을 레디 상태로 변경
        //if (GameManager.GetInstance().GetGameState() != GameManager.GameState.Ready)
        //{
        //    isHost = false;
        //    isSetHost = false;
        //    OnGameReady();
        //    //LobbyUI.GetInstance().ChangeRoomLoadScene();
        //}
        // GameManager.GetInstance().ChangeState(GameManager.GameState.Start);
    }
}
