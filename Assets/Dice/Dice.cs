using BackEnd;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Dice : MonoBehaviour {
    public Sprite[] diceImages; // 주사위 이미지 배열 (6면체 주사위 이미지를 가정)
    public float rollTime = 2f; // 주사위가 돌아가는 시간
    public int diceNum;
    public bool isRolling;
    public Stone stone;

    private Camera mainCamera;
    private Image image;
    string userId;
    private void Start()
    {
        stone = GetComponentInParent<Stone>();
        image = GetComponent<Image>();
        mainCamera = Camera.main;
        BackendReturnObject bro = Backend.BMember.GetUserInfo();
        if (bro.IsSuccess())
        {
            userId = bro.GetReturnValuetoJSON()["row"]["inDate"].ToString();
        }
    }

    private void Update()
    {
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling && stone.gameObject.tag == "Player1" && InGameUiManager.GetInstance().count % 2 == 0 &&
            userId == "2023-11-09T12:17:20.000Z")
        {
            StartCoroutine(RollDice());
        }
        if (Input.GetKeyDown(KeyCode.Space) && !isRolling && stone.gameObject.tag == "Player2" && InGameUiManager.GetInstance().count % 2 == 1 &&
            userId != "2023-11-09T12:17:20.000Z")
        {
            StartCoroutine(RollDice());
        }
    }

    private IEnumerator RollDice()
    {
        isRolling = true;

        float elapsedTime = 0f;
        int currentIndex = 0;
        int finalIndex = Random.Range(0, diceImages.Length);

        while (elapsedTime < rollTime)
        {
            // 주사위 이미지를 순차적으로 변경하여 돌아가는 효과를 만듦
            image.sprite = diceImages[currentIndex];

            currentIndex++;
            if (currentIndex >= diceImages.Length)
            {
                currentIndex = 0;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 주사위가 멈춘 후 최종 결과 이미지를 표시
        image.sprite = diceImages[finalIndex];
        stone.diceNum = finalIndex + 1;
    }
}
