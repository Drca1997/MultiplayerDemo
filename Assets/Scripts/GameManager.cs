using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    List<int> players;
    Dictionary<int, int> finalScores;

    public static event EventHandler<OnGameEndArgs> OnGameEnd;
    public class OnGameEndArgs: EventArgs
    {
        public bool isVictory;
        public int finalScore;
    }

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        players = new List<int>();
        finalScores= new Dictionary<int, int>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ClientRpc]
    private void GetPlayerFinalScoreClientRpc()
    {
        GetPlayerFinalScoreServerRpc((int)PlayerController.LocalInstance.OwnerClientId, PlayerController.LocalInstance.Score);
    }

    [ServerRpc(RequireOwnership = false)]
    private void GetPlayerFinalScoreServerRpc(int playerID, int finalScore)
    {
        finalScores[playerID] = finalScore;
        if (finalScores.Count == NetworkManager.Singleton.ConnectedClientsIds.Count)
        {
            List<ulong> winnersID = GetWinner();
            ClientRpcParams winnerParams = new ClientRpcParams();
            ClientRpcSendParams winnerSendParams = new ClientRpcSendParams { TargetClientIds = winnersID };
            winnerParams.Send = winnerSendParams;
            GameVictoryClientRpc(winnerParams);
            
            List<ulong> losers = GetLosers(winnersID);
            ClientRpcParams losersParams = new ClientRpcParams();
            ClientRpcSendParams losersSendParams = new ClientRpcSendParams { TargetClientIds = losers };
            losersParams.Send = losersSendParams;
            GameOverClientRpc(losersParams);
        }
    }

    private List<ulong> GetWinner()
    {
        List<int> winnerID = new List<int> { -1};
        int max = 0;
        foreach(KeyValuePair<int, int> pair in finalScores)
        {
            if (pair.Value > max)
            {
                max = pair.Value;
                winnerID.Clear();
                winnerID.Add(pair.Key);
            }
            else if(pair.Value == max)
            {
                winnerID.Add(pair.Key);
            }
        }
        return winnerID.ConvertAll(i => (ulong)i);
    }

    private List<ulong> GetLosers(List<ulong> winnersID)
    {
        List<ulong> losers = new List<ulong>();
        foreach(ulong playerID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!winnersID.Contains(playerID))
            {
                losers.Add(playerID);
            }
        }
        return losers;
    }
    [ServerRpc(RequireOwnership=false)]
    public void CheckEndGameServerRpc()
    {
        if (!AreThereChestsToOpen())
        {
            GetPlayerFinalScoreClientRpc();
        }
    }
    private bool AreThereChestsToOpen()
    {
        foreach (TreasureChest chest in TreasureChestSpawner.Instance.SpawnedTreasureChests)
        {
            if (!chest.Opened)
            {
                return true;
            }
        }
        return false;
    }
    [ClientRpc]
    private void GameOverClientRpc(ClientRpcParams rpcParams)
    {
        OnGameEnd?.Invoke(this, new OnGameEndArgs { isVictory = false, finalScore = PlayerController.LocalInstance.Score });
    }

    [ClientRpc]
    private void GameVictoryClientRpc(ClientRpcParams rpcParams)
    {
        OnGameEnd?.Invoke(this, new OnGameEndArgs { isVictory = true, finalScore = PlayerController.LocalInstance.Score });
    }

}
