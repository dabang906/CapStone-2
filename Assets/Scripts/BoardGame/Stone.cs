using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Stone : MonoBehaviour
{
    public Route currentRoute;

    int routePosition;

    public static int diceNum;
    public int steps;

    bool isMoving;

    bool hasRolledDice;  // 주사위를 굴렸는지 여부를 저장하는 변수
    private void Awake() {
        routePosition++;
    }
    void Update()
    {
        if (!isMoving && diceNum != 0 && !hasRolledDice)
        {
            DiceCheckZone.isTrigger = false;
            steps = diceNum;
            Debug.Log("Dice Rolled: " + steps);
            hasRolledDice = true;  // 주사위를 굴렸음을 표시
            StartCoroutine(Move());
        }
    }

    IEnumerator Move()
    {
        if (isMoving)
        {
            yield break;
        }

        isMoving = true;

        while (steps > 0)
        {
            if (currentRoute.childNodeList.Count == 0)
            {
                Debug.Log("No nodes in the route.");
                yield break;
            }

            routePosition %= currentRoute.childNodeList.Count;

            Vector3 nextPos = currentRoute.childNodeList[routePosition].position;
            while (MoveToNextNode(nextPos))
            {
                yield return null;

            }

            yield return new WaitForSeconds(0.1f);
            Debug.Log(steps);
            steps--;
            routePosition++;

        }
        isMoving = false;
        hasRolledDice = false;  // 이동이 끝났으므로 다음 주사위 굴림을 허용
        diceNum = 0;
    }

    bool MoveToNextNode(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, 2f * Time.deltaTime));
    }
}