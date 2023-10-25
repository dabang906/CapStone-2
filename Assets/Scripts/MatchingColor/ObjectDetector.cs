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
        //"maincamera" 태그를 가지고 있는 오브젝트 탐색 후 camera 컴포넌트 정보 전달
        maincamera = Camera.main;
    }

    private void Update()
    {
        //마우스 왼쪽 버튼을 눌렀을때
        if(Input.GetMouseButtonDown(0))
        {
            //카메라 위치에서 화면의 마우스 위치를 관통하는 광선 생성
            //RAY.ORIGIN : 광선의 시작 위치 = 카메라 위치
            //RAT.DIRECTION : 광선의 진행방향
            ray = maincamera.ScreenPointToRay(Input.mousePosition);

            //2d 모니터를 통해 3d 월드의 오브젝트를 마우스로 선택하는 방법]
            //관성에 부딪히는 오브젝트를 검출해서 hit에 저장
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                //부딪힌 오브젝트의 transform 정보를 매개변수로 이벤트 호출
               raycastEvent.Invoke(hit.transform);
            }
        }
    }
}
