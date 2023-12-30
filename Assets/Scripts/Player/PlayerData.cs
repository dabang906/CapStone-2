using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BackEnd;
using System.Text;
using Unity.VisualScripting;

public class PlayerData : MonoBehaviour
{
    public static UserData userData;

    private static PlayerData instance = null;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
        // 모든 씬에서 유지
        DontDestroyOnLoad(this.gameObject);
    }

    public static PlayerData GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("PlayerData 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return instance;
    }

    private string gameDataRowInDate = string.Empty;

    public void GameDataInsert()
    {
        if (userData == null)
        {
            userData = new UserData();
        }

        Debug.Log("데이터를 초기화합니다.");
        userData.player1coin = 10;
        userData.player1routePosition = 0;
        userData.player1result = 0;
        userData.player1state = 0;
        userData.player2coin = 10;
        userData.player2routePosition = 0;
        userData.player2result = 0;
        userData.turn = 4;

        Debug.Log("뒤끝 업데이트 목록에 해당 데이터들을 추가합니다.");
        Param param = new Param();
        param.Add("player1coin", userData.player1coin);
        param.Add("player1routePosition", userData.player1routePosition);
        param.Add("player1result", userData.player1result);
        param.Add("player1state", userData.player1state);
        param.Add("player2coin", userData.player2coin);
        param.Add("player2routePosition", userData.player2routePosition);
        param.Add("player2result", userData.player2result);
        param.Add("turn", userData.turn);

        Debug.Log("게임정보 데이터 삽입을 요청합니다.");
        var bro = Backend.GameData.Insert("USER_DATA", param);

        if (bro.IsSuccess())
        {
            Debug.Log("게임정보 데이터 삽입에 성공했습니다. : " + bro);

            //삽입한 게임정보의 고유값입니다.
            gameDataRowInDate = bro.GetInDate();
        }
        else
        {
            Debug.LogError("게임정보 데이터 삽입에 실패했습니다. : " + bro);
        }
    }

    public int GameDataGet(string value)
    {
        Debug.Log("게임 정보 조회 함수를 호출합니다.");
        var bro = Backend.GameData.GetMyData("USER_DATA", new Where());
        if (bro.IsSuccess())
        {
            Debug.Log("게임 정보 조회에 성공했습니다. : " + bro);


            LitJson.JsonData gameDataJson = bro.FlattenRows(); // Json으로 리턴된 데이터를 받아옵니다.

            // 받아온 데이터의 갯수가 0이라면 데이터가 존재하지 않는 것입니다.
            if (gameDataJson.Count <= 0)
            {
                Debug.LogWarning("데이터가 존재하지 않습니다.");
            }
            else
            {
                gameDataRowInDate = gameDataJson[0]["inDate"].ToString(); //불러온 게임정보의 고유값입니다.

                userData = new UserData();

                userData.player1coin = int.Parse(gameDataJson[0]["player1coin"].ToString());
                userData.player1routePosition = int.Parse(gameDataJson[0]["player1routePosition"].ToString());
                userData.player1result = int.Parse(gameDataJson[0]["player1result"].ToString());
                userData.player1state = int.Parse(gameDataJson[0]["player1state"].ToString());
                userData.player2coin = int.Parse(gameDataJson[0]["player2coin"].ToString());
                userData.player2routePosition = int.Parse(gameDataJson[0]["player2routePosition"].ToString());
                userData.player2result = int.Parse(gameDataJson[0]["player2result"].ToString());
                userData.turn = int.Parse(gameDataJson[0]["turn"].ToString());

                Debug.Log(userData.player1coin.ToString());

                switch(value)
                {
                    case "player1coin":
                        return userData.player1coin;
                    case "player1routePosition":
                        return userData.player1routePosition;
                    case "player1result":
                        return userData.player1result;
                    case "player1state":
                        return userData.player1state;
                    case "player2coin":
                        return userData.player2coin;
                    case "player2routePosition":
                        return userData.player2routePosition;
                    case "player2result":
                        return userData.player2result;
                    case "turn":
                        return userData.turn;
                }
            }
            return 0;
        }
        else
        {
            Debug.LogError("게임 정보 조회에 실패했습니다. : " + bro);
            return 0;
        }
    }

    public void Player1CoinUp()
    {
        Debug.Log("코인을 5 증가시킵니다.");
        userData.player1coin += 5;
    }

    public void Player1CoinDown()
    {
        Debug.Log("코인을 5 감소시킵니다.");
        userData.player1coin -= 5;
    }

    public void Player2CoinUp()
    {
        Debug.Log("코인을 5 증가시킵니다.");
        userData.player2coin += 5;
    }

    public void Player2CoinDown()
    {
        Debug.Log("코인을 5 감소시킵니다.");
        userData.player2coin -= 5;
    }

    public void Player1Route(int route)
    {
        userData.player1routePosition = route;
    }
    public void Player2Route(int route)
    {
        userData.player2routePosition = route;
    }
    public void Player1Double()
    {
        userData.player1state = 1;
    }
    public void Player2Double()
    {
        userData.player2state = 1;
    }
    public void TurnDown()
    {
        userData.turn -= 1;
    }
    public void GameDataUpdate()
    {
        if (userData == null)
        {
            Debug.LogError("서버에서 다운받거나 새로 삽입한 데이터가 존재하지 않습니다. Insert 혹은 Get을 통해 데이터를 생성해주세요.");
            return;
        }

        Param param = new Param();
        param.Add("player1coin", userData.player1coin);
        param.Add("player1routePosition", userData.player1routePosition);
        param.Add("player2coin", userData.player2coin);
        param.Add("player2routePosition", userData.player2routePosition);
        param.Add("turn", userData.turn);

        BackendReturnObject bro = null;

        if (string.IsNullOrEmpty(gameDataRowInDate))
        {
            Debug.Log("내 제일 최신 게임정보 데이터 수정을 요청합니다.");

            bro = Backend.GameData.Update("USER_DATA", new Where(), param);
        }
        else
        {
            Debug.Log($"{gameDataRowInDate}의 게임정보 데이터 수정을 요청합니다.");

            bro = Backend.GameData.UpdateV2("USER_DATA", gameDataRowInDate, Backend.UserInDate, param);
        }

        if (bro.IsSuccess())
        {
            Debug.Log("게임정보 데이터 수정에 성공했습니다. : " + bro);
        }
        else
        {
            Debug.LogError("게임정보 데이터 수정에 실패했습니다. : " + bro);
        }
    }

    public void Player1Result(int result)
    {
        userData.player1result = result;
    }
    public void Player2Result(int result)
    {
        userData.player2result = result;
    }
}

public class UserData
{
    public int player1coin = 10;
    public int player1routePosition = 0;
    public int player1result = 0;
    public int player1state = 0;
    public int player2coin = 10;
    public int player2routePosition = 0;
    public int player2result = 0;
    public int turn = 4;
    public int player2state = 0;
}