using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public Vector2 look;
    public static GameInput Instance { get; private set; }
    public PlayerInputActions PlayerInputActions { get => playerInputActions; }

    private PlayerInputActions playerInputActions;

    public static event EventHandler OnInteractActionStarted;
    public static event EventHandler OnInteractActionPerformed;
    public static event EventHandler OnAttackActionPerformed;
    public static event EventHandler OnLookActionPerformed;

    void Awake() 
    {
        if (Instance != null)
        {
            Debug.LogError("Sth went wrong in GameInput class");
        }
        Instance = this;

        look = Vector2.zero;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.Interact.started += Interact_started;

        playerInputActions.Player.Attack.performed += Attack_performed;

      
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }

   

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractActionPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnInteractActionStarted?.Invoke(this, EventArgs.Empty);
    }

    private void Attack_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        OnAttackActionPerformed?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        
        inputVector = inputVector.normalized;

        return inputVector;
        
        /*float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        x = Mathf.Clamp(x, -1.0f, 1.0f);
        y = Mathf.Clamp(y, -1.0f, 1.0f);
        return new Vector2(x, y).normalized;
        */
    }
}
