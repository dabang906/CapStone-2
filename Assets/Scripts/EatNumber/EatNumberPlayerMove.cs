using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EatNumberPlayerMove : MonoBehaviour
{
    public Text text;

    public int movePower;

    private int mul;
    private bool pause;

    private void Start()
    {
        mul = 1;

        Freeze();

        Invoke("UnFreeze", 5.0f);
        Invoke("Freeze", 35.0f);
    }

    private void FixedUpdate()
    {
        if (pause) { 
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += Vector3.left * movePower * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.position += Vector3.right * movePower * Time.deltaTime;
            }
        }  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Box")
        {
            mul = mul * other.GetComponent<EatNumberCard>().number;
            text.text = "Number : " + mul.ToString();
        }
    }

    void Freeze()
    {
        pause = false;
    }

    void UnFreeze()
    {
        pause = true;
    }
}