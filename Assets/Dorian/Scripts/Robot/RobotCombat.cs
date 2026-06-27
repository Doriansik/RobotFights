using UnityEngine;
using System;

[RequireComponent(typeof(EnemyEnergy))]
[RequireComponent(typeof(Enemy))]
public class RobotCombat : MonoBehaviour
{
    #region Constants
    private const float CrossFadeDuration = 0.1f;
    private const int SequenceStartIndex = 0;
    #endregion

    #region Serialized Fields
    [SerializeField] private AttackDataStats[] comboSequence;
    [SerializeField] private float comboResetTime = 2.5f;
    [SerializeField] private Animator animator;
    [SerializeField] private Enemy enemyStats;
    [SerializeField] private EnemyEnergy enemyEnergy;
    [SerializeField] private float energyAmount;

    [SerializeField] private MeleeHitbox[] handHitboxes;
    [SerializeField] private CombatAnimationDispatcher animationDispatcher;
    #endregion

    #region Private Fields
    private int currentComboIndex;
    private float lastAttackTime;
    private int activeAttackDamage;
    private int hitAnimationHash;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        InitializeDependencies();
        hitAnimationHash = Animator.StringToHash("HitReaction");
        ResetComboValues();
        lastAttackTime = -comboResetTime;
    }

    private void OnEnable()
    {
        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
    }
    #endregion

    #region Initialization
    private void InitializeDependencies()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!enemyStats) enemyStats = GetComponent<Enemy>();
        if (!enemyEnergy) enemyEnergy = GetComponent<EnemyEnergy>();
    }

    private void SubscribeEvents()
    {
        if (enemyStats) enemyStats.OnDamageTaken += HandleDamageTakenReaction;

        if (animationDispatcher)
        {
            animationDispatcher.OnHitboxEnableRequested += EnableAttackHitboxes;
            animationDispatcher.OnHitboxDisableRequested += DisableAttackHitboxes;
        }
    }

    private void UnsubscribeEvents()
    {
        if (enemyStats) enemyStats.OnDamageTaken -= HandleDamageTakenReaction;

        if (animationDispatcher)
        {
            animationDispatcher.OnHitboxEnableRequested -= EnableAttackHitboxes;
            animationDispatcher.OnHitboxDisableRequested -= DisableAttackHitboxes;
        }
    }
    #endregion

    #region Combat Logic
    private void HandleDamageTakenReaction()
    {
        ResetComboValues();
        DisableAttackHitboxes();

        if (animator) animator.CrossFade(hitAnimationHash, CrossFadeDuration);
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
        activeAttackDamage = currentAttack.Damage;

        ExecuteAttack(currentAttack);
        AdvanceComboSequence();

        lastAttackTime = Time.time;
    }

    private void AdvanceComboSequence()
    {
        currentComboIndex++;
        if (currentComboIndex >= comboSequence.Length)
        {
            currentComboIndex = SequenceStartIndex;
        }
    }

    private void ResetComboValues()
    {
        currentComboIndex = SequenceStartIndex;
    }

    private void ExecuteAttack(AttackDataStats attack)
    {
        if (animator) animator.SetTrigger(attack.AnimationTrigger);
    }
    #endregion
}