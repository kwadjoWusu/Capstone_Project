using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerCon;
    PlayerLocomotion playerLocomotion;

    AnimatorManager animatorManager;

    public Vector2 moveInput;
    public Vector2 cameraInput;

    public float cameraInputX;
    public float cameraInputY;

    public float moveAmount;
    public float vertInput;
    public float horiInput;

    public bool sprint_input;

    public bool jump_input;
    public bool dodge_input;

    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void OnEnable()
    {
        if ( playerCon == null)
        {
            playerCon = new PlayerControls();

            playerCon.PlayerMovement.Movement.performed += i => moveInput = i.ReadValue<Vector2>(); //this basically records the action taken by a player(WASD or gamepad) and stores it in a Vector2 variable
            playerCon.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
            playerCon.PlayerActions.Sprint.performed += i => sprint_input = true;
            playerCon.PlayerActions.Sprint.canceled += i => sprint_input = false;
            playerCon.PlayerActions.Sprint.performed += i => sprint_input = true;
            playerCon.PlayerActions.Sprint.canceled += i => sprint_input = false;
            playerCon.PlayerActions.Jump.performed += i => jump_input = true;
            playerCon.PlayerActions.Dodge.performed += i => dodge_input = true;


        }
        playerCon.Enable();
    }

    private void OnDisable()
    {
        playerCon.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpInput();
        HandleDodgeInput();
        //HandleActionInput();

    }

    private void HandleMovementInput()
    {
        vertInput = moveInput.y;
        horiInput = moveInput.x;

        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horiInput) + Mathf.Abs(vertInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount,playerLocomotion.isSprinting);
    }

    private void HandleSprintingInput()
    {
        if (sprint_input && moveAmount>0.5f)
        {
            playerLocomotion.isSprinting = true;
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
    }
    public void HandleJumpInput()
    {
        if (jump_input)
        {
            jump_input = false;
            playerLocomotion.HandleJumping();
        }
    }

    public void HandleDodgeInput()
    {
        if (dodge_input)
        {
            dodge_input = false;
            playerLocomotion.HandleDodge();
        }
    }
}
