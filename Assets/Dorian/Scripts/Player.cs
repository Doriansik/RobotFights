using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float rotationRightY = 90f;
    [SerializeField] private float rotationLeftY = 270f;

    [SerializeField] private float distanceToGround;
    [SerializeField] private float groundOffset;

    [SerializeField] private float crouchHeight;
    [SerializeField] private float crouchLerpSpeed;
    [SerializeField] private float colliderCenterMultiplier;

    [SerializeField] private InputActionReference actionCrouch;
    [SerializeField] private InputActionReference actionMove;
    [SerializeField] private InputActionReference actionJump;

    private Vector2 inputDirection;
    private CapsuleCollider col;
    private Rigidbody rb;
    private Animator animator;
    private float originalHeight;
    private bool isCrouching;
    private CombatController combatController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        animator = GetComponentInChildren<Animator>();
        combatController = GetComponent<CombatController>();
        originalHeight = col.height;
    }

    private void Update()
    {
        if (combatController != null && combatController.CurrentState is not IdleState)
        {
            inputDirection = Vector2.zero;
            animator.SetBool("Movement", false);
            return;
        }

        HandleMovementInput();
        HandleJump();
        HandleCrouch();
        HandleRotation();
    }

    private void FixedUpdate()
    {
        if (combatController != null && combatController.CurrentState is not IdleState) return;

        HandleMovement();
    }

    private void HandleMovementInput()
    {
        inputDirection = actionMove.action.ReadValue<Vector2>();
        animator.SetBool("Movement", inputDirection != Vector2.zero);
    }

    private void HandleRotation()
    {
        if (inputDirection.x != 0)
        {
            float targetY = inputDirection.x > 0 ? rotationRightY : rotationLeftY;
            Quaternion targetRot = Quaternion.Euler(0, targetY, 0);
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

        if (isCrouching)
        {
            col.height = Mathf.Lerp(col.height, crouchHeight, Time.deltaTime * crouchLerpSpeed);
            col.center = new Vector3(col.center.x, crouchHeight / colliderCenterMultiplier, col.center.z);
        }
        else
        {
            col.height = Mathf.Lerp(col.height, originalHeight, Time.deltaTime * crouchLerpSpeed);
            col.center = new Vector3(col.center.x, originalHeight / colliderCenterMultiplier, col.center.z);
        }
    }

    private void HandleMovement()
    {
        Vector3 movement = new Vector3(inputDirection.x, 0, 0) * (moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + movement);
    }

    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * groundOffset;
        return Physics.Raycast(origin, Vector3.down, distanceToGround + groundOffset);
    }
}