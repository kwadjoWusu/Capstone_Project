using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    Vector3 moveDirection;
    Transform cameraObject;
    public Rigidbody playerRigidbody;
    InputManager inputManager;
    PlayerManager playerManager;
    AnimatorManager animatorManager;

    [Header("Falling")]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffSet = 0.7f;
    public LayerMask groundLayer;
  

    [Header("Movement Flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;

    [Header("Movement Speeds")]
    public float walkingSpeed = 1.5f;
    public float sprintingSpeed = 7;
    public float runSpeed = 5;
    public float rotateSpeed = 15;

    [Header("Jump Speeds")]
    public float jumpHeight = 3;
    public float gravityIntensity = -15;

    [Header("Slope Settings")]
    public float maxSlopeAngle = 45f; // Maximum angle (in degrees) the player can walk on.
    public float slideSpeed = 5f;     // Speed at which the player slides down steep slopes.




    private void Awake()
    {
        isGrounded = true;
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();

    }

    //public void HandleAllMovement()

    //{
    //    HandleFallingAndLanding();
    //    if (playerManager.isInteracting)
    //        return;
    //    HandleMovement();
    //    HandleRotation();
    //}

    //Test code 
    public void HandleAllMovement()
    {
        HandleFallingAndLanding();
        // Only block movement if on the ground and interacting.
        if (playerManager.isInteracting && isGrounded)
            return;
        HandleMovement();
        HandleRotation();
    }


    //private void HandleMovement()
    //{

    //    if (isJumping)
    //        return;
    //    moveDirection = cameraObject.forward * inputManager.vertInput;
    //    moveDirection = moveDirection + cameraObject.right * inputManager.horiInput;
    //    moveDirection.Normalize();
    //    moveDirection.y = 0;

    //    // if we sprinting, select the sprinting speed
    //    if (isSprinting)
    //    {
    //        moveDirection = moveDirection * sprintingSpeed;
    //    }
    //    else
    //    {
    //        // if we running, select the running speed
    //        // if we walking, select the walking speed
    //        if (inputManager.moveAmount >= 0.5f)
    //        {
    //            moveDirection = moveDirection * runSpeed;
    //        }
    //        else
    //        {
    //            moveDirection = moveDirection * walkingSpeed;
    //        }

    //    }
    //    if (isGrounded && !isJumping)
    //    {
    //        Vector3 movementVelocity = moveDirection;
    //        playerRigidbody.velocity = movementVelocity;
    //    }
    //}

    //Test code
    //private void HandleMovement()
    //{
    //    // Calculate horizontal input direction.
    //    moveDirection = cameraObject.forward * inputManager.vertInput + cameraObject.right * inputManager.horiInput;
    //    moveDirection.Normalize();
    //    moveDirection.y = 0;

    //    // Choose speed based on movement flags.
    //    if (isSprinting)
    //    {
    //        moveDirection *= sprintingSpeed;
    //    }
    //    else if (inputManager.moveAmount >= 0.5f)
    //    {
    //        moveDirection *= runSpeed;
    //    }
    //    else
    //    {
    //        moveDirection *= walkingSpeed;
    //    }

    //    // Preserve the current vertical velocity while updating horizontal velocity.
    //    Vector3 currentVelocity = playerRigidbody.velocity;
    //    Vector3 newVelocity = new Vector3(moveDirection.x, currentVelocity.y, moveDirection.z);
    //    playerRigidbody.velocity = newVelocity;
    //}

    private void HandleMovement()
    {
        // Calculate horizontal input direction.
        moveDirection = cameraObject.forward * inputManager.vertInput + cameraObject.right * inputManager.horiInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        // Choose movement speed.
        if (isSprinting)
        {
            moveDirection *= sprintingSpeed;
        }
        else if (inputManager.moveAmount >= 0.5f)
        {
            moveDirection *= runSpeed;
        }
        else
        {
            moveDirection *= walkingSpeed;
        }

        // Slope handling: if grounded, adjust moveDirection based on the slope.
        if (isGrounded)
        {
            // Set the ray origin a bit above the player's position (similar to your falling detection).
            Vector3 rayCastOrigin = transform.position;
            rayCastOrigin.y += rayCastHeightOffSet;

            RaycastHit slopeHit;
            // Cast a ray downward to detect the ground's normal.
            if (Physics.Raycast(rayCastOrigin, Vector3.down, out slopeHit, 1f, groundLayer))
            {
                float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);

                if (slopeAngle <= maxSlopeAngle)
                {
                    // Project the desired moveDirection onto the slope plane.
                    moveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
                }
                else
                {
                    // Optional: If the slope is too steep, force sliding down.
                    Vector3 slideDirection = new Vector3(slopeHit.normal.x, -slopeHit.normal.y, slopeHit.normal.z);
                    moveDirection = slideDirection * slideSpeed;
                }
            }
        }

        // Preserve the current vertical velocity while updating horizontal velocity.
        Vector3 currentVelocity = playerRigidbody.velocity;
        Vector3 newVelocity = new Vector3(moveDirection.x, currentVelocity.y, moveDirection.z);
        playerRigidbody.velocity = newVelocity;
    }


    //private void HandleRotation()
    //{

    //    if (isJumping)
    //        return;
    //    Vector3 targetDirection = cameraObject.forward * inputManager.vertInput;
    //    targetDirection = targetDirection + cameraObject.right * inputManager.horiInput;
    //    targetDirection.Normalize();
    //    targetDirection.y = 0;

    //    if (targetDirection == Vector3.zero)
    //    {
    //        targetDirection = transform.forward;
    //    }

    //    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
    //    Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed*Time.deltaTime);
    //    if (isGrounded && !isJumping)
    //    {
    //        transform.rotation = playerRotation;
    //    }

    //}

    //Test code
    private void HandleRotation()
    {
        // Calculate input-based target direction.
        Vector3 targetDirection = cameraObject.forward * inputManager.vertInput + cameraObject.right * inputManager.horiInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        // If there is no input, maintain the current forward direction.
        if (targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        // Optionally adjust rotation speed if airborne.
        float currentRotateSpeed = isGrounded ? rotateSpeed : rotateSpeed * 0.5f;

        // Smoothly rotate towards the target direction.
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, currentRotateSpeed * Time.deltaTime);
    }


    //public void HandleFallingAndLanding()
    //{
    //    RaycastHit hit;
    //    Vector3 rayCastOrigin = transform.position;
    //    rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffSet;

    //    if (!isGrounded && !isJumping)
    //    {
    //        if (!playerManager.isInteracting)
    //        {
    //            animatorManager.PlayTargetAnimation("Falling", true);
    //        }

    //        inAirTimer = inAirTimer + Time.deltaTime;
    //        playerRigidbody.AddForce(transform.forward * leapingVelocity);
    //        playerRigidbody.AddForce(Vector3.down * fallingVelocity * inAirTimer);
    //    }

    //    if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, 0.5f,groundLayer))
    //    {
    //        if ( !isGrounded && playerManager.isInteracting)
    //        {
    //            animatorManager.PlayTargetAnimation("Landing" , true);
    //        }
    //        inAirTimer = 0;
    //        isGrounded = true;
    //        playerManager.isInteracting = false;
    //    }
    //    else
    //    {
    //        isGrounded = false; 
    //    }



    //}

    //Test code
    public void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += rayCastHeightOffSet;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Falling", true);
            }
            animatorManager.animator.SetBool("isUsingRootMotion", false);
            inAirTimer += Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(Vector3.down * fallingVelocity * inAirTimer);
        }

        if (Physics.SphereCast(rayCastOrigin, 0.2f, Vector3.down, out hit, 0.5f, groundLayer))
        {
            // Only trigger landing animation if we just became grounded.
            if (!isGrounded)
            {
                // Start the landing animation and disable movement.
                animatorManager.PlayTargetAnimation("Landing", true);
                StartCoroutine(WaitForLandingAnimation());
            }
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private IEnumerator WaitForLandingAnimation()
    {
        // Set the flag to block movement.
        playerManager.isInteracting = true;

        // Option 1: If you know the exact duration of the landing animation:
        // float landingDuration = 0.8f; // Replace with your animation's duration.
        // yield return new WaitForSeconds(landingDuration);

        // Option 2: Wait until the landing animation is finished by polling the animator state.
        // This assumes your landing animation state is named "Landing" in your Animator.
        while (animatorManager.animator.GetCurrentAnimatorStateInfo(0).IsName("Landing"))
        {
            yield return null;
        }

        // Re-enable movement after the landing animation has finished.
        playerManager.isInteracting = false;
    }




    public void HandleJumping()
    {
        if (isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRigidbody.velocity = playerVelocity;
        }
    }

    public void HandleDodge()
    {
        if (playerManager.isInteracting)
            return;
        animatorManager.PlayTargetAnimation("Dodge", true,true);
        //Toggle invulnerable bool for no hp damage durning animation
    }




}
