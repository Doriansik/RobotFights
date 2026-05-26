using System;
using UnityEngine;

public interface IDamageable
{
    event Action OnDamageTaken;
    void TakeDamage(int damage, Vector3 impactPosition);
}