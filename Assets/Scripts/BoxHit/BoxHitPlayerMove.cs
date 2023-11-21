using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxHitPlayerMove : MonoBehaviour
{
    public int jumpPower;

    Rigidbody rig;
    bool isJump;
    BoxHitCollisionEvent boxHitCollisionEvent;
    // Start is called before the first frame update
    void Start()
    {
        isJump = false;
        rig = GetComponent<Rigidbody>();
        boxHitCollisionEvent = FindObjectOfType<BoxHitCollisionEvent>();
        /*
        Freeze();
        Invoke("UnFreeze", 13.0f);
        Invoke("Freeze", 13.0f);*/
    }

    void FixedUpdate()
    {
        Jump(isJump);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Plane")
        {
            isJump = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Plane")
        {
            isJump = false;
        }
    }

    void Jump(bool isJump)
    {
        if (Input.GetKey(KeyCode.Space) && isJump && boxHitCollisionEvent.timerRunning)
        {
            rig.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }
}
