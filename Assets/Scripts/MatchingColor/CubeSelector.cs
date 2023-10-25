using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSelector : MonoBehaviour
{
    private ObjectDetector objectDetector;

    private void Awake()
    {
        objectDetector = GetComponent<ObjectDetector>();

        //레이어가 touchable인 오브젝트만 선택 가능하도록 ObjectDetector의 latermask 설정
        objectDetector.raycastEvent.AddListener(SelectCube);
    }

    public void SelectCube(Transform hit)
    {
        //선택된 오브젝트의 cubecontroller.changecolor를 호출해 큐브 색상 변경
        hit.GetComponent<CubeController>().ChangeColor();
    }    

}
