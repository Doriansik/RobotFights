using UnityEngine;

public class TrackedEnemy : MonoBehaviour
{
    #region Fields
    [SerializeField] private EnemyManager enemyManager;
    #endregion

    #region Unity Methods
    private void Start()
    {
        if (enemyManager != null)
        {
            enemyManager.RegisterEnemy(this);
        }
    }

    private void OnDestroy()
    {
        if (enemyManager != null)
        {
            enemyManager.UnregisterEnemy(this);
        }
    }
    #endregion
}