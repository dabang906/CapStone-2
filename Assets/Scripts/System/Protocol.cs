using BackEnd.Tcp;
using UnityEngine;
using System.Collections.Generic;

namespace Protocol
{
    // 이벤트 타입
    public enum Type : sbyte
    {
        Key = 0,        // 키(가상 조이스틱) 입력
        PlayerMove,     // 플레이어 이동
        PlayerRotate,   // 플레이어 회전
        PlayerAttack,   // 플레이어 공격
        PlayerDamaged,  // 플레이어 데미지 받음
        PlayerNoMove,   // 플레이어 이동 멈춤
        PlayerNoRotate, // 플레이어 회전 멈춤
        bulletInfo,

        AIPlayerInfo,   // AI가 존재하는 경우 AI 정보
        LoadRoomScene,      // 룸 씬으로 전환
        LoadGameScene,      // 인게임 씬으로 전환
        StartCount,     // 시작 카운트
        GameStart,      // 게임 시작
        GameEnd,        // 게임 종료
        GameSync,       // 플레이어 재접속 시 게임 현재 상황 싱크
        Max
    }

    // 애니메이션 싱크는 사용하지 않습니다.
    /*
    public enum AnimIndex
    {
        idle = 0,
        walk,
        walkBack,
        stop,
        max
    }
    */

    // 조이스틱 키 이벤트 코드
    public static class KeyEventCode
    {
        public const int NONE = 0;
        public const int MOVE = 1;      // 이동 메시지
        public const int ATTACK = 2;    // 공격 메시지
        public const int NO_MOVE = 4;   // 이동 멈춤 메시지
    }



    public class Message
    {
        public Type type;

        public Message(Type type)
        {
            this.type = type;
        }
    }

    public class KeyMessage : Message
    {
        public int keyData;
        public float x;
        public float y;
        public float z;

        public KeyMessage(int data, Vector3 pos) : base(Type.Key)
        {
            this.keyData = data;
            this.x = pos.x;
            this.y = pos.y;
            this.z = pos.z;
        }
    }

    public class PlayerMoveMessage : Message
    {
        public SessionId playerSession;
        public float xPos;
        public float yPos;
        public float zPos;
        public float xDir;
        public float yDir;
        public float zDir;
        public PlayerMoveMessage(SessionId session, Vector3 pos, Vector3 dir) : base(Type.PlayerMove)
        {
            this.playerSession = session;
            this.xPos = pos.x;
            this.yPos = pos.y;
            this.zPos = pos.z;
            this.xDir = dir.x;
            this.yDir = dir.y;
            this.zDir = dir.z;
        }
    }

    public class PlayerAttackMessage : Message
    {
        public SessionId playerSession;
        public float dir_x;
        public float dir_y;
        public float dir_z;
        public PlayerAttackMessage(SessionId session, Vector3 pos) : base(Type.PlayerAttack)
        {
            this.playerSession = session;
            dir_x = pos.x;
            dir_y = pos.y;
            dir_z = pos.z;
        }
    }

    public class PlayerDamegedMessage : Message
    {
        public SessionId playerSession;
        public float hit_x;
        public float hit_y;
        public float hit_z;
        public PlayerDamegedMessage(SessionId session, float x, float y, float z) : base(Type.PlayerDamaged)
        {
            this.playerSession = session;
            this.hit_x = x;
            this.hit_y = y;
            this.hit_z = z;
        }
    }

    public class PlayerNoMoveMessage : Message
    {
        public SessionId playerSession;
        public float xPos;
        public float yPos;
        public float zPos;
        public PlayerNoMoveMessage(SessionId session, Vector3 pos) : base(Type.PlayerNoMove)
        {
            this.playerSession = session;
            this.xPos = pos.x;
            this.yPos = pos.y;
            this.zPos = pos.z;
        }
    }

    public class AIPlayerInfo : Message
    {
        public SessionId m_sessionId;
        public string m_nickname;
        public byte m_teamNumber;
        public int m_numberOfMatches;
        public int m_numberOfWin;
        public int m_numberOfDraw;
        public int m_numberOfDefeats;
        public int m_points;
        public int m_mmr;

        public AIPlayerInfo(MatchUserGameRecord gameRecord) : base(Type.AIPlayerInfo)
        {
            this.m_sessionId = gameRecord.m_sessionId;
            this.m_nickname = gameRecord.m_nickname;
            this.m_teamNumber = gameRecord.m_teamNumber;
            this.m_numberOfWin = gameRecord.m_numberOfWin;
            this.m_numberOfDraw = gameRecord.m_numberOfDraw;
            this.m_numberOfDefeats = gameRecord.m_numberOfDefeats;
            this.m_points = gameRecord.m_points;
            this.m_mmr = gameRecord.m_mmr;
            this.m_numberOfMatches = gameRecord.m_numberOfMatches;
        }

        public MatchUserGameRecord GetMatchRecord()
        {
            MatchUserGameRecord gameRecord = new MatchUserGameRecord();
            gameRecord.m_sessionId = this.m_sessionId;
            gameRecord.m_nickname = this.m_nickname;
            gameRecord.m_numberOfMatches = this.m_numberOfMatches;
            gameRecord.m_numberOfWin = this.m_numberOfWin;
            gameRecord.m_numberOfDraw = this.m_numberOfDraw;
            gameRecord.m_numberOfDefeats = this.m_numberOfDefeats;
            gameRecord.m_mmr = this.m_mmr;
            gameRecord.m_points = this.m_points;
            gameRecord.m_teamNumber = this.m_teamNumber;

            return gameRecord;
        }
    }

    public class LoadRoomSceneMessage : Message
    {
        public LoadRoomSceneMessage() : base(Type.LoadRoomScene)
        {

        }
    }

    public class LoadGameSceneMessage : Message
    {
        public LoadGameSceneMessage() : base(Type.LoadGameScene)
        {

        }
    }

    public class StartCountMessage : Message
    {
        public int time;
        public StartCountMessage(int time) : base(Type.StartCount)
        {
            this.time = time;
        }
    }

    public class GameStartMessage : Message
    {
        public GameStartMessage() : base(Type.GameStart) { }
    }

    public class GameEndMessage : Message
    {
        public int count;
        public int[] sessionList;
        public GameEndMessage(Stack<SessionId> result) : base(Type.GameEnd)
        {
            count = result.Count;
            sessionList = new int[count];
            for (int i = 0; i < count; ++i)
            {
                sessionList[i] = (int)result.Pop();
            }
        }
    }

    public class GameSyncMessage : Message
    {
        public SessionId host;
        public int count = 0;
        public float[] xPos = null;
        public float[] zPos = null;
        public int[] hpValue = null;
        public bool[] onlineInfo = null;

        public GameSyncMessage(SessionId host, int count, float[] x, float[] z, int[] hp, bool[] online) : base(Type.GameSync)
        {
            this.host = host;
            this.count = count;
            this.xPos = new float[count];
            this.zPos = new float[count];
            this.hpValue = new int[count];
            this.onlineInfo = new bool[count];

            for (int i = 0; i < count; ++i)
            {
                xPos[i] = x[i];
                zPos[i] = z[i];
                hpValue[i] = hp[i];
                onlineInfo[i] = online[i];
            }
        }
    }
}
