using System;

public interface IDamageable
{
    event Action OnDamageTaken;
    void TakeDamage(int damage);
}