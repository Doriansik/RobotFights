using UnityEngine;
using System;

public class Enemy : MonoBehaviour, IDamageable
{
    public event Action OnDamageTaken;

    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;

    [SerializeField] private int maxHp = 100;

    [Header("Effects")]
    [SerializeField] private GameObject bloodEffectPrefab;
    [SerializeField] private Transform bloodEffectSpawnPoint;

    private int currentHp;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int damage)
    {
        SpawnBloodEffect();
        OnDamageTaken?.Invoke();

        if (damage <= 0 || currentHp <= 0) return;

        currentHp -= damage;
        currentHp = Mathf.Max(currentHp, 0);

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