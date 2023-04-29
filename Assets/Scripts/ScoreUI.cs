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
    [SerializeField] private GameObject scoreTableObject;
    [SerializeField] private Transform scoreTableRoot;
    [SerializeField] private GameObject scoreTableRowPrefab;

    // Start is called before the first frame update
    void Start()
    {
        PlayerController.OnUpdateScore += OnUpdateScore;
        GameManager.OnGameEnd += OnGameEnd;
        GameManager.OnScoreTableUpdate += OnScoreTableUpdate;
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
        scoreTableObject.SetActive(true);
    }

    private void OnScoreTableUpdate(object sender, GameManager.OnScoreTableUpdateArgs args)
    {
        GameObject scoreTableRowObj = Instantiate(scoreTableRowPrefab, scoreTableRoot);
        scoreTableRowObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = args.pos.ToString();
        scoreTableRowObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = args.name;
        scoreTableRowObj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = args.score.ToString();
    }


}
