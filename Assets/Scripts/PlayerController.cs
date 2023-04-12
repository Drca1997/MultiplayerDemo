using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private List<Vector3> possibleSpawnPositions;
    private int score = 0;

    private Vector3 lastInteractDirection;
    private IInteractable selectedInteractable;
    private Vector3 movementVector;
    private bool isWalking;

    public int Score 
    { 
        get => score; 
        set 
        { 
            score = value; 
            OnUpdateScore?.Invoke(this, new OnUpdateScoreArgs { score = value }); 
        } 
    }

    public static event EventHandler<OnSelectedInteractableChangedEventArgs> OnSelectedInteractableChanged;
    public class OnSelectedInteractableChangedEventArgs : EventArgs
    {
        public IInteractable selectedInteractable;
    }

    public static event EventHandler<OnUpdateScoreArgs> OnUpdateScore;
    public class OnUpdateScoreArgs: EventArgs
    {
        public int score;
    }

    public static event EventHandler<OnPlayerSpawnArgs> OnPlayerSpawn;
    public class OnPlayerSpawnArgs: EventArgs
    {
        public Transform playerTransform;
    }

  


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        transform.position = possibleSpawnPositions[(int)OwnerClientId];
        OnPlayerSpawn?.Invoke(this, new OnPlayerSpawnArgs { playerTransform = transform});
    }

    // Start is called before the first frame update
    void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    void Update()
    {
        if (!IsOwner) { return; }

        HandleMovement();
        HandleInteractions();
        
    }

    private void FixedUpdate()
    {
        if (!IsOwner) { return; }
        float playerHeight = 2f;
        float playerRadius = 0.5f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, movementVector, moveSpeed * Time.fixedDeltaTime);
        if (!canMove)
        {
            Vector3 moveXOnly = new Vector3(movementVector.x, 0, 0);
            canMove = (movementVector.x < -0.5f || movementVector.x > 0.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveXOnly, moveSpeed * Time.fixedDeltaTime);
            if(canMove)
            {
                movementVector = moveXOnly;
            }
            else
            {
                Vector3 moveZOnly = new Vector3(0, 0, movementVector.z);
                canMove = (movementVector.z < -0.5f || movementVector.z > 0.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveZOnly, moveSpeed * Time.fixedDeltaTime);
                if (canMove)
                {
                    movementVector = moveZOnly;
                }
            }
        }
        if (canMove)
        {
            transform.position += moveSpeed * Time.fixedDeltaTime * movementVector;
        }
        transform.forward = Vector3.Slerp(transform.forward, movementVector, Time.fixedDeltaTime * rotateSpeed);
        isWalking = movementVector.magnitude >= 0.1f;
    }

    private void HandleMovement()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();
        movementVector = new Vector3(inputVector.x, 0, inputVector.y);
    }

    private void HandleInteractions()
    {
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero)
        {
            lastInteractDirection = moveDir;
        }

        float interactDistance = 1.2f;
        Debug.DrawLine(transform.position, transform.position + lastInteractDirection * interactDistance);
        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance))
        {
            if (raycastHit.transform.TryGetComponent(out IInteractable interactable))
            {
                if (interactable != selectedInteractable)
                {
                    SetSelectedInteractable(interactable);
                }
            }
            else
            {
                SetSelectedInteractable(null);
            }
        }
        else
        {
            SetSelectedInteractable(null);
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (selectedInteractable != null)
        {
            selectedInteractable.Interact(this);
        }
    }

    private void SetSelectedInteractable(IInteractable selectedInteractable)
    {
        this.selectedInteractable = selectedInteractable;

        OnSelectedInteractableChanged?.Invoke(this, new OnSelectedInteractableChangedEventArgs
        {
            selectedInteractable = selectedInteractable
        });
    }
    public bool IsWalking()
    {
        return isWalking;
    }
}
