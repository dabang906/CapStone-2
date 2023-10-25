using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSelector : MonoBehaviour
{
    private ObjectDetector objectDetector;

    private void Awake()
    {
        objectDetector = GetComponent<ObjectDetector>();

        //���̾ touchable�� ������Ʈ�� ���� �����ϵ��� ObjectDetector�� latermask ����
        objectDetector.raycastEvent.AddListener(SelectCube);
    }

    public void SelectCube(Transform hit)
    {
        //���õ� ������Ʈ�� cubecontroller.changecolor�� ȣ���� ť�� ���� ����
        hit.GetComponent<CubeController>().ChangeColor();
    }    

}
