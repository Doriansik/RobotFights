using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rotateSpeed;


    [Space]
    [Header("GroundCheck")]
    [SerializeField] private float distanceToGround;
    [SerializeField] private float groundOffset;

    [Space]
    [Header("Crouch Settings")]
    [SerializeField] private float crouchForce;
    [SerializeField] private float crouchHeight;
    [SerializeField] private float crouchScaleY;
    [SerializeField] private float crouchLerpSpeed;
    [SerializeField] private float colliderCenterMultiplier;

    [Space]
    [Header("Input Actions")]
    [SerializeField] private InputActionReference actionCrouch;
    [SerializeField] private InputActionReference actionMove;
    [SerializeField] private InputActionReference actionJump;


    private Vector2 inputDirection;
    private CapsuleCollider col;
    private Rigidbody rb;
    private Animator animator;
    private float originalHeight;
    private float startYscale;
    private bool isCrouching;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();

        startYscale = transform.localScale.y;
        originalHeight = col.height;
    }

    private void Update()
    {
        HandleMovementInput();
        HandleJump();
        HandleCrouch();
        HandleRotation();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovementInput()
    {
        inputDirection = actionMove.action.ReadValue<Vector2>();
        
        if (inputDirection != Vector2.zero)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }


    private void HandleRotation()
    {
        if (inputDirection.x != 0)
        {
            float targetY = inputDirection.x > 0 ? 90 : 270;
            Quaternion targetRot = Quaternion.Euler(0, targetY, 0);

            transform.rotation = Quaternion.Lerp(transform.rotation,targetRot,Time.deltaTime * rotateSpeed);
        }

    }


    private void HandleJump()
    {
        if (actionJump.action.triggered && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("IsJumping", true);
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }
    }

    private void HandleCrouch()
    {
        isCrouching = actionCrouch.action.IsPressed();

        if (isCrouching)
        {
            col.height = Mathf.Lerp(col.height, crouchHeight, Time.deltaTime * crouchLerpSpeed);
            col.center = new Vector3(col.center.x, crouchHeight / colliderCenterMultiplier, col.center.z);
            animator.SetBool("IsCrounching", true);

            //float multiplierScaleY = 0.5f;
            //float targetScaleY = startYscale * multiplierScaleY;
            //Vector3 newScale = transform.localScale;
            //newScale.y = Mathf.Lerp(newScale.y, targetScaleY, Time.deltaTime * crouchLerpSpeed);
            //transform.localScale = newScale;
        }
        else
        {
            col.height = Mathf.Lerp(col.height, originalHeight, Time.deltaTime * crouchLerpSpeed);
            col.center = new Vector3(col.center.x, crouchHeight / colliderCenterMultiplier, col.center.z);
            animator.SetBool("IsCrounching", false);

            //Vector3 newScale = transform.localScale;
            //newScale.y = Mathf.Lerp(newScale.y, startYscale, Time.deltaTime * crouchLerpSpeed);
            //transform.localScale = newScale;
        }
    }

    private void HandleMovement()
    {
        Vector3 movement = new Vector3(inputDirection.x, 0, 0) * (moveSpeed * Time.deltaTime);
        rb.MovePosition(rb.position + movement);
    }

    private bool IsGrounded()
    {
        // Do czasu jak nie bedzie modeli
        Vector3 origin = transform.position + Vector3.up * groundOffset;
        //////////////////////////////////////////////////////////////////
        bool hit = Physics.Raycast(origin, Vector3.down, out RaycastHit raycastHit, distanceToGround + groundOffset);


        //Do testow w edytorze
        Debug.DrawRay(origin, Vector3.down * (distanceToGround + groundOffset), hit ? Color.green : Color.red);
        /////////////////////////////////////////////////////////////////
        return hit;
    }
}