using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BackEnd;
using System.Text;

public class BackendChart
{
    private static BackendChart _instance = null;

    public static BackendChart Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new BackendChart();
            }

            return _instance;
        }
    }

    public void ChartGet(string chartId)
    {
        Debug.Log($"{chartId}의 차트 불러오기를 요청합니다.");
        var bro = Backend.Chart.GetChartContents(chartId);

        if (bro.IsSuccess() == false)
        {
            Debug.LogError($"{chartId}의 차트를 불러오는 중, 에러가 발생했습니다. : " + bro);
            return;
        }

        Debug.Log("차트 불러오기에 성공했습니다. : " + bro);
        foreach (LitJson.JsonData gameData in bro.FlattenRows())
        {
            StringBuilder content = new StringBuilder();
            content.AppendLine("itemID : " + int.Parse(gameData["itemId"].ToString()));
            content.AppendLine("itemName : " + gameData["itemName"].ToString());
            content.AppendLine("itemType : " + gameData["itemType"].ToString());
            content.AppendLine("itemID : " + long.Parse(gameData["itemPower"].ToString()));
            content.AppendLine("itemInfo : " + gameData["itemInfo"].ToString());

            Debug.Log(content.ToString());
        }
    }
}
