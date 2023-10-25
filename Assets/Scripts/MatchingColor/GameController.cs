using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI textScore;
    private int score = 0;

    public void IncreaseScore()
    {
        score++;
        textScore.text = $"Score {score}";
    }
}
