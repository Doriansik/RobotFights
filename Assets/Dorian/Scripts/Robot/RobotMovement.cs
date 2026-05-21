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

    public GameObject GameObject => gameObject;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        if (!animator) animator = GetComponent<Animator>();
        if (!combatModule) combatModule = GetComponent<RobotCombat>();
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
        if (!target) return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        float dist = toTarget.magnitude;

        if (!isAttackingRole && AttackCoordinator.Instance != null && dist <= tokenRequestDistance)
        {
            isAttackingRole = AttackCoordinator.Instance.TryGetAttackToken(this);
        }

        float currentTargetDistance = isAttackingRole ? attackDistance : waitDistance;

        if (dist > DistanceEpsilon)
        {
            Quaternion rot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.fixedDeltaTime * rotateSpeed);
        }

        bool canMove = dist > currentTargetDistance;
        if (animator) animator.SetBool("Movement", canMove);

        if (canMove)
        {
            Vector3 move = toTarget.normalized * (moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(rb.position + move);
        }
        else if (isAttackingRole)
        {
            combatModule.TryAttack();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}