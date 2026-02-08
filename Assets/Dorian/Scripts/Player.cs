using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rotateSpeed;

    [Header("GroundCheck")]
    [SerializeField] private float distanceToGround;
    [SerializeField] private float groundOffset;

    [Header("Crouch Settings")]
    [SerializeField] private float crouchHeight;
    [SerializeField] private float crouchLerpSpeed;
    [SerializeField] private float colliderCenterMultiplier;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference actionCrouch;
    [SerializeField] private InputActionReference actionMove;
    [SerializeField] private InputActionReference actionJump;
    [SerializeField] private InputActionReference actionPunch;

    [Header("Attack Settings")]
    [SerializeField] private int punchDamage = 10;
    [SerializeField] private int kickDamage = 15;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask attackLayer;
    [SerializeField] private float comboMaxTime = 2f;

    private Vector2 inputDirection;
    private CapsuleCollider col;
    private Rigidbody rb;
    private Animator animator;
    private float originalHeight;
    private bool isCrouching;

    private int comboStepPunch = 0;
    private int comboStepKick = 0;
    private float lastAttackTime = 0f;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        originalHeight = col.height;
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        HandleMovementInput();
        HandleJump();
        HandleCrouch();
        HandleRotation();
        HandlePunchCombo();
    }

    private void FixedUpdate()
    {
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
            float targetY = inputDirection.x > 0 ? 90 : 270;
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

    private void HandlePunchCombo()
    {
        if (actionPunch.action.triggered)
        {
            if (comboStepPunch != 0 && Time.time - lastAttackTime > comboMaxTime)
            {
                comboStepPunch = 0;
                animator.SetInteger("ComboStepPunch", 0);
            }

            comboStepPunch++;
            if (comboStepPunch > 3) comboStepPunch = 1;

            animator.SetInteger("ComboStepPunch", comboStepPunch);
            animator.SetTrigger("Attack");

            PerformAttack(punchDamage);
            CameraShake.Instance.InduceStress(.03f);
            lastAttackTime = Time.time;
        }
    }

    

    private void PerformAttack(int damage)
    {
        Vector3 attackOrigin = transform.position + Vector3.up * 1f;
        Collider[] hits = Physics.OverlapSphere(
            attackOrigin + transform.forward * attackRange / 2f,
            attackRange / 2f,
            attackLayer
        );

        foreach (Collider hit in hits)
        {
            var enemy = hit.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(damage, playerHealth);
        }
    }
}
