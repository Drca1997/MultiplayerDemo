using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static Hat;

public class TreasureChest : NetworkBehaviour, IInteractable
{
    private Animator animator;
    private const string OPENED = "Opened";
    private bool opened = false;
    private int treasureValue;
    HatSO hatSO;
    

    public bool Opened { get => opened; }

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        animator = GetComponentInChildren<Animator>();  
        if (IsServer)
        {
            SetHat();
            SetTreasureValue();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(PlayerController player)
    {
        if (!opened)
        {
            SetPlayerScore(player);
            OpenTreasureChestServerRpc(player.GetComponent<NetworkObject>());
        }
    }

    [ServerRpc(RequireOwnership=false)]
    private void OpenTreasureChestServerRpc(NetworkObjectReference playerRef)
    {
        if (hatSO != null)
        {
            SpawnHat(playerRef);
        }
        OpenTreasureChestClientRpc();
        //GameManager.Instance.CheckEndGame();    
    }

    [ClientRpc]
    private void OpenTreasureChestClientRpc()
    {
        animator.SetBool(OPENED, true);
        opened = true;
    }

    private void SetPlayerScore(PlayerController player)
    {
        int points;
        if (!player.HasHat())
        {
            points = treasureValue;
        }
        else
        {
            points = GameDesignConstants.TREASURE_WITH_HAT_SCORE_FOR_PLAYER_WITH_HAT; 
            if (player.CurrentHat is CrownHat)
            {
                points = Mathf.FloorToInt(points * GameDesignConstants.CROWN_HAT_SCORE_MULTIPLIER);
            }
        }
        player.SetScore(points);
    }

    private void SetTreasureValue()
    {
        int value = GameDesignConstants.TREASURE_WITH_HAT_SCORE;
        if (hatSO == null)
        {
            value = Random.Range(GameDesignConstants.TREASURE_MIN_SCORE, GameDesignConstants.TREASURE_MAX_SCORE + 1);
        }
        SetTreasureValueClientRpc(value);
    }

    [ClientRpc]
    private void SetTreasureValueClientRpc(int value)
    {
        treasureValue = value;
    }


    private void SetHat()
    {
        int type = 0;
        if (DoesSpawnHat())
        {
            type = Random.Range(1, GameDesignConstants.NUMBER_OF_HATS + 1);
        }
        else
        {
            type = 0;
        }
        SetHatClientRpc(type);
    }

    [ClientRpc]
    private void SetHatClientRpc(int value)
    {
        if (value == 0)
        {
            hatSO = null;
        }
        else
        {
            hatSO = (HatSO)Resources.Load("HatSOs\\Hat" + value.ToString());
        }
    } 

    private bool DoesSpawnHat()
    {
        float value = Random.Range(0, 0.99f);
        if (value <= GameDesignConstants.TREASURE_SPAWNS_HAT_PROBABILITY)
        {
            return true;
        }
        return false;
    }

    
    private void SpawnHat(NetworkObjectReference playerRef)
    {
        playerRef.TryGet(out NetworkObject playerObj);
        PlayerController playerController  = playerObj.GetComponent<PlayerController>();
        if (!playerController.HasHat())
        {
            //GameObject hatObj = Instantiate(hatSO.prefab, playerController.HatSpawnPosition.position, playerController.HatSpawnPosition.rotation, null);
            //hatObj.GetComponent<NetworkObject>().Spawn();
            //hatObj.GetComponent<FollowTransform>().SetTargetTransform(playerController.HatSpawnPosition);
            playerController.CurrentHat = HatSpawnerManager.Instance.HatPrefabs[hatSO.index].GetComponent<Hat>();
            SpawnHatClientRpc(playerController.OwnerClientId, hatSO.index);
            ClientRpcParams clientParams = new ClientRpcParams();
            ClientRpcSendParams clientSendParams = new ClientRpcSendParams {TargetClientIds = new List<ulong> { playerController.OwnerClientId} };
            clientParams.Send = clientSendParams;
            SetHatClientRpc(playerRef, hatSO.index, clientParams);
        }
    }

    [ClientRpc]
    private void SpawnHatClientRpc(ulong playerID, int hatPrefab)
    {
        HatSpawnerManager.Instance.SpawnHat(playerID, hatPrefab);

    }

    [ClientRpc]
    private void SetHatClientRpc(NetworkObjectReference playerRef, int hatPrefab, ClientRpcParams clientParams)
    {
        playerRef.TryGet(out NetworkObject playerObj);
        PlayerController playerController = playerObj.GetComponent<PlayerController>();
        playerController.CurrentHat = HatSpawnerManager.Instance.HatPrefabs[hatPrefab].GetComponent<Hat>();
    }

}
