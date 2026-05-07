using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float stopDistance = 1.6f;
    [SerializeField] private float minRotationDistance = 0.001f;

    [Header("Attack")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackCooldown = 1.0f;
    [SerializeField] private float attackRange = 1.7f;
    [SerializeField] private float attackRadius = 0.6f;
    [SerializeField] private float attackOriginYOffset = 1f;
    [SerializeField] private float attackOriginForwardMultiplier = 0.5f;
    [SerializeField] private LayerMask targetLayer;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffectPrefab;

    [Header("Rotation")]
    [SerializeField] private float rotateSpeed = 12f;

    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;

    private float lastAttackTime;

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!rb) rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void FixedUpdate()
    {
        if (!target) return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        float dist = toTarget.magnitude;

        if (dist > minRotationDistance)
        {
            Quaternion rot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.fixedDeltaTime * rotateSpeed);
        }

        bool canMove = dist > stopDistance;
        if (animator) animator.SetBool("Movement", canMove);

        if (canMove)
        {
            Vector3 move = toTarget.normalized * (moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(rb.position + move);
        }
        else
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        if (animator) animator.SetTrigger("Attack");

        PerformAttack();

        lastAttackTime = Time.time;
    }

    private void PerformAttack()
    {
        Vector3 origin = transform.position + Vector3.up * attackOriginYOffset + transform.forward * (attackRange * attackOriginForwardMultiplier);
        Collider[] hits = Physics.OverlapSphere(origin, attackRadius, targetLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            IDamageable dmg = hits[i].GetComponentInParent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage);
                SpawnHitEffect(hits[i]);
                break;
            }
        }
    }

    private void SpawnHitEffect(Collider hitCollider)
    {
        if (hitEffectPrefab != null)
        {
            Vector3 hitPoint = hitCollider.ClosestPoint(transform.position);
            Instantiate(hitEffectPrefab, hitPoint, Quaternion.identity);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}