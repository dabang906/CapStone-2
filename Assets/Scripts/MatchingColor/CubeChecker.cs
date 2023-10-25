using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeChecker : MonoBehaviour
{
    [SerializeField]
    private CubeSpawner cubeSpawner;
    [SerializeField]
    private GameController gameController;

    private CubeController[] touchCubes; // 치 가능한 큐브셋의 큐브들

    private int correctCount = 0;
    private int incorrectCount = 0;

    public int CorrectCount
    {
        set => correctCount = Mathf.Max(0, value);
        get => correctCount;
    }

    public int IncorrectCount
    {
        set => incorrectCount = Mathf.Max(0, value);
        get => incorrectCount;
    }

    private void Awake()
    {
        touchCubes = GetComponentsInChildren<CubeController>();

        for (int i = 0; i < touchCubes.Length; ++i) 
        {
            touchCubes[i].Setup(cubeSpawner, this);
        }
    }

    private void Update()
    {
        //맞은 개수 + 틀린 개수가 터치 가능한 큐브의 개수와 같을 때
        if(CorrectCount + IncorrectCount == touchCubes.Length)
        {
            //하나도 틀리지 않았을때 : 성공
            if (IncorrectCount == 0)
            {
                gameController.IncreaseScore();
            }
            //하나라도 틀렸을때 : 실패
            else
            {
                Debug.Log("Failed");
            }
            CorrectCount = 0;
            IncorrectCount = 0;
        }
    }
}
