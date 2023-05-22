using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController LocalInstance { get; private set; }
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private List<Vector3> possibleSpawnPositions;
    [SerializeField] private SnowballSO snowballSO;
    [SerializeField] private Transform snowballSpawnPoint;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Transform hatSpawnPosition;
    [SerializeField] private Transform finalTreasureHoldingPosition;
    [SerializeField] private Transform finalTreasureDropPosition;

    private Hat currentHat;
    

    private SnowballUI snowballUIManager;

    private int score = 0;
    private float currentCooldown = 0;
    private Vector3 lastInteractDirection;
    private Vector3 movementVector;
    private IInteractable selectedInteractable;
    private bool isWalking;
    private const float _threshold = 0.01f;
    private bool hasFinalTreasure;

    public int Score 
    { 
        get => score; 
        
    }

    public Transform HatSpawnPosition { get => hatSpawnPosition; }
    public Hat CurrentHat { get => currentHat; set => currentHat = value; }
    public Vector3 MovementVector { get => movementVector; }
    public bool HasFinalTreasure { get => hasFinalTreasure; set => hasFinalTreasure = value; }
    public Transform FinalTreasureHoldingPosition { get => finalTreasureHoldingPosition; }
    public Transform FinalTreasureDropPosition { get => finalTreasureDropPosition; set => finalTreasureDropPosition = value; }

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

    public static event EventHandler OnPickUpFinalTreasure;
    public static event EventHandler OnDropFinalTreasure;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        LocalInstance = this;
        transform.position = possibleSpawnPositions[MultiplayerManager.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];
        OnPlayerSpawn?.Invoke(this, new OnPlayerSpawnArgs { playerTransform = transform });
    }

    // Start is called before the first frame update
    void Start()
    {
        GameInput.OnInteractActionPerformed += GameInput_OnInteractAction;
        GameInput.OnAttackActionPerformed += GameInput_OnAttackAction;
        
        currentCooldown = snowballSO.cooldown;
        snowballUIManager = FindObjectOfType<SnowballUI>();

        PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(MultiplayerManager.Instance.GetPlayerColor(playerData.colorID));

        hasFinalTreasure = false;
    }

    void Update()
    {
        if (!IsOwner) { return; }

        HandleMovement();
        if (!hasFinalTreasure)
        {
            HandleInteractions();
        }

        UpdateAttackCooldown();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) { return; }
        float playerRadius = 0.6f;
        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, movementVector, out RaycastHit hit, Quaternion.identity, moveSpeed * Time.fixedDeltaTime);
        if (hit.collider != null && hit.collider.gameObject.GetComponent<EndGameTrigger>())
        {
            canMove = true;
            if (hasFinalTreasure)
            {
                Debug.Log("ENDGAME");
                score += GameDesignConstants.FINAL_TREASURE_SCORE;
                GameManager.Instance.EndGame();
                
            }
        }
        if (!canMove)
        {
            Vector3 moveXOnly = new Vector3(movementVector.x, 0, 0);
            //canMove = (movementVector.x < -0.5f || movementVector.x > 0.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveXOnly, moveSpeed * Time.fixedDeltaTime);
            canMove = (movementVector.x < -0.5f || movementVector.x > 0.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveXOnly, Quaternion.identity, moveSpeed * Time.fixedDeltaTime);
            if (canMove)
            {
                movementVector = moveXOnly;
            }
            else
            {
                Vector3 moveZOnly = new Vector3(0, 0, movementVector.z);
                //canMove = (movementVector.z < -0.5f || movementVector.z > 0.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveZOnly, moveSpeed * Time.fixedDeltaTime);
                canMove = (movementVector.z < -0.5f || movementVector.z > 0.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveZOnly, Quaternion.identity, moveSpeed * Time.fixedDeltaTime);
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
        movementVector = new Vector3(inputVector.x, 0f, inputVector.y);
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
        if (!IsOwner) { return; }
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

    private void UpdateAttackCooldown()
    {
        currentCooldown = Mathf.Clamp(currentCooldown - Time.deltaTime, 0, float.MaxValue);
        snowballUIManager.SetFillAmount(snowballSO.cooldown, currentCooldown);
    }

    private void GameInput_OnAttackAction(object sender, EventArgs e)
    {
        if (!IsOwner) { return; }
        if (currentCooldown <= 0 && !hasFinalTreasure)
        {
            InstantiateProjectilServerRpc(GetComponent<NetworkObject>(), snowballSpawnPoint.position, transform.forward);
            if (currentHat != null && currentHat is CowboyHat)
            {
                currentCooldown = snowballSO.cooldown - GameDesignConstants.COWBOY_HAT_THROW_COOLDOWN_BONUS;
            }
            else
            {
                currentCooldown = snowballSO.cooldown;
            }
            snowballUIManager.ResetCooldown();
            
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    [ServerRpc]
    private void InstantiateProjectilServerRpc(NetworkObjectReference thrower, Vector3 origin, Vector3 direction)
    {
        thrower.TryGet(out NetworkObject throwerObject);
        Transform snowball = Instantiate(snowballSO.prefab, origin, Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z), null);
        snowball.GetComponent<SnowballController>().Init(throwerObject, snowballSO.speed, direction);
        snowball.GetComponent<NetworkObject>().Spawn();
        
    }

    public bool HasHat()
    {
        if (currentHat != null)
        {
            return true;
        }
        return false;
    }

    public void SetScore(int morePoints)
    {
        score += morePoints;
        OnUpdateScore?.Invoke(this, new OnUpdateScoreArgs { score = score });
    }

    public void PickUpFinalTreasure()
    {
        hasFinalTreasure = true;
        OnPickUpFinalTreasure?.Invoke(this, EventArgs.Empty);
    }

    public void DropFinalTreasure()
    {
        hasFinalTreasure = false;
        OnDropFinalTreasure?.Invoke(this, EventArgs.Empty);
    }
}
