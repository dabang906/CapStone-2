using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using BackEnd;

public class BackendManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var bro = Backend.Initialize(true);

        if (bro.IsSuccess())
        {
            Debug.Log("초기화 성공 : " + bro);
        }
        else
        {
            Debug.LogError("초기화 실패 : " + bro);
        }

        Test();
    }

    async void Test()
    {
        await Task.Run(() =>
        {
            BackendLogin.Instance.CustomLogin("user1", "1234");

            //BackendGameData.Instance.GameDataInsert();
            //BackendGameData.Instance.GameDataGet();

            //if (BackendGameData.userData == null)
            //{
            //    BackendGameData.Instance.GameDataInsert();
            //}

            //BackendGameData.Instance.LevelUp();

            //BackendGameData.Instance.GameDataUpdate();

            //BackendRank.Instance.RankInsert(100);
            //BackendRank.Instance.RankGet();

            BackendChart.Instance.ChartGet("95859");

            Debug.Log("테스트를 종료합니다.");
        });
    }
}