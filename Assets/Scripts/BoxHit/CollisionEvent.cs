using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionEvent : MonoBehaviour
{
    public Text text;

    int count;
    int mul;

    void Start()
    {
        int rand1 = Random.Range(41, 100);
        int rand2 = Random.Range(3, 8);

        count = rand1;
        mul = rand2;

        text.text = rand1.ToString();
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            count = count - mul;
            text.text = count.ToString();
        }
    }
    
}
