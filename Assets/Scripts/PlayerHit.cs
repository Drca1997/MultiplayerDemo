using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerHit : NetworkBehaviour, IDamageable
{
    [SerializeField] float recoveryTime;

    private float currentRecoveryTime;
    private bool isStunned = false;

    public bool IsStunned { get => isStunned; set => isStunned = value; }

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

    public void ProcessHit()
    {
        GameInput.Instance.PlayerInputActions.Disable();
        isStunned = true;
        currentRecoveryTime = recoveryTime;
        PlayerController.LocalInstance.SetScore(-GameDesignConstants.ON_HIT_PENALTY);
        OnHit?.Invoke(this, EventArgs.Empty);
    }


    private void OnRecovery()
    {
        GameInput.Instance.PlayerInputActions.Enable();
        OnStunnedRecovery?.Invoke(this, EventArgs.Empty);
    }

   
}
