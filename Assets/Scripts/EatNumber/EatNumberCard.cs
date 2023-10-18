using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EatNumberCard : MonoBehaviour
{
    public Text MulText;
    public int number;

    private void Awake()
    {
        number = Random.Range(2, 6);
        MulText.text = number.ToString();
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
