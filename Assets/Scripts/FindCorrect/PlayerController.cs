using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    Vector3 direction;
    Timer timer;
    
    public float forwardSpeed;
    public float laneDistance = 2f;
    public int correct = 0;
    public int wrong = 0;
    private Animator anim;
    int Run;
    int desiredLane = 1;    //0:left 1:middle 2:right
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        timer = FindObjectOfType<Timer>();
        controller = GetComponent<CharacterController>();
        Run = Animator.StringToHash("Run");
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.timerRunning)
        {
            anim.SetBool(Run, true);
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

            if (desiredLane == 0)
            {
                targetPostion += Vector3.left * laneDistance;
            }
            else if (desiredLane == 2) { targetPostion += Vector3.right * laneDistance; }

            if (transform.position == targetPostion) return;
            Vector3 diff = targetPostion - transform.position;
            Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;
            if (moveDir.sqrMagnitude < diff.sqrMagnitude) controller.Move(moveDir);
            else controller.Move(diff);
        }
    }
    void FixedUpdate()
    {
        controller.Move(direction * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Correct") {
            correct++;
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Wrong")
        {
            wrong++;
            Destroy(other.gameObject);
        }
    }
}
