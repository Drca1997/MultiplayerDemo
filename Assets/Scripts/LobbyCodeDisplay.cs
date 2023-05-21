using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LobbyCodeDisplay : MonoBehaviour
{
    TMPro.TextMeshProUGUI label;

  
    private void Awake()
    {
        label = GetComponent<TMPro.TextMeshProUGUI>();
        MultiplayerManager.Instance.OnLobbyCode += OnLobbyCode;
    }

    private void Start()
    {
        MultiplayerManager.Instance.GetLobbyCodeServerRpc(MultiplayerManager.Instance.OwnerClientId);
    }

    private void OnLobbyCode(object sender, MultiplayerManager.OnLobbyCodeArgs args)
    {
        label.text = "Lobby Code: " + args.lobbyCode;
    }

}
