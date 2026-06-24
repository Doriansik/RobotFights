using UnityEngine;

public class GameUIController : MonoBehaviour
{
    #region Fields
    [SerializeField] private EnemyManager enemyManager;
    [SerializeField] private GameObject winMessagePanel;
    [SerializeField] private GameObject startGamePanel;
    #endregion

    #region Unity Methods
    private void OnEnable()
    {
        if (enemyManager != null)
        {
            enemyManager.OnAllEnemiesDefeated += ShowWinMessage;
        }
    }

    private void OnDisable()
    {
        if (enemyManager != null)
        {
            enemyManager.OnAllEnemiesDefeated -= ShowWinMessage;
        }
    }
    #endregion

    #region Private Methods
    private void ShowWinMessage()
    {
        if (winMessagePanel != null)
        {
            winMessagePanel.SetActive(true);
        }
    }
    #endregion

    #region Public Methods
    public void OnStartGameAfterStartOfTheRound()
    {
        startGamePanel.SetActive(false);
        Time.timeScale = 1.0f;
    }
    #endregion
}