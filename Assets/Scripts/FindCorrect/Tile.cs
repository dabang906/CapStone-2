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
    public Text questionText;

    string expression;
    float result;
    List<int> numbers = new List<int>();
    // Start is called before the first frame update
    void Start()
    {
        GenerateRandomExpression();
        correct.GetComponentInChildren<Text>().text = result.ToString();
        wrong.GetComponentInChildren<Text>().text = (result-2).ToString();
        RandNum();
        Instantiate(correct, spawnPoints[numbers[0]].transform.position,Quaternion.identity);
        Instantiate(wrong, spawnPoints[numbers[1]].transform.position, Quaternion.identity);
        Instantiate(wrong, spawnPoints[numbers[2]].transform.position, Quaternion.identity);
    }

    void RandNum()
    {
        while (numbers.Count < 3)
        {
            int randomNumber = Random.Range(0, 3);

            if (!numbers.Contains(randomNumber))
            {
                numbers.Add(randomNumber);
            }
        }
    }
    void GenerateRandomExpression()
    {
        int num1 = Random.Range(1, 10); // 1���� 99������ ���� ����
        int num2 = Random.Range(1, 10); // 1���� 99������ ���� ����

        string[] operators = { "+", "-", "*", "/" };
        string op = operators[Random.Range(0, operators.Length)]; // ������ �迭���� �������� ����

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

        questionText.text = expression;
        Debug.Log("Generated expression: " + expression + " Result: " + result);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
