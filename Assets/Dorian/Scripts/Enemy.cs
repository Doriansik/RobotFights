using UnityEngine;
using System;

public class Enemy : MonoBehaviour, IDamageable
{
    public event Action OnDamageTaken;
    public event Action<Vector3> OnDamageTakenWithPosition;

    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;

    [SerializeField] private int maxHp = 100;

    private int currentHp;
    private readonly int minHealth = 0;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage, Vector3 impactPosition)
    {
        OnDamageTaken?.Invoke();
        OnDamageTakenWithPosition?.Invoke(impactPosition);

        if (damage <= minHealth || currentHp <= minHealth) return;

        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, minHealth);

        if (EnemyHPUIManager.Instance != null)
        {
            EnemyHPUIManager.Instance.UpdateHp(this, currentHp);
        }

        if (currentHp == minHealth)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}