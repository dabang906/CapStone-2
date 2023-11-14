using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillBoard : MonoBehaviour
{
    void Update()
    {
        Vector3 target = Camera.main.transform.position;
        target.x = this.transform.position.x;
        this.transform.LookAt(target);
        float x = -this.transform.rotation.eulerAngles.x;
        this.transform.SetPositionAndRotation(this.transform.position, Quaternion.Euler(new Vector3(x, 0, 0)));
    }
}
