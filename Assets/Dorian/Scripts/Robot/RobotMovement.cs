using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RobotMovement : MonoBehaviour, IAttacker
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float attackDistance = 1.6f;
    [SerializeField] private float waitDistance = 4.0f;
    [SerializeField] private float tokenRequestDistance = 5.0f;
    [SerializeField] private float rotateSpeed = 12f;
    [SerializeField] private Animator animator;
    [SerializeField] private RobotCombat combatModule;

    private Rigidbody rb;
    private bool isAttackingRole;
    private const float DistanceEpsilon = 0.001f;
    private const string MovementAnimatorParameter = "Movement";

    public GameObject GameObject => gameObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (animator == null) animator = GetComponent<Animator>();
        if (combatModule == null) combatModule = GetComponent<RobotCombat>();
    }

    private void OnDestroy()
    {
        if (AttackCoordinator.Instance != null)
        {
            AttackCoordinator.Instance.ReleaseToken(this);
        }
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        float dist = toTarget.magnitude;

        if (!isAttackingRole && AttackCoordinator.Instance != null && dist <= tokenRequestDistance)
        {
            isAttackingRole = AttackCoordinator.Instance.TryGetAttackToken(this);
        }
    }

    private void HandleRotation(Vector3 toTarget, float distanceToTarget)
    {
        if (distanceToTarget > DistanceEpsilon)
        {
            Quaternion targetRotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotateSpeed);
        }
    }

    private void HandleMovementAndCombat(Vector3 moveDirection, float distanceToTarget)
    {
        float currentTargetDistance = isAttackingRole ? attackDistance : waitDistance;
        bool canMove = distanceToTarget > currentTargetDistance;

        if (dist > DistanceEpsilon)
        {
            animator.SetBool(MovementAnimatorParameter, canMove);
        }

        if (canMove)
        {
            Vector3 moveVector = moveDirection * (moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(rb.position + moveVector);
        }
        else if (isAttackingRole)
        {
            if (combatModule != null)
            {
                combatModule.TryAttack();
            }
        }
    }
}