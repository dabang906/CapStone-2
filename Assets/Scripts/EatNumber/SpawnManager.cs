using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    public GameObject Card;

    public float spawnTime = 1.0f;
    public float drag = 1.0f;

    private float time = 0;

    // Update is called once per frame
    void Update()
    {
        this.time += Time.deltaTime;
        if (this.time >= this.spawnTime)
        {
            this.time = 0;

            GameObject item = Instantiate(Card);

            Rigidbody rig = item.AddComponent<Rigidbody>();
            rig.drag = this.drag;

            item.transform.position = new Vector3(Random.Range(-3, 4), 20, 0);
        }
    }
}
