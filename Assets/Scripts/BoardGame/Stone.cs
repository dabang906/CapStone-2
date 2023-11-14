using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Stone : MonoBehaviour
{
    public Route currentRoute;
    public int count = 0;

    int routePosition;
    public int coin = 20;
    public static int diceNum;
    public int steps;

    Dice dice;
    Transform playerTransform;
    bool isMoving;
    SphereCollider sphereCollider;
    bool hasRolledDice;  // 주사위를 굴렸는지 여부를 저장하는 변수
    void Awake() {
        dice = FindObjectOfType<Dice>();
        playerTransform= transform;
        routePosition++;
        sphereCollider = GetComponentInChildren<SphereCollider>();
        sphereCollider.isTrigger = true;
    }
    void Update()
    {
        if (!isMoving && diceNum != 0 && !hasRolledDice)
        {
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

        transform.rotation = Quaternion.LookRotation(playerTransform.forward);
        isMoving = true;

        while (steps > 0)
        {
            sphereCollider.isTrigger = false;
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
        dice.isRolling = false;  // 이동이 끝났으므로 다음 주사위 굴림을 허용
        sphereCollider.isTrigger = true;
        count++;
        diceNum = 0;
        hasRolledDice= false;

        if (count %4 == 0)
        {
            SceneManager.LoadScene(Random.RandomRange(4, 8));
        }
    }

    bool MoveToNextNode(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, 2f * Time.deltaTime));
    }

    void OnTriggerEnter(Collider other)
    {
        
    }
}