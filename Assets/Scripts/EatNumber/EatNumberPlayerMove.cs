using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Eatnum1PlayerMove : MonoBehaviour
{
    public Text text;

    public int movePower;

    private int num1;
    private int num2;
    private int result;
    private int numCount;
    private int resultCount;
    private string op;
    private bool freeze;

    private void Start()
    {
        numCount = 0;
        resultCount = 0;
        num1 = 0;
        num2 = 0;

        Freeze();

        Invoke("UnFreeze", 5.0f);
        Invoke("Freeze", 35.0f);
    }

    private void FixedUpdate()
    {
        if (freeze) { 
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
            if (numCount == 0)
            {
                num1 = other.GetComponent<EatNumberCard>().number;
                Debug.Log("num1 eat");

                numCount = 1;
            }
            else if (numCount == 1)
            {
                num2 = other.GetComponent<EatNumberCard>().number;
                Debug.Log("num2 eat");

                numCount = 0;
            }
            if (resultCount == 1)
            {
                switch (op)
                {
                    case "+":
                        result = num1 + num2;
                        Debug.Log("+");
                        break;
                    case "-":
                        result = num1 - num2;
                        Debug.Log("-");
                        break;
                    case "*":
                        result = num1 * num2;
                        Debug.Log("X");
                        break;
                    case "/":
                        result = num1 / num2;
                        Debug.Log("/");
                        break;
                }
            }
            else
            {
                switch(numCount)
                {
                    case 0:
                        switch (op)
                        {
                            case "+":
                                result = result + num2;
                                Debug.Log("+");
                                break;
                            case "-":
                                result = result - num2;
                                Debug.Log("-");
                                break;
                            case "X":
                                result = result * num2;
                                Debug.Log("X");
                                break;
                            case "/":
                                result = result / num2;
                                Debug.Log("/");
                                break;
                        }
                        break;
                    case 1:
                        switch (op)
                        {
                            case "+":
                                result = result + num1;
                                Debug.Log("+");
                                break;
                            case "-":
                                result = result - num1;
                                Debug.Log("-");
                                break;
                            case "X":
                                result = result * num1;
                                Debug.Log("X");
                                break;
                            case "/":
                                result = result / num1;
                                Debug.Log("/");
                                break;
                        }
                        break;
                }
            }
            text.text = "Number : " + result.ToString();
        }
        if(other.tag == "OPBox")
        {
            op = other.GetComponent<EatNumberOPCard>().text;
            Debug.Log("op eat");
            resultCount++;
        }
    }

    void Freeze()
    {
        freeze = false;
    }

    void UnFreeze()
    {
        freeze = true;
    }
}