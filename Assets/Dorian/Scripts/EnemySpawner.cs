using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float spawnDelay = 2f;

    [Header("Refs")]
    [SerializeField] private Transform player;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnDelay);
    }

    private void SpawnEnemy()
    {
        if (!enemyPrefab || spawnPoints.Length == 0 || !player) return;

        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemyObj = Instantiate(enemyPrefab, point.position, point.rotation);

        EnemyAI enemyAI = enemyObj.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.SetTarget(player);
        }
    }
}
