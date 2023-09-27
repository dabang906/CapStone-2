using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinUi : MonoBehaviour
{
    int coin = 20;
    public Text coinText;
    void Start()
    {
        coinText.text = coin.ToString();
    }
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Red")
        {
            
        }
    }
}
