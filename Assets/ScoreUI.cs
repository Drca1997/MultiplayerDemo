using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        PlayerController.OnUpdateScore += OnUpdateScore;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnUpdateScore(object sender, PlayerController.OnUpdateScoreArgs args)
    {
        text.text = "SCORE: " + args.score.ToString(); 
    }


}
