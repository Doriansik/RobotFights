using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public event Action<int> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnDamageTaken;
    public event Action<Vector3> OnDamageTakenWithPosition;

    [SerializeField] private int maxHp = 100;
    [SerializeField] private float deathDestroyDelay = 3f;

    public int CurrentHp { get; private set; }
    public bool IsDead { get; private set; }

    private readonly int minHealth = 0;

    private void Awake()
    {
        CurrentHp = maxHp;
    }

    public void TakeDamage(int damage, Vector3 impactPosition)
    {
        OnDamageTaken?.Invoke();
        OnDamageTakenWithPosition?.Invoke(impactPosition);

        if (damage <= minHealth || IsDead) return;

        CurrentHp -= damage;
        OnHealthChanged?.Invoke(CurrentHp);

        if (CurrentHp <= minHealth)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= minHealth || IsDead) return;

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
}