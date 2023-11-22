using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Stone : MonoBehaviour
{
    public enum GameState
    {
        Idle,
        DicePlus,
        AllUp,
        OpenShop,
    }
    public GameState diceState;
    private static Stone instance = null;
    public Route currentRoute;
    public int count = 0;

    int routePosition;
    public int coin = 10;
    public static int diceNum;
    public int steps;
    public GameObject shop;

    List<int> numbers = new List<int>();
    Dice dice;
    bool isMoving;
    SphereCollider sphereCollider;
    bool hasRolledDice;  // 주사위를 굴렸는지 여부를 저장하는 변수

    void Awake() {
        #region 싱글톤
        /*
        if (null == instance)
        {
            //이 클래스 인스턴스가 탄생했을 때 전역변수 instance에 게임매니저 인스턴스가 담겨있지 않다면, 자신을 넣어준다.
            instance = this;

            //씬 전환이 되더라도 파괴되지 않게 한다.
            //gameObject만으로도 이 스크립트가 컴포넌트로서 붙어있는 Hierarchy상의 게임오브젝트라는 뜻이지만, 
            //나는 헷갈림 방지를 위해 this를 붙여주기도 한다.
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            //만약 씬 이동이 되었는데 그 씬에도 Hierarchy에 GameMgr이 존재할 수도 있다.
            //그럴 경우엔 이전 씬에서 사용하던 인스턴스를 계속 사용해주는 경우가 많은 것 같다.
            //그래서 이미 전역변수인 instance에 인스턴스가 존재한다면 자신(새로운 씬의 GameMgr)을 삭제해준다.
            Destroy(this.gameObject);
        }*/
        #endregion
        if (PlayerPrefs.HasKey("routePosition"))
        {
            routePosition = PlayerPrefs.GetInt("routePosition")-1;
            this.transform.position = currentRoute.childNodeList[routePosition/2].position;
            PlayerPrefs.DeleteAll();
        }
        RandNum();
        dice = FindObjectOfType<Dice>();
        if(routePosition == 0) routePosition+=2;
        sphereCollider = GetComponentInChildren<SphereCollider>();
        sphereCollider.isTrigger = false;
    }
    void Update()
    {
        if (!isMoving && diceNum != 0 && !hasRolledDice)
        {
            steps = diceNum * 2;
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

        switch (diceState)
        {
            case GameState.Idle: break;
            case GameState.DicePlus: steps = steps * 2; break;
            case GameState.AllUp: AllCoinUp(); break;
            default: break;
        }

        while (steps > 0)
        {
            sphereCollider.isTrigger = false;
            if (currentRoute.childNodeList.Count == 0)
            {
                Debug.Log("No nodes in the route.");
                yield break;
            }

            routePosition %= currentRoute.childNodeList.Count;
            //if (routePosition == 0) coin += 10;
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

        switch (diceState)
        {
            case GameState.OpenShop: OpenShop(); break;
        }

        if (count %2 == 0)
        {
            PlayerPrefs.SetInt("routePosition", routePosition);
            SceneManager.LoadScene(numbers[UnityEngine.Random.RandomRange(0,4)]);
        }
    }

    bool MoveToNextNode(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, 2f * Time.deltaTime));
    }
    void RandNum()
    {
        while (numbers.Count < 3)
        {
            int randomNumber = UnityEngine.Random.Range(4, 8);

            if (!numbers.Contains(randomNumber))
            {
                numbers.Add(randomNumber);
            }
        }
    }
    public void AllCoinUp()
    {
        coin += 3;
    }
    public void OpenShop()
    {
        shop.SetActive(true);
    }
}