using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CubeSpawner : MonoBehaviour
{
    //���� ������������ ��� ������ ť���� ������ ��
    [SerializeField]
    private Color[] cubeColors;
    public Color[] CubeColors => cubeColors;

    // ť��� ������ ���� ����
    [SerializeField]
    private GameObject cubesetPrefab; // ť��� ������
    [SerializeField]
    private Transform spawnPoint;   //ť��� ���� ��ġ
    [SerializeField]
    private float spawnTime = 4.0f; //ť��� ���� �ֱ�
    Timer timer;
    private void Awake()
    {
        StartCoroutine(SpawnCubeSet());
        timer = FindObjectOfType<Timer>();
    }

    private IEnumerator SpawnCubeSet()
    {
        yield return new WaitForSeconds(13f);
        while (true)
        {
            GameObject clone = Instantiate(cubesetPrefab, spawnPoint.position, Quaternion.identity);
            //ť��� ������Ʈ�� �ڽ����� �ִ� 9�� ť���� meshRenderer ������Ʈ ����
            MeshRenderer[] renderers = clone.GetComponentsInChildren<MeshRenderer>();
            //9�� ť���� ������ ���Ƿ� ���� (���� ������������ ��� ������ ť�� �������� = CubeColors)
            for (int i = 0; i < renderers.Length; ++i)
            {
                int index = Random.Range(0, CubeColors.Length);
                renderers[i].material.color = CubeColors[index];
            }
            yield return new WaitForSeconds(spawnTime);
            if (!timer.timerRunning) break;
        }
        yield return new WaitForSeconds(1f);
    }
}
