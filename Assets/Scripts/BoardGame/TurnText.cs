using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnText : MonoBehaviour
{
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        PlayerData.GetInstance().GameDataUpdate();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        //text.text = "Turn : " + PlayerData.GetInstance().GameDataGet("turn");
    }
}
