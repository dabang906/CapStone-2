using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BoardUi : MonoBehaviour
{
    public Text coinText;
    public GameObject luckyImage;
    Stone stone;
    void Start()
    {
        PlayerData.GetInstance().GameDataUpdate();
        stone = FindObjectOfType<Stone>();
        stone.coin = PlayerData.GetInstance().GameDataGet("coin");
        coinText.text = stone.coin.ToString();
    }
    void Update()
    {
        coinText.text = "Coin : " + stone.coin.ToString();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Red")
        {
            stone.coin -= 5;
            PlayerData.GetInstance().CoinDown();
            coinText.text = stone.coin.ToString();
        }
        if (other.gameObject.tag == "Blue")
        {
            stone.coin += 5;
            PlayerData.GetInstance().CoinUp();
            coinText.text = stone.coin.ToString();
        }
        if(other.gameObject.tag == "Lucky")
        {
            luckyImage.SetActive(true);
        }
    }
}
