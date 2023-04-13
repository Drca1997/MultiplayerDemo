using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI finalScoreLabel;
    [SerializeField] private TextMeshProUGUI resultLabel;

    // Start is called before the first frame update
    void Start()
    {
        PlayerController.OnUpdateScore += OnUpdateScore;
        GameManager.OnGameEnd += OnGameEnd;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnUpdateScore(object sender, PlayerController.OnUpdateScoreArgs args)
    {
        text.text = "SCORE: " + args.score.ToString(); 
    }

    

    private void OnGameEnd(object sender, GameManager.OnGameEndArgs args)
    {
        text.gameObject.SetActive(false);
        finalScoreLabel.gameObject.SetActive(true);
        finalScoreLabel.text = "FINAL SCORE: " + args.finalScore;
        resultLabel.gameObject.SetActive(true);
        if (args.isVictory)
        {
            resultLabel.text = "VICTORY"; 
        }
        else
        {
            resultLabel.text = "DEFEAT";
        }
    }


}
