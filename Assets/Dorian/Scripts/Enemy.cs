using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int healOnDeath = 10;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyAI enemyAI;

    public int CurrentHp { get; private set; }
    public int MaxHp => maxHp;
    public bool IsDead { get; private set; }

    private void Awake()
    {
        CurrentHp = maxHp;
        if (!animator) animator = GetComponent<Animator>();
        if (!enemyAI) enemyAI = GetComponent<EnemyAI>();
    }

    private void Start()
    {
        EnemyHPUIManager.Instance?.RegisterEnemy(this);
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, null);
    }

    public void TakeDamage(int damage, PlayerHealth killer)
    {
        if (IsDead) return;

        CurrentHp -= damage;
        EnemyHPUIManager.Instance?.UpdateHp(CurrentHp);

        if (CurrentHp <= 0)
        {
            IsDead = true;
            EnemyHPUIManager.Instance?.UnregisterEnemy(this);

            if (killer != null) killer.Heal(healOnDeath);

            if (animator) animator.SetTrigger("Death");
            if (enemyAI) enemyAI.enabled = false;

            Destroy(gameObject);
        }
    }
}
