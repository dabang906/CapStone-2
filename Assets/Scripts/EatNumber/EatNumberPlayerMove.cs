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
    private int number;

    private void Start()
    {
        mul = 1;
    }


    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-1, 0, 0) * movePower * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(1, 0, 0) * movePower * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Box")
        {
            mul = mul * other.GetComponent<Card>().number;
            text.text = "Number : " + mul.ToString();
        }
    }
}