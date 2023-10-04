using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public float rotationSpeed = 1f; // 회전 속도. 값을 조절하여 회전하는 데 걸리는 시간 조정 가능
    public Text cardText;

    public static int currentIndex = 0;
    public int cardIndex;
    public int cardNum;
    bool turn = false;

    CardSpawnManager spawnManager;
    void Awake()
    {
        cardNum = Random.RandomRange(1, 9);
        cardText.text = cardNum.ToString();
        spawnManager = FindObjectOfType<CardSpawnManager>();
        spawnManager.cards.Add(cardNum);
        cardIndex = currentIndex;
        currentIndex++;
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
