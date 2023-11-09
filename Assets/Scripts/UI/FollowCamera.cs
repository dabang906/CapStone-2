using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{

    public Transform target;
    public float dist = 8.0f;
    public float height = 2.0f;
    public float smoothRotate = 5.0f;

    void LateUpdate()
    {
        if (target == null)
        {
            return;
        }
        // 이동
        var targetPos = target.position - (Vector3.forward * dist) + (Vector3.up * height);
        if (targetPos.z >= 3)
        {
            targetPos.z = 3.0f;
        }
        else if (targetPos.z <= -25)
        {
            targetPos.z = -25.0f;
        }
        if (targetPos.x >= 5)
        {
            targetPos.x = 5.0f;
        }
        else if (targetPos.x <= -5)
        {
            targetPos.x = -5.0f;
        }
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothRotate * Time.deltaTime);

        // 회전
        //transform.LookAt(target);
        //var rot = transform.rotation.eulerAngles;
        //rot.y = 0;
        //transform.rotation = Quaternion.Euler(rot);
    }
}
