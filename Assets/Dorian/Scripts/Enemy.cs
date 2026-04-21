using UnityEngine;
using System;

public class Enemy : MonoBehaviour, IDamageable
{
    public event Action OnDamageTaken;

    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;

    [SerializeField] private int maxHp = 100;

    private int currentHp;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        if (currentHp <= 0) return;

        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);

        OnDamageTaken?.Invoke();

        if (EnemyHPUIManager.Instance != null)
        {
            EnemyHPUIManager.Instance.UpdateHp(this, currentHp);
        }

        if (currentHp == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}