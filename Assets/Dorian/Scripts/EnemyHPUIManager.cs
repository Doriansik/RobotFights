using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnemyHPUIManager : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;

    private readonly List<Enemy> aliveEnemies = new();
    private Enemy currentEnemy;

    public static EnemyHPUIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;

        if (!hpSlider)
        {
            Debug.LogError("EnemyHPUIManager: Nie podpiêty hpSlider w Inspectorze!");
            return;
        }

        hpSlider.gameObject.SetActive(false);
    }

    public void RegisterEnemy(Enemy enemy)
    {
        aliveEnemies.Add(enemy);
        SetCurrentEnemy(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        aliveEnemies.Remove(enemy);

        if (currentEnemy == enemy)
        {
            if (aliveEnemies.Count > 0)
                SetCurrentEnemy(aliveEnemies[^1]);
            else
                Clear();
        }
    }

    private void SetCurrentEnemy(Enemy enemy)
    {
        if (!hpSlider) return;

        currentEnemy = enemy;
        hpSlider.maxValue = enemy.MaxHp;
        hpSlider.value = enemy.CurrentHp;
        hpSlider.gameObject.SetActive(true);
    }

    public void UpdateHp(int hp)
    {
        if (!hpSlider) return;
        if (currentEnemy != null)
            hpSlider.value = hp;
    }

    private void Clear()
    {
        currentEnemy = null;
        if (hpSlider) hpSlider.gameObject.SetActive(false);
    }
}
