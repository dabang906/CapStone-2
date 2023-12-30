using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoardUi : MonoBehaviour
{
    public Stone stone;
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Red")
        {
            if(stone.gameObject.tag == "Player1")
            {
                stone.coin -= 5;
                PlayerData.GetInstance().Player1CoinDown();
                PlayerData.GetInstance().GameDataUpdate();
                InGameUiManager.GetInstance().player1CoinTextUpdate(stone.coin);
                InGameUiManager.GetInstance().UpdateCount();
            }
            if (stone.gameObject.tag == "Player2")
            {
                stone.coin -= 5;
                PlayerData.GetInstance().Player2CoinDown();
                PlayerData.GetInstance().GameDataUpdate();
                InGameUiManager.GetInstance().player2CoinTextUpdate(stone.coin);
                InGameUiManager.GetInstance().UpdateCount();
            }
        }
        if (other.gameObject.tag == "Blue")
        {
            if(stone.gameObject.tag == "Player1")
            {
                stone.coin += 5;
                PlayerData.GetInstance().Player1CoinUp();
                PlayerData.GetInstance().GameDataUpdate();
                InGameUiManager.GetInstance().player1CoinTextUpdate(stone.coin);
                InGameUiManager.GetInstance().UpdateCount();
            }
            if (stone.gameObject.tag == "Player2")
            {
                stone.coin += 5;
                PlayerData.GetInstance().Player2CoinUp();
                PlayerData.GetInstance().GameDataUpdate();
                InGameUiManager.GetInstance().player2CoinTextUpdate(stone.coin);
                InGameUiManager.GetInstance().UpdateCount();
            }
        }
        if(other.gameObject.tag == "White") InGameUiManager.GetInstance().UpdateCount();
        if (other.gameObject.tag == "Lucky")
        {

            InGameUiManager.GetInstance().OnLucky();
        }
    }
}
