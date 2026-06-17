using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region Events
    public event Action OnAllEnemiesDefeated;
    #endregion

    #region Fields
    [SerializeField] private List<TrackedEnemy> activeEnemies = new List<TrackedEnemy>();
    private readonly int emptyListCount = 0;
    #endregion

    #region Public Methods
    public void RegisterEnemy(TrackedEnemy enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(TrackedEnemy enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            CheckWinCondition();
        }
    }
    #endregion

    #region Private Methods
    private void CheckWinCondition()
    {
        if (activeEnemies.Count == emptyListCount)
        {
            OnAllEnemiesDefeated?.Invoke();
        }
    }
    #endregion
}