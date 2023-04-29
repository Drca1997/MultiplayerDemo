using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TreasureChest : NetworkBehaviour, IInteractable
{
    private Animator animator;
    private const string OPENED = "Opened";
    private bool opened = false;
    private int treasureValue;

    public bool Opened { get => opened; }

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        animator = GetComponentInChildren<Animator>();  
        if (IsServer)
        {
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
            player.Score += treasureValue;
            OpenTreasureChestServerRpc();
        }
    }

    [ServerRpc(RequireOwnership=false)]
    private void OpenTreasureChestServerRpc()
    {
        OpenTreasureChestClientRpc();
        GameManager.Instance.CheckEndGame();    
    }

    [ClientRpc]
    private void OpenTreasureChestClientRpc()
    {
        animator.SetBool(OPENED, true);
        opened = true;
    }


    private void SetTreasureValue()
    {
        int value = Random.Range(GameDesignConstants.TREASURE_MIN_SCORE, GameDesignConstants.TREASURE_MAX_SCORE);
        SetTreasureValueClientRpc(value);
    }

    [ClientRpc]
    private void SetTreasureValueClientRpc(int value)
    {
        treasureValue = value;
    }

}
