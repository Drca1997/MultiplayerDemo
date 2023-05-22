using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FinalTreasure : NetworkBehaviour, IInteractable
{
    private bool isPickedUp = false;
    private PlayerController playerCarrying;

    private void Awake()
    {
        PlayerController.OnDropFinalTreasure += OnDrop;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!IsHost) { return; }
        if (playerCarrying != null)
        {
            float posX = playerCarrying.FinalTreasureHoldingPosition.position.x;
            float posY = playerCarrying.FinalTreasureHoldingPosition.position.y;
            float posZ = playerCarrying.FinalTreasureHoldingPosition.position.z;
            float rotX = playerCarrying.FinalTreasureHoldingPosition.rotation.x;
            float rotY = playerCarrying.FinalTreasureHoldingPosition.rotation.y;
            float rotZ = playerCarrying.FinalTreasureHoldingPosition.rotation.z;
            UpdateFinalTreasureLocationClientRpc(posX, posY, posZ, rotX, rotY, rotZ);
        }
    }

    public void Interact(PlayerController player)
    {
        PickUpFinalTreasureServerRpc(player.GetComponent<NetworkObject>());
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickUpFinalTreasureServerRpc(NetworkObjectReference player)
    {
        if (!isPickedUp)
        {
            isPickedUp = true;
            player.TryGet(out NetworkObject playerObj);
            playerCarrying = playerObj.GetComponent<PlayerController>();
            ClientRpcParams clientParams = new ClientRpcParams();
            ClientRpcSendParams clientSendParams = new ClientRpcSendParams { TargetClientIds = new List<ulong>() {playerObj.OwnerClientId } };
            clientParams.Send = clientSendParams;
            SetPlayerCarrierClientRpc(player, clientParams);
        }

    }

    [ClientRpc]
    private void SetPlayerCarrierClientRpc(NetworkObjectReference player, ClientRpcParams clientParams)
    {
        player.TryGet(out NetworkObject playerObj);
        playerCarrying = playerObj.GetComponent<PlayerController>();
        playerCarrying.PickUpFinalTreasure();
    }


    [ClientRpc]
    private void UpdateFinalTreasureLocationClientRpc(float posX, float posY, float posZ, float rotX, float rotY, float rotZ)
    {
        transform.position = new Vector3(posX, posY, posZ);
        transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
    }

    private void OnDrop(object sender, EventArgs args)
    {
        OnDropServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnDropServerRpc()
    {
        if (isPickedUp && playerCarrying != null)
        {
            float posX = playerCarrying.FinalTreasureDropPosition.position.x;
            float posY = playerCarrying.FinalTreasureDropPosition.position.y;
            float posZ = playerCarrying.FinalTreasureDropPosition.position.z;
            float rotX = playerCarrying.FinalTreasureDropPosition.rotation.x;
            float rotY = playerCarrying.FinalTreasureDropPosition.rotation.y;
            float rotZ = playerCarrying.FinalTreasureDropPosition.rotation.z;
            UpdateFinalTreasureLocationClientRpc(posX, posY, posZ, rotX, rotY, rotZ);
            ClientRpcParams clientParams = new ClientRpcParams();
            ClientRpcSendParams clientSendParams = new ClientRpcSendParams { TargetClientIds = new List<ulong>() { playerCarrying.OwnerClientId } };
            clientParams.Send = clientSendParams;
            isPickedUp = false;
            playerCarrying = null;
            OnDropClientRpc(clientParams);
        }
    }

    [ClientRpc]
    private void OnDropClientRpc(ClientRpcParams clientParams)
    {
        playerCarrying = null;
    }

}
