using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestingCharacterSelectionUI : MonoBehaviour
{
    [SerializeField] Button readyButton;

    private void Awake()
    {
        readyButton.onClick.AddListener(()=>
        {
            CharacterSelection.Instance.SetPlayerReady();
            readyButton.GetComponentInChildren<TextMeshProUGUI>().text = "READY";
        });
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
