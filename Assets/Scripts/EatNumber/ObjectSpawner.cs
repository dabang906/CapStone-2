using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject prefabToSpawn; // 생성할 프리팹
    public float spawnInterval = 2.0f; // 생성 간격 (초)
    public float initialDelay = 1.0f; // 시작 딜레이 (초)

    void Start()
    {
        // 시작 딜레이 후에 일정 간격으로 프리팹 생성 시작
        InvokeRepeating("SpawnPrefab", initialDelay, spawnInterval);
    }

    void SpawnPrefab()
    {
        // 프리팹을 코드로 생성
        GameObject newObject = Instantiate(prefabToSpawn);

        // 생성된 프리팹을 원하는 위치로 이동
        newObject.transform.position = transform.position;

        // 원하는 초기화 작업 수행
        // 예: newObject.GetComponent<MyScript>().Initialize(); // 스크립트 초기화
    }
}
