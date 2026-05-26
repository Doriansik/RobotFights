using UnityEngine;
using System;

[RequireComponent(typeof(EnemyEnergy))]
public class RobotCombat : MonoBehaviour
{
    public static event Action<GameObject, string, int> OnComboExecuted;

    [SerializeField] private AttackDataStats[] comboSequence;
    [SerializeField] private float comboResetTime = 2.5f;
    [SerializeField] private Animator animator;
    [SerializeField] private Enemy enemyStats;
    [SerializeField] private EnemyEnergy enemyEnergy;
    [SerializeField] private string robotName;
    [SerializeField] private float energyAmount;

    [SerializeField] private MeleeHitbox[] handHitboxes;
    [SerializeField] private CombatAnimationDispatcher animationDispatcher;

    private int currentComboIndex;
    private int totalComboCounter;
    private float lastAttackTime;
    private int activeAttackDamage;
    private readonly int sequenceStartIndex = 0;

    private void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!enemyStats) enemyStats = GetComponent<Enemy>();
        if (!enemyEnergy) enemyEnergy = GetComponent<EnemyEnergy>();

        ResetComboValues();
        lastAttackTime = -comboResetTime;
    }

    private void OnEnable()
    {
        if (enemyStats)
        {
            enemyStats.OnDamageTaken += HandleDamageTakenReaction;
        }

        if (animationDispatcher)
        {
            animationDispatcher.OnHitboxEnableRequested += EnableAttackHitboxes;
            animationDispatcher.OnHitboxDisableRequested += DisableAttackHitboxes;
        }
    }

    private void OnDisable()
    {
        if (enemyStats)
        {
            enemyStats.OnDamageTaken -= HandleDamageTakenReaction;
        }

        if (animationDispatcher)
        {
            animationDispatcher.OnHitboxEnableRequested -= EnableAttackHitboxes;
            animationDispatcher.OnHitboxDisableRequested -= DisableAttackHitboxes;
        }
    }

    private void HandleDamageTakenReaction()
    {
        ResetComboValues();
        DisableAttackHitboxes();
    }

    private void EnableAttackHitboxes()
    {
        foreach (MeleeHitbox hitbox in handHitboxes)
        {
            hitbox.ActivateHitbox(activeAttackDamage);
        }
    }

    private void DisableAttackHitboxes()
    {
        foreach (MeleeHitbox hitbox in handHitboxes)
        {
            hitbox.DeactivateHitbox();
        }
    }

    public void TryAttack()
    {
        if (Time.time - lastAttackTime > comboResetTime)
        {
            ResetComboValues();
        }

        AttackDataStats currentAttack = comboSequence[currentComboIndex];

        if (Time.time < lastAttackTime + currentAttack.CooldownBeforeNextAttack) return;

        if (!enemyEnergy.HasEnoughEnergy(energyAmount)) return;

        enemyEnergy.ConsumeEnergy(energyAmount);

        totalComboCounter++;
        activeAttackDamage = currentAttack.Damage;

        ExecuteAttack(currentAttack);

        currentComboIndex++;
        if (currentComboIndex >= comboSequence.Length)
        {
            currentComboIndex = sequenceStartIndex;
        }

        lastAttackTime = Time.time;
    }

    private void ResetComboValues()
    {
        currentComboIndex = sequenceStartIndex;
        totalComboCounter = sequenceStartIndex;

        OnComboExecuted?.Invoke(gameObject, robotName, totalComboCounter);
    }

    private void ExecuteAttack(AttackDataStats attack)
    {
        if (animator) animator.SetTrigger(attack.AnimationTrigger);

        OnComboExecuted?.Invoke(gameObject, robotName, totalComboCounter);
    }
}