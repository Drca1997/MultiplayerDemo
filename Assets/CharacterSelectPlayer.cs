using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private TextMeshPro username;
    

    private void Start()
    {
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged += OnPlayerDataChanged;
        CharacterSelection.Instance.OnReadyChanged += OnPlayerReady;
        UpdatePlayer();
    }

    private void OnPlayerReady(object sender, EventArgs args)
    {
        UpdatePlayer();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void UpdatePlayer()
    {
        if (MultiplayerManager.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyGameObject.SetActive(CharacterSelection.Instance.IsPlayerReady(playerData.clientID));
            username.text = playerData.playerName.ToString();
            playerVisual.SetPlayerColor(MultiplayerManager.Instance.GetPlayerColor(playerData.colorID));
        }
        else
        {
            Hide();
        }
    }
    private void OnPlayerDataChanged(object sender, System.EventArgs args)
    {
        UpdatePlayer();
    }

    private void OnDestroy()
    {
        
        MultiplayerManager.Instance.OnPlayerDataNetworkListChanged -= OnPlayerDataChanged;
    }
}
