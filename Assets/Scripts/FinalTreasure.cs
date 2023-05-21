using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FinalTreasure : NetworkBehaviour, IInteractable
{
    private bool isPickedUp = false;
    private PlayerController playerCarrying;

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
            UpdateFinalTreasureLocationClientRpc();
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
            playerObj.GetComponent<PlayerController>().PickUpFinalTreasure();
            SetPlayerCarrierClientRpc(player);
            //GetComponent<NetworkObject>().DontDestroyWithOwner = true;
            //GetComponent<NetworkObject>().Despawn();
        }

    }

    [ClientRpc]
    private void SetPlayerCarrierClientRpc(NetworkObjectReference player)
    {
        player.TryGet(out NetworkObject playerObj);
        playerCarrying = playerObj.GetComponent<PlayerController>();
    }

    [ClientRpc]
    private void UpdateFinalTreasureLocationClientRpc()
    {
        transform.position = playerCarrying.FinalTreasureHoldingPosition.transform.position;
        transform.rotation = playerCarrying.FinalTreasureHoldingPosition.transform.rotation;
    }

}
