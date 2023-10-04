using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatNumberPlayerMove : MonoBehaviour
{
    public int movePower;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-1, 0, 0) * movePower * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(1, 0, 0) * movePower * Time.deltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
