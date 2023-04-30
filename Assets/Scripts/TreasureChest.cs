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
            OpenTreasureChestServerRpc();
        }
    }

    [ServerRpc(RequireOwnership=false)]
    private void OpenTreasureChestServerRpc()
    {
        if (hatSO != null)
        {
            //SpawnHat();
        }
        OpenTreasureChestClientRpc();
        GameManager.Instance.CheckEndGame();    
    }

    [ClientRpc]
    private void OpenTreasureChestClientRpc()
    {
        animator.SetBool(OPENED, true);
        opened = true;
    }

    private void SetPlayerScore(PlayerController player)
    {
        if (!player.HasHat())
        {
            player.Score += treasureValue;
        }
        else
        {
            player.Score += GameDesignConstants.TREASURE_WITH_HAT_SCORE_FOR_PLAYER_WITH_HAT;
        }
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
        float value = Random.Range(0, 1);
        if (value <= GameDesignConstants.TREASURE_SPAWNS_HAT_PROBABILITY)
        {
            return true;
        }
        return false;
    }

    /*
    private GameObject SpawnHat()
    {
        switch (hatType)
        {
            case HatEnum.COWBOY:
                GameObject cowboyHatObj = Instantiate();
                return cowboyHatObj;
            case HatEnum.CROWN:
                GameObject crownObj = Instantiate();
                return crownObj;
            default:
                return null;
        }
    }*/

}
