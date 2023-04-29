using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UsernameUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField playerNameInputField;

    // Start is called before the first frame update
    void Start()
    {
        playerNameInputField.onValueChanged.AddListener((string newText) => {
            MultiplayerManager.Instance.SetPlayerName(newText);
        });
        playerNameInputField.text = MultiplayerManager.Instance.GetPlayerName();
    }

    
}
