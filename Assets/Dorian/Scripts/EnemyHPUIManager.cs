using UnityEngine;
using UnityEngine.UI;

public class EnemyHPUIManager : MonoBehaviour
{
    public static EnemyHPUIManager Instance { get; private set; }

    [SerializeField] private Slider hpSlider;

    private Enemy currentEnemy;

    private void Awake()
    {
        Instance = this;
        if (hpSlider) hpSlider.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        AttackCoordinator.OnTargetChanged += SetCurrentEnemy;
    }

    private void OnDisable()
    {
        AttackCoordinator.OnTargetChanged -= SetCurrentEnemy;
    }

    private void SetCurrentEnemy(GameObject targetObj)
    {
        if (targetObj == null)
        {
            currentEnemy = null;
            if (hpSlider) hpSlider.gameObject.SetActive(false);
            return;
        }

        Enemy enemy = targetObj.GetComponent<Enemy>();
        if (enemy != null && hpSlider != null)
        {
            currentEnemy = enemy;
            hpSlider.maxValue = enemy.MaxHp;
            hpSlider.value = enemy.CurrentHp;
            hpSlider.gameObject.SetActive(true);
        }
    }

    public void UpdateHp(Enemy sender, int hp)
    {
        if (hpSlider && currentEnemy == sender)
        {
            hpSlider.value = hp;
        }
    }
}