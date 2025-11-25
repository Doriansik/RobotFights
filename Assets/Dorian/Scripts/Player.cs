using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    
    [Space]
    [Header("GroundCheck")]
    [SerializeField] private float distanceToGround;
    [SerializeField] private float groundOffset = 0.1f;

    [Space]
    [Header("Crouch Settings")]
    [SerializeField] private float crouchForce;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float crouchScaleY = 0.5f;
    [SerializeField] private float crouchLerpSpeed = 10f;
    [SerializeField] private float colliderCenterMultiplier;

    [Space]
    [Header("Input Actions")]
    [SerializeField] private InputActionReference actionCrouch;
    [SerializeField] private InputActionReference actionMove;
    [SerializeField] private InputActionReference actionJump;


    private Vector3 inputDirection;
    private CapsuleCollider col;
    private Rigidbody rb;
    private float originalHeight;
    private float startYscale;
    private bool isCrouching;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        startYscale = transform.localScale.y;
        originalHeight = col.height;
    }

    private void Update()
    {
        HandleMovementInput();
        HandleJump();
        HandleCrouch();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovementInput()
    {
        inputDirection = actionMove.action.ReadValue<Vector2>();
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

            float multiplierScaleY = 0.5f;
            float targetScaleY = startYscale * multiplierScaleY;
            Vector3 newScale = transform.localScale;
            newScale.y = Mathf.Lerp(newScale.y, targetScaleY, Time.deltaTime * crouchLerpSpeed);
            transform.localScale = newScale;
        }
        else
        {
            col.height = Mathf.Lerp(col.height, originalHeight, Time.deltaTime * crouchLerpSpeed);
            col.center = new Vector3(col.center.x, crouchHeight / colliderCenterMultiplier, col.center.z);

            Vector3 newScale = transform.localScale;
            newScale.y = Mathf.Lerp(newScale.y, startYscale, Time.deltaTime * crouchLerpSpeed);
            transform.localScale = newScale;
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
