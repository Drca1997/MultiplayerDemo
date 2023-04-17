using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerHit : NetworkBehaviour, IDamageable
{
    [SerializeField] float recoveryTime;

    private float currentRecoveryTime;
    bool isStunned = false;

    public static event EventHandler OnHit;
    public static event EventHandler OnStunnedRecovery;

    private void Start()
    {
        currentRecoveryTime = recoveryTime;
    }

    private void Update()
    {
        if (!IsOwner) { return; }
        if (isStunned)
        {
            currentRecoveryTime -= Time.deltaTime;
            if (currentRecoveryTime <= 0)
            {
                isStunned = false;
                OnRecovery();
                currentRecoveryTime = recoveryTime;
            }
        }
    }

    public void ProcessHit(ulong hitPlayerID)
    {
        /*
        ClientRpcParams rpcParams = new ClientRpcParams();
        ClientRpcSendParams rpcSendParams = new ClientRpcSendParams { TargetClientIds = new List<ulong> {hitPlayerID } };
        StunPlayerClientRpc(rpcParams);
        */
        GameInput.Instance.PlayerInputActions.Disable();
        isStunned = true;
        currentRecoveryTime = recoveryTime;
        OnHit?.Invoke(this, EventArgs.Empty);
    }

    [ClientRpc]
    private void StunPlayerClientRpc(ClientRpcParams args)
    {
        GameInput.Instance.PlayerInputActions.Disable();
        isStunned = true;
        currentRecoveryTime = recoveryTime;
        OnHit?.Invoke(this, EventArgs.Empty);
    }

    private void OnRecovery()
    {
        GameInput.Instance.PlayerInputActions.Enable();
        OnStunnedRecovery?.Invoke(this, EventArgs.Empty);
    }

   
}
