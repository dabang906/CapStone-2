using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void OnEnable()
    {
        StartCoroutine(ShopTimer());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator ShopTimer()
    {
        for (int i= 0; i < 10; i++)
        {
            yield return new WaitForSeconds(1f);
        }
        gameObject.SetActive(false);
    }
}
