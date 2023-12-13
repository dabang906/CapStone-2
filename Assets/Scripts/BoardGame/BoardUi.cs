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
                InGameUiManager.GetInstance().coinTextUpdate(stone.coin);
            }
            if (stone.gameObject.tag == "Player2")
            {
                stone.coin -= 5;
                PlayerData.GetInstance().Player2CoinDown();
                InGameUiManager.GetInstance().coinTextUpdate(stone.coin);
            }
        }
        if (other.gameObject.tag == "Blue")
        {
            if(stone.gameObject.tag == "Player1")
            {
                stone.coin += 5;
                PlayerData.GetInstance().Player1CoinUp();
                InGameUiManager.GetInstance().coinTextUpdate(stone.coin);
            }
            if (stone.gameObject.tag == "Player2")
            {
                stone.coin += 5;
                PlayerData.GetInstance().Player2CoinUp();
                InGameUiManager.GetInstance().coinTextUpdate(stone.coin);
            }
        }
        if(other.gameObject.tag == "Lucky")
        {
            InGameUiManager.GetInstance().OnLucky();
        }
    }
}
