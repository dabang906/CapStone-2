using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    Vector3 direction;

    public float forwardSpeed;
    public float laneDistance = 4f;

    int desiredLane = 1;    //0:left 1:middle 2:right
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        direction.z = forwardSpeed;
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desiredLane++;
            if (desiredLane == 3) desiredLane = 2;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desiredLane--;
            if (desiredLane == -1) desiredLane = 0;
        }
        Vector3 targetPostion = transform.position.z * transform.forward + transform.position.y * transform.up;

        if(desiredLane == 0)
        {
            targetPostion += Vector3.left * laneDistance;
        }else if(desiredLane == 2) { targetPostion += Vector3.right * laneDistance; }

        transform.position = Vector3.Lerp(transform.position, targetPostion, 80 * Time.deltaTime);
    }
    void FixedUpdate()
    {
        controller.Move(direction * Time.deltaTime);
    }
}
