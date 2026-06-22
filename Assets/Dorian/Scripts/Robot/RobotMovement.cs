using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(EnemyEnergy))]
public class RobotMovement : MonoBehaviour, IAttacker
{
    #region Constants
    private const float DistanceEpsilon = 0.001f;
    private const float ZeroFloat = 0f;
    #endregion

    #region Serialized Fields
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float attackDistance = 1.6f;
    [SerializeField] private float waitDistance = 4.0f;
    [SerializeField] private float tokenRequestDistance = 5.0f;
    [SerializeField] private float rotateSpeed = 12f;
    [SerializeField] private float normalSpeedMultiplier = 1f;
    [SerializeField] private float exhaustedSpeedMultiplier = 0.5f;
    [SerializeField] private Animator animator;
    [SerializeField] private RobotCombat combatModule;
    [SerializeField] private EnemyEnergy enemyEnergy;
    #endregion

    #region Private Fields
    private Rigidbody rb;
    private bool isAttackingRole;
    #endregion

    #region Properties
    public GameObject GameObject => gameObject;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        if (!animator) animator = GetComponent<Animator>();
        if (!combatModule) combatModule = GetComponent<RobotCombat>();
        if (!enemyEnergy) enemyEnergy = GetComponent<EnemyEnergy>();
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
        toTarget.y = ZeroFloat;
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
            float currentMultiplier = enemyEnergy.IsExhausted ? exhaustedSpeedMultiplier : normalSpeedMultiplier;
            Vector3 move = toTarget.normalized * (moveSpeed * currentMultiplier * Time.fixedDeltaTime);
            rb.MovePosition(rb.position + move);
        }
        else if (isAttackingRole)
        {
            combatModule.TryAttack();
        }
    }
    #endregion

    #region Methods
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    #endregion
}