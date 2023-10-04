using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxHitPlayerMove : MonoBehaviour
{
    Rigidbody rig;

    public int jumpPower;

    bool isJump;

    // Start is called before the first frame update
    void Start()
    {
        isJump = false;
        rig = GetComponent<Rigidbody>();

        Freeze();

        Invoke("UnFreeze", 5.0f);
        Invoke("Freeze", 15.0f);
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

    void Freeze()
    {
        rig.constraints = RigidbodyConstraints.FreezeAll;
    }

    void UnFreeze()
    {
        rig.constraints = RigidbodyConstraints.FreezeAll;
        rig.constraints &= ~RigidbodyConstraints.FreezePositionY;
    }

    void Jump(bool isJump)
    {
        if (Input.GetKey(KeyCode.Space) && isJump)
        {
            rig.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }
}
