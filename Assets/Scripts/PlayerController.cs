using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    private Vector3 movementVector;
    private bool isWalking;

    public static event EventHandler<OnPlayerSpawnArgs> OnPlayerSpawn;
    public class OnPlayerSpawnArgs: EventArgs
    {
        public Transform playerTransform;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        OnPlayerSpawn?.Invoke(this, new OnPlayerSpawnArgs { playerTransform = transform});
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (!IsOwner) { return; }

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        x = Mathf.Clamp(x, -1.0f, 1.0f);
        y = Mathf.Clamp(y, -1.0f, 1.0f);
        movementVector = new Vector3(x, 0, y).normalized;
    }

    private void FixedUpdate()
    {
        if (movementVector.magnitude >= 0.1f) //Se houve input do jogador
        {
            bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 2f, 0.5f, movementVector, moveSpeed * Time.fixedDeltaTime);
            if (!canMove)
            {
                Vector3 moveXOnly = new Vector3(movementVector.x, 0, 0);
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 2f, 0.5f, moveXOnly, moveSpeed * Time.fixedDeltaTime);
                if(canMove)
                {
                    movementVector = moveXOnly;
                }
                else
                {
                    Vector3 moveZOnly = new Vector3(0, 0, movementVector.z);
                    canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * 2f, 0.5f, moveZOnly, moveSpeed * Time.fixedDeltaTime);
                    if (canMove)
                    {
                        movementVector = moveZOnly;
                    }
                }
            }

            if (canMove)
            {
                transform.position += moveSpeed * Time.fixedDeltaTime * movementVector;
                transform.forward = Vector3.Slerp(transform.forward, movementVector, Time.fixedDeltaTime * rotateSpeed);
                isWalking = true;
            }
            
        }
        else
        {
            isWalking = false;   
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }
}
