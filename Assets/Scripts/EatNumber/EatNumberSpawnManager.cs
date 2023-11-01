using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class EatNumberSpawnManager : MonoBehaviour
{
    public GameObject Card;
    public GameObject OPCard;

    public float CardSpawnTime = 1.0f;
    public float CardDrag = 1.0f;
    public float OPCardSpawnTime = 2.0f;
    public float OPCardDrag = 1.0f;
    public bool freeze;

    private GameObject item;
    private int count = 0;
    private float time = 0;

    private void Start()
    {
        Freeze();

        Invoke("UnFreeze", 0f);
        Invoke("Freeze", 35.0f);
    }

    // Update is called once per frame
    void Update()
    {
        this.time += Time.deltaTime;
        if ((this.time >= this.CardSpawnTime) && freeze)
        {
            this.time = 0;
            if (count == 0)
            {
                item = Instantiate(Card);
                count = 1;
            }
            else if (count == 1)
            {
                item = Instantiate(OPCard);
                count = 0;
            }
            Rigidbody rig = item.AddComponent<Rigidbody>();
            rig.drag = this.CardDrag;

            item.transform.position = new Vector3(Random.Range(-3, 4), 20, 0);

        }
    }

    void Freeze()
    {
        freeze = false;
    }

    void UnFreeze()
    {
        freeze = true;
    }
}
