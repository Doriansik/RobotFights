using UnityEngine;
using System;

public class Enemy : MonoBehaviour, IDamageable
{
    #region Events
    public event Action OnDamageTaken;
    public event Action<Vector3> OnDamageTakenWithPosition;
    #endregion

    #region Properties
    public int MaxHp => maxHp;
    public int CurrentHp => currentHp;
    #endregion

    #region Serialized Fields
    [SerializeField] private int maxHp = 100;
    #endregion

    #region Private Fields
    private int currentHp;
    private readonly int minHealth = 0;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        currentHp = maxHp;
    }
    #endregion

    #region Health Logic
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
    #endregion
}