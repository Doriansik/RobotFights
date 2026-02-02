using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] private Animator animator;
    [SerializeField] private Player player;

    public int CurrentHp { get; private set; }
    public bool IsDead { get; private set; }

    private void Awake()
    {
        CurrentHp = maxHp;
        if (!animator) animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        CurrentHp -= damage;


        if (CurrentHp <= 0)
        {
            IsDead = true;
            animator.SetTrigger("Death");
            player.enabled = false;
            Destroy(gameObject, 3f);
        }
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        CurrentHp += amount;
        if (CurrentHp > maxHp) CurrentHp = maxHp;
    }

    public void ResetHp()
    {
        IsDead = false;
        CurrentHp = maxHp;
    }
}
