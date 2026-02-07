using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyAI enemyAI;
    [SerializeField] private Slider slider;

    public int CurrentHp { get; private set; }
    public bool IsDead { get; private set; }

    private void Awake()
    {
        CurrentHp = maxHp;
        slider.maxValue = maxHp;
        slider.value = CurrentHp;
        if (!animator) animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        CurrentHp -= damage;
        slider.value = CurrentHp;


        if (CurrentHp <= 0)
        {
            IsDead = true;
            animator.SetTrigger("Death");
            enemyAI.enabled = false;
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
