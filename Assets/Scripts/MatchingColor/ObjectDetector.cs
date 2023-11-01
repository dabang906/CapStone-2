using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class RaycastEvent : UnityEvent<Transform> { }

public class ObjectDetector : MonoBehaviour
{
    [HideInInspector]
    public RaycastEvent raycastEvent = new RaycastEvent();

    [SerializeField]
    private LayerMask layerMask;
    private Camera maincamera;
    private Ray ray;
    private RaycastHit hit;

    private void Awake()
    {
        //"maincamera" �±׸� ������ �ִ� ������Ʈ Ž�� �� camera ������Ʈ ���� ����
        maincamera = Camera.main;
    }

    private void Update()
    {
        //���콺 ���� ��ư�� ��������
        if(Input.GetMouseButtonDown(0))
        {
            //ī�޶� ��ġ���� ȭ���� ���콺 ��ġ�� �����ϴ� ���� ����
            //RAY.ORIGIN : ������ ���� ��ġ = ī�޶� ��ġ
            //RAT.DIRECTION : ������ �������
            ray = maincamera.ScreenPointToRay(Input.mousePosition);

            //2d ����͸� ���� 3d ������ ������Ʈ�� ���콺�� �����ϴ� ���]
            //������ �ε����� ������Ʈ�� �����ؼ� hit�� ����
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                //�ε��� ������Ʈ�� transform ������ �Ű������� �̺�Ʈ ȣ��
               raycastEvent.Invoke(hit.transform);
            }
        }
    }
}
