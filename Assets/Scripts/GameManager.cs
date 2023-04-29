using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private Transform playerPrefab;
    [SerializeField] private UsernamesManager usernamesManager;
    public static GameManager Instance { get; private set; }
    Dictionary<int, int> finalScores;
    private Dictionary<ulong, NetworkObject> playerNetworkObjects;

    public static event EventHandler<OnGameEndArgs> OnGameEnd;
    public class OnGameEndArgs : EventArgs
    {
        public bool isVictory;
        public int finalScore;
    }
    public static event EventHandler<OnScoreTableUpdateArgs> OnScoreTableUpdate;
    public class OnScoreTableUpdateArgs: EventArgs
    {
        public int pos;
        public string name;
        public int score;
    }

    private void Awake()
    {
        Instance = this;
        playerNetworkObjects = new Dictionary<ulong, NetworkObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        finalScores= new Dictionary<int, int>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(object sender, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
            playerNetworkObjects[clientId] = playerTransform.GetComponent<NetworkObject>();
        }
        foreach(KeyValuePair<ulong, NetworkObject> pair in playerNetworkObjects)
        {
            FixedString64Bytes name = MultiplayerManager.Instance.GetPlayerDataFromClientId(pair.Key).playerName;
            GetUsernameClientRpc(pair.Value, name);
        }
    }

    [ClientRpc]
    private void GetUsernameClientRpc(NetworkObjectReference playerReference, FixedString64Bytes playerName)
    {
        playerReference.TryGet(out NetworkObject playerObj);
        usernamesManager.GetPlayerRef(playerObj.transform, playerName.ToString());   
    }

    [ClientRpc]
    private void GetPlayerFinalScoreClientRpc()
    {
        GetPlayerFinalScoreServerRpc((int)PlayerController.LocalInstance.OwnerClientId, PlayerController.LocalInstance.Score);
    }

    [ServerRpc(RequireOwnership = false)]
    private void GetPlayerFinalScoreServerRpc(int playerID, int finalScore)
    {
        //to fix: isto esta a correr 2 vezes por cada jogador. pq??
        finalScores[playerID] = finalScore;
        if (finalScores.Count == NetworkManager.Singleton.ConnectedClientsIds.Count) 
        {
            List<ulong> winnersID = SendWinnerData();
            SendLosersData(winnersID);

            BuildScoreTable();
        }
    }

    private List<ulong> SendWinnerData()
    {
        List<ulong> winnersID = GetWinner();
        ClientRpcParams winnerParams = new ClientRpcParams();
        ClientRpcSendParams winnerSendParams = new ClientRpcSendParams { TargetClientIds = winnersID };
        winnerParams.Send = winnerSendParams;
        GameVictoryClientRpc(winnerParams);
        return winnersID;
    }

    private void SendLosersData(List<ulong> winnersID)
    {
        List<ulong> losers = GetLosers(winnersID);
        ClientRpcParams losersParams = new ClientRpcParams();
        ClientRpcSendParams losersSendParams = new ClientRpcSendParams { TargetClientIds = losers };
        losersParams.Send = losersSendParams;
        GameOverClientRpc(losersParams);
    }

    private void BuildScoreTable()
    {
        List<ScoreEntry> scoreTable = GetScoreTable();

        for (int i = 0; i < scoreTable.Count; i++)
        {
            FixedString64Bytes name = MultiplayerManager.Instance.GetPlayerDataFromClientId(scoreTable[i].playerID).playerName;
            ScoreTableClientRpc(i + 1, name, scoreTable[i].score);
        }
    }

    [ClientRpc]
    private void ScoreTableClientRpc(int pos, FixedString64Bytes playerName, int score)
    {
        OnScoreTableUpdate?.Invoke(this, new OnScoreTableUpdateArgs { pos = pos, name = playerName.ToString(), score = score});
    }

    private List<ScoreEntry> GetScoreTable()
    {
        List<ScoreEntry> scoreTable = new List<ScoreEntry>();
        foreach(KeyValuePair<int, int> score in finalScores)
        {
            scoreTable.Add(new ScoreEntry((ulong)score.Key, score.Value));
        }

        scoreTable.Sort((a, b) => 
        { 
            if (a.score > b.score)
            {
                return -1;
            }
            else if(a.score < b.score)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        });
        return scoreTable;
    }

    private List<ulong> GetWinner(/*out List<int> scoreTable*/)
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

    //[ServerRpc(RequireOwnership=false)]
    public void CheckEndGame()
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

public struct ScoreEntry
{
    public ulong playerID;
    public int score;
    public ScoreEntry(ulong playerID, int score)
    {
        this.playerID = playerID;
        this.score = score;
    }
}
