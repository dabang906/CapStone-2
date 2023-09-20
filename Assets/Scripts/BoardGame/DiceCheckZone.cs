using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCheckZone : MonoBehaviour
{
    Vector3 diceVelocity;
    public static bool isTrigger = true;
    private void FixedUpdate() {
        diceVelocity = DiceScript.diceVelocity;
    }
    void OnTriggerStay(Collider col) {
        if(diceVelocity.x == 0f && diceVelocity.y == 0f && diceVelocity.z == 0f &&isTrigger){
            switch(col.gameObject.name){
                case "Side1":
                    DiceNumberTextScript.diceNumber = 2;
                    Stone.diceNum = 2;
                    break;
                case "Side2":
                    DiceNumberTextScript.diceNumber = 3;
                    Stone.diceNum = 3;
                    break;
                case "Side3":
                    DiceNumberTextScript.diceNumber = 1;
                    Stone.diceNum = 1;
                    break;
                case "Side4":
                    DiceNumberTextScript.diceNumber = 6;
                    Stone.diceNum = 6;
                    break;
                case "Side5":
                    DiceNumberTextScript.diceNumber = 5;
                    Stone.diceNum = 5;
                    break;
                case "Side6":
                    DiceNumberTextScript.diceNumber = 4;
                    Stone.diceNum = 4;
                    break;
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
