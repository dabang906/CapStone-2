using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Text textScore;
    [HideInInspector]
    public int score = 0;

    public void IncreaseScore()
    {
        score++;
        textScore.text = $"Score {score}";
    }
}
