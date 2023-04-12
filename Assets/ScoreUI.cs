using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI finalScoreLabel;

    // Start is called before the first frame update
    void Start()
    {
        PlayerController.OnUpdateScore += OnUpdateScore;
        //TreasureChestSpawner.OnGameEnd += OnGameEnd;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnUpdateScore(object sender, PlayerController.OnUpdateScoreArgs args)
    {
        text.text = "SCORE: " + args.score.ToString(); 
    }

    /*

    private void OnGameEnd(object sender, TreasureChestSpawner.OnGameEndArgs args)
    {
        finalScoreLabel.gameObject.SetActive(true);
        finalScoreLabel.text = "FINAL SCORE: " + args.finalScore;
    }*/


}
