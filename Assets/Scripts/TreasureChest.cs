using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TreasureChest : NetworkBehaviour, IInteractable
{
    private Animator animator;
    private const string OPENED = "Opened";
    private bool opened = false;
    private int treasureValue = 1;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        animator = GetComponentInChildren<Animator>();    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(PlayerController player)
    {
        if (!opened)
        {
            OpenTreasureChestServerRpc();
            player.Score += treasureValue;
        }
    }

    [ServerRpc(RequireOwnership=false)]
    private void OpenTreasureChestServerRpc()
    {
        animator.SetBool(OPENED, true);
        opened = true;
    }
}
