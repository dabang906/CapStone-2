using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardGameCard : MonoBehaviour
{
    public float rotationSpeed = 1f; // ȸ�� �ӵ�. ���� �����Ͽ� ȸ���ϴ� �� �ɸ��� �ð� ���� ����
    public Text cardText;

    public static int currentIndex = 0;
    public int cardIndex;
    public int cardNum;

    bool turn = false;
    string expression; // ������ ���� ��
    float result; // ���� ���

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

        cardText.text = expression;// �ؽ�Ʈ ������Ʈ�� ���ڽ� �Ҵ� 

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
