using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    [SerializeField]
    private PlayerController playerController;
    private Animator animator;
    private OwnerNetworkAnimator ownerNetworkAnimator;
    private const string IS_WALKING = "IsWalking";
    private const string INTERACT = "Interact";
    private const string THROW = "Throw";
    private const string STUNNED = "IsStunned";
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        GameInput.OnInteractActionStarted += OnInteract;
        GameInput.OnAttackActionPerformed += OnThrowSnowball;
        PlayerHit.OnHit += OnStunned;
        PlayerHit.OnStunnedRecovery += OnRecovery;
        PlayerController.OnPickUpFinalTreasure += OnPickFinalTreasure;
        ownerNetworkAnimator = GetComponent<OwnerNetworkAnimator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) { return; }
        animator.SetBool(IS_WALKING, playerController.IsWalking());
        
    }

    private void OnInteract(object sender, EventArgs args)
    {   if (!IsOwner) { return; }
        ownerNetworkAnimator.SetTrigger(INTERACT);
    }

    private void OnThrowSnowball(object sender, EventArgs args)
    {
        if (!IsOwner) { return; }
        if (!playerController.HasFinalTreasure)
        {
            ownerNetworkAnimator.SetTrigger(THROW);
        }
    }

    private void OnStunned(object sender, EventArgs args)
    {
        if (!IsOwner) {return; }
        animator.SetBool(STUNNED, true);
    }

    private void OnRecovery(object sender, EventArgs args)
    {
        if (!IsOwner) { return; }
        animator.SetBool(STUNNED, false);
    }

    private void OnPickFinalTreasure(object sender, EventArgs args)
    {
        if (!IsOwner) { return; }
        //animator.SetBool(CARRYING_TREASURE, false);
    }



}
