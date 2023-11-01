using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerText : MonoBehaviour
{
    public Text correctText;
    public Text wrongText;

    PlayerController playerController;
    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        correctText.text = "Correct : " + playerController.correct.ToString();
        wrongText.text = "Wrong : " + playerController.wrong.ToString();
    }
}
