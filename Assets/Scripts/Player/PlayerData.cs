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
        userData.coin = 10;
        userData.routePosition = 0;
        userData.turn = 8;

        Debug.Log("뒤끝 업데이트 목록에 해당 데이터들을 추가합니다.");
        Param param = new Param();
        param.Add("coin", userData.coin);
        param.Add("routePosition", userData.routePosition);
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

                userData.coin = int.Parse(gameDataJson[0]["coin"].ToString());
                userData.routePosition = int.Parse(gameDataJson[0]["routePosition"].ToString());
                userData.turn = int.Parse(gameDataJson[0]["turn"].ToString());

                Debug.Log(userData.coin.ToString());

                switch(value)
                {
                    case "coin":
                        return userData.coin;
                    case "routePosition":
                        return userData.routePosition;
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

    public void CoinUp()
    {
        Debug.Log("코인을 5 증가시킵니다.");
        userData.coin += 5;
    }

    public void CoinDown()
    {
        Debug.Log("코인을 5 감소시킵니다.");
        userData.coin -= 5;
    }

    public void GameDataUpdate()
    {
        if (userData == null)
        {
            Debug.LogError("서버에서 다운받거나 새로 삽입한 데이터가 존재하지 않습니다. Insert 혹은 Get을 통해 데이터를 생성해주세요.");
            return;
        }

        Param param = new Param();
        param.Add("coin", userData.coin);

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
}

public class UserData
{
    public int coin = 10;
    public int routePosition = 0;
    public int turn = 8;
}