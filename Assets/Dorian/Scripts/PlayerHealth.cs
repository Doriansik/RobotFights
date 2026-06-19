using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public event Action<int> OnHealthChanged;
    public event Action OnDeath;
    public event Action OnDamageTaken;

    [SerializeField] private int maxHp = 100;
    [SerializeField] private float deathDestroyDelay = 3f;

    [Header("Effects")]
    [SerializeField] private GameObject bloodEffectPrefab;
    [SerializeField] private Transform bloodEffectSpawnPoint;

    public int CurrentHp { get; private set; }
    public bool IsDead { get; private set; }

    private void Awake()
    {
        CurrentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        SpawnBloodEffect();
        OnDamageTaken?.Invoke();

        if (damage <= 0 || IsDead) return;

        CurrentHp -= damage;
        OnHealthChanged?.Invoke(CurrentHp);

        if (CurrentHp <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead) return;

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

    private void SpawnBloodEffect()
    {
        if (bloodEffectPrefab != null)
        {
            Vector3 spawnPosition = bloodEffectSpawnPoint != null ? bloodEffectSpawnPoint.position : transform.position;
            GameObject blood = Instantiate(bloodEffectPrefab, spawnPosition, Quaternion.identity);
            Destroy(blood, 2f);
        }
    }
}