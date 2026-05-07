using UnityEngine;
using System;

public class RobotCombat : MonoBehaviour
{
    public static event Action<GameObject, string, int> OnComboExecuted;

    [SerializeField] private AttackData[] comboSequence;
    [SerializeField] private float comboResetTime = 2.5f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float attackVerticalOffset = 1.0f;
    [SerializeField] private float attackForwardOffsetMultiplier = 0.5f;
    [SerializeField] private Animator animator;
    [SerializeField] private Enemy enemyStats;
    [SerializeField] private string robotName;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private Transform hitEffectSpawnPoint;

    private int currentComboIndex;
    private int totalComboCounter;
    private float lastAttackTime;

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        if (!enemyStats) enemyStats = GetComponent<Enemy>();

        ResetComboValues();
        lastAttackTime = -comboResetTime;
    }

    private void OnEnable()
    {
        if (enemyStats)
        {
            enemyStats.OnDamageTaken += ResetComboValues;
        }
    }

    private void OnDisable()
    {
        if (enemyStats)
        {
            enemyStats.OnDamageTaken -= ResetComboValues;
        }
    }

    public void TryAttack()
    {
        if (Time.time - lastAttackTime > comboResetTime)
        {
            ResetComboValues();
        }

        AttackData currentAttack = comboSequence[currentComboIndex];

        if (Time.time < lastAttackTime + currentAttack.CooldownBeforeNextAttack) return;

        totalComboCounter++;
        ExecuteAttack(currentAttack);

        currentComboIndex++;
        if (currentComboIndex >= comboSequence.Length)
        {
            currentComboIndex = 0;
        }

        lastAttackTime = Time.time;
    }

    private void ResetComboValues()
    {
        currentComboIndex = 0;
        totalComboCounter = 0;

        OnComboExecuted?.Invoke(gameObject, robotName, totalComboCounter);
    }

    private void ExecuteAttack(AttackData attack)
    {
        if (animator) animator.SetTrigger(attack.AnimationTrigger);

        OnComboExecuted?.Invoke(gameObject, robotName, totalComboCounter);

        Vector3 origin = transform.position + Vector3.up * attackVerticalOffset + transform.forward * (attack.AttackRange * attackForwardOffsetMultiplier);
        Collider[] hits = Physics.OverlapSphere(origin, attack.AttackRadius, targetLayer);

        for (int i = 0; i < hits.Length; i++)
        {
            IDamageable dmg = hits[i].GetComponentInParent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(attack.Damage);
                SpawnHitEffect(hits[i]);
                break;
            }
        }
    }

    private void SpawnHitEffect(Collider hitCollider)
    {
        if (hitEffectPrefab != null)
        {
            Vector3 spawnPosition = hitEffectSpawnPoint != null ? hitEffectSpawnPoint.position : hitCollider.ClosestPoint(transform.position);
            Instantiate(hitEffectPrefab, spawnPosition, Quaternion.identity);
        }
    }
}