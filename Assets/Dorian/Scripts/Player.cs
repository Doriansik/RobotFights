using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(PlayerEnergy))]
public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float backwardSpeedMultiplier;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float rotationRightY;
    [SerializeField] private float rotationLeftY;

    [SerializeField] private float distanceToGround;
    [SerializeField] private float groundOffset;

    [SerializeField] private float crouchHeight;
    [SerializeField] private float crouchLerpSpeed;
    [SerializeField] private float colliderCenterMultiplier;

    [SerializeField] private float normalSpeedMultiplier;
    [SerializeField] private float exhaustedSpeedMultiplier;
    [SerializeField] private float zeroFloatValue;

    [SerializeField] private InputActionReference actionCrouch;
    [SerializeField] private InputActionReference actionMove;
    [SerializeField] private InputActionReference actionJump;

    [SerializeField] private Transform targetOpponent;

    private Vector2 inputDirection;
    private CapsuleCollider col;
    private Rigidbody rb;
    private Animator animator;
    private float originalHeight;
    private bool isCrouching;
    private CombatController combatController;
    private PlayerEnergy playerEnergy;

    private readonly int moveXHash = Animator.StringToHash("MoveX");
    private readonly int moveZHash = Animator.StringToHash("MoveZ");

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        animator = GetComponentInChildren<Animator>();
        combatController = GetComponent<CombatController>();
        playerEnergy = GetComponent<PlayerEnergy>();
        originalHeight = col.height;
    }

    private void Update()
    {
        if (combatController != null && combatController.CurrentState is not IdleState)
        {
            inputDirection = Vector2.zero;
            UpdateAnimator(Vector3.zero);
            return;
        }

        HandleInput();
        HandleJump();
        HandleCrouch();
        HandleRotation();
    }

    private void FixedUpdate()
    {
        if (combatController != null && combatController.CurrentState is not IdleState) return;

        HandleMovement();
    }

    private void HandleInput()
    {
        inputDirection = actionMove.action.ReadValue<Vector2>();
    }

    private void HandleRotation()
    {
        if (targetOpponent != null)
        {
            float targetY = targetOpponent.position.x > transform.position.x ? rotationRightY : rotationLeftY;
            Quaternion targetRot = Quaternion.Euler(zeroFloatValue, targetY, zeroFloatValue);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotateSpeed);
        }
    }

    private void HandleJump()
    {
        if (actionJump.action.triggered && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void HandleCrouch()
    {
        isCrouching = actionCrouch.action.IsPressed();

        float targetHeight = isCrouching ? crouchHeight : originalHeight;
        col.height = Mathf.Lerp(col.height, targetHeight, Time.deltaTime * crouchLerpSpeed);
        col.center = new Vector3(col.center.x, targetHeight / colliderCenterMultiplier, col.center.z);
    }

    private void HandleMovement()
    {
        Vector3 movementVector = new Vector3(inputDirection.x, zeroFloatValue, inputDirection.y);

        float energyMultiplier = playerEnergy.IsExhausted ? exhaustedSpeedMultiplier : normalSpeedMultiplier;

        bool isMovingBackward = Vector3.Dot(movementVector.normalized, transform.forward) < zeroFloatValue;
        float directionMultiplier = isMovingBackward ? backwardSpeedMultiplier : normalSpeedMultiplier;

        float currentSpeed = moveSpeed * energyMultiplier * directionMultiplier;

        Vector3 finalMovement = movementVector * (currentSpeed * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + finalMovement);

        UpdateAnimator(movementVector * currentSpeed);
    }

    private void UpdateAnimator(Vector3 worldVelocity)
    {
        if (moveSpeed == zeroFloatValue) return;

        Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);

        animator.SetFloat(moveXHash, localVelocity.x / moveSpeed);
        animator.SetFloat(moveZHash, localVelocity.z / moveSpeed);
    }

    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * groundOffset;
        return Physics.Raycast(origin, Vector3.down, distanceToGround + groundOffset);
    }
}