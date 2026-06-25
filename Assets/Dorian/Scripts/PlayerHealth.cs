using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    #region Constants
    private const int DefaultMaxHp = 100;
    private const float DefaultDeathDelay = 3f;
    private const int MinHealth = 0;
    #endregion

    #region Events
    public event Action<int> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnDamageTaken;
    public event Action<Vector3> OnDamageTakenWithPosition;
    #endregion

    #region Configuration
    [SerializeField] private int maxHp = DefaultMaxHp;
    [SerializeField] private float deathDestroyDelay = DefaultDeathDelay;
    #endregion

    #region Properties
    public int CurrentHp { get; private set; }
    public bool IsDead { get; private set; }
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        CurrentHp = maxHp;
    }
    #endregion

    #region Methods
    public void TakeDamage(int damage, Vector3 impactPosition)
    {
        OnDamageTaken?.Invoke();
        OnDamageTakenWithPosition?.Invoke(impactPosition);

        if (damage <= MinHealth || IsDead) return;

        CurrentHp -= damage;
        OnHealthChanged?.Invoke(CurrentHp);

        if (CurrentHp <= MinHealth)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= MinHealth || IsDead) return;

        CurrentHp = Mathf.Min(CurrentHp + amount, maxHp);
        OnHealthChanged?.Invoke(CurrentHp);
    }

    public void ResetHp()
    {
        IsDead = false;
        CurrentHp = maxHp;
        OnHealthChanged?.Invoke(CurrentHp);
    }

    private void Die()
    {
        IsDead = true;
        OnDeath?.Invoke();
        Destroy(gameObject, deathDestroyDelay);
    }
    #endregion
}