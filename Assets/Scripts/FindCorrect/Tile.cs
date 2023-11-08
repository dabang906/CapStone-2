using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public GameObject[] spawnPoints;
    public GameObject correct;
    public GameObject wrong;

    string expression; // 생성된 랜덤 식
    float result; // 식의 결과
    // Start is called before the first frame update
    void Start()
    {
        GenerateRandomExpression();
        
    }


    void GenerateRandomExpression()
    {
        int num1 = Random.Range(1, 10); // 1부터 99까지의 랜덤 정수
        int num2 = Random.Range(1, 10); // 1부터 99까지의 랜덤 정수

        string[] operators = { "+", "-", "*", "/" };
        string op = operators[Random.Range(0, operators.Length)]; // 연산자 배열에서 무작위로 선택

        switch (op)
        {
            case "+":
                expression = num1 + " " + op + " " + num2;
                result = num1 + num2;
                break;
            case "-":
                expression = num1 + " " + op + " " + num2;
                result = num1 - num2;
                break;
            case "*":
                expression = num1 + " " + op + " " + num2;
                result = num1 * num2;
                break;
            case "/":
                if (num2 != 0)
                {
                    expression = (num1 * num2) + " / " + num2;
                    result = (float)(num1 * num2) / (float)num2;
                    break;
                }
                else
                {
                    GenerateRandomExpression();
                    return;
                }

            default:
                Debug.LogError("Invalid operator");
                return;
        }

        cardText.text = expression;// 텍스트 컴포넌트에 숫자식 할당 

        Debug.Log("Generated expression: " + expression + " Result: " + result);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
