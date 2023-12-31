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
        DoubleDice,
        GoldDice,
    }
    public GameState diceState;

    int routePosition;
    public int coin = 10;
    public int diceNum;
    public int steps;

    bool diceDouble = false;
    GameObject route;
    Dice dice;
    bool isMoving;
    SphereCollider sphereCollider;
    Route currentRoute;
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
        route = GameObject.Find("Route1");
        currentRoute = route.GetComponent<Route>();
        if (PlayerData.GetInstance().GameDataGet("turn") != 4 && gameObject.tag == "Player1")
        {
            routePosition = PlayerData.GetInstance().GameDataGet("player1routePosition")-1;
            this.transform.position = currentRoute.childNodeList[routePosition/2].position;
            if(PlayerData.GetInstance().GameDataGet("player1state") == 1) diceDouble = true;
        }
        if (PlayerData.GetInstance().GameDataGet("turn") != 4 && gameObject.tag == "Player2")
        {
            routePosition = PlayerData.GetInstance().GameDataGet("player2routePosition") - 1;
            this.transform.position = currentRoute.childNodeList[routePosition / 2].position;
            if (PlayerData.GetInstance().GameDataGet("player2state") == 1) diceDouble = true;
        }
        dice = GetComponentInChildren<Dice>();
        if(routePosition == 0) routePosition+=2;
        sphereCollider = GetComponentInChildren<SphereCollider>();
        sphereCollider.isTrigger = false;
        coin = PlayerData.GetInstance().GameDataGet("player1coin");
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

        if (diceDouble) { steps = steps * 2; diceDouble = false; }

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
        switch (diceState)
        {
            case GameState.AllUp: AllCoinUp(); break;
        }
        if (gameObject.tag == "Player1") { PlayerData.GetInstance().Player1Route(routePosition); }
        if (gameObject.tag == "Player2") { PlayerData.GetInstance().Player2Route(routePosition); }
        diceNum = 0;
        hasRolledDice= false;
        PlayerData.GetInstance().GameDataUpdate();
        
    }

    bool MoveToNextNode(Vector3 goal)
    {
        return goal != (transform.position = Vector3.MoveTowards(transform.position, goal, 2f * Time.deltaTime));
    }
    
    public void AllCoinUp()
    {
        PlayerData.GetInstance().Player1CoinUp();
        PlayerData.GetInstance().Player2CoinUp();
    }
    public void OpenShop()
    {
 //       shop.SetActive(true);
    }
}