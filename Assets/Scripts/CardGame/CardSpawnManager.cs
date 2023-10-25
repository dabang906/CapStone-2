using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpawnManager : MonoBehaviour
{
    public List<int> cards = new List<int>();
    public GameObject[] CardSpawnPoint;
    public GameObject card;
    void Start()
    {
        for(int i = 0; i < CardSpawnPoint.Length; i++)
        {
            Instantiate(card, CardSpawnPoint[i].transform.position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
