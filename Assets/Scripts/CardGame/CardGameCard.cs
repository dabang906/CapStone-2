using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGameCard : MonoBehaviour
{
    public float rotationSpeed = 1f; // 회전 속도. 값을 조절하여 회전하는 데 걸리는 시간 조정 가능
    public Text cardText;

    public static int currentIndex = 0;
    public int cardIndex;
    public int cardNum;

    bool turn = false;
    string expression; // 생성된 랜덤 식
    float result; // 식의 결과

    CardSpawnManager spawnManager;
    void Awake()
    {
        GenerateRandomExpression();
        spawnManager = FindObjectOfType<CardSpawnManager>();
        spawnManager.cards.Add(cardNum);
        cardIndex = currentIndex;
        currentIndex++;
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
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == this.transform && !turn)
                {
                    StartCoroutine(RotateMe(Vector3.forward * 180, rotationSpeed));
                    Debug.Log(cardIndex);
                    turn = true;
                }
            }
        }
    }

    IEnumerator RotateMe(Vector3 byAngles, float inTime)
    {
        var fromAngle = transform.rotation;
        var toAngle = Quaternion.Euler(transform.eulerAngles + byAngles);
        for (var t = 0f; t < 1; t += Time.deltaTime / inTime)
        {
            transform.rotation = Quaternion.Slerp(fromAngle, toAngle, t);
            yield return null;
        }
        turn = false;
    }
}
