using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EatNumberOPCard : MonoBehaviour
{
    public Text MulText;
    public string text;

    private int number;

    private void Awake()
    {
        number = Random.Range(0,3);
        switch(number)
        {
            case 0:
                MulText.text = "+";
                text = "+";
                break;
            case 1:
                MulText.text = "-";
                text = "-";
                break;
            case 2:
                MulText.text = "X";
                text = "X";
                break;
            case 3:
                MulText.text = "/";
                text = "/";
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= 1)
        {
            Destroy(gameObject);
        }

        if (FindObjectOfType<EatNumberTimer>().timerRunning == false)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
