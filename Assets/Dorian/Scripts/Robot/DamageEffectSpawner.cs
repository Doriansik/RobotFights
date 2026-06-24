using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class DamageEffectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bloodEffectPrefab;
    [SerializeField] private float effectDestroyTime = 2.0f;
    [SerializeField] private Transform bloodSpawnPoint;

    private Enemy enemyHealth;

    private void Awake()
    {
        enemyHealth = GetComponent<Enemy>();
    }

    private void OnEnable()
    {
        enemyHealth.OnDamageTakenWithPosition += SpawnBlood;
    }

    private void OnDisable()
    {
        enemyHealth.OnDamageTakenWithPosition -= SpawnBlood;
    }

    private void SpawnBlood(Vector3 impactPosition)
    {
        if (bloodEffectPrefab != null)
        {
            Vector3 finalPosition = bloodSpawnPoint != null ? bloodSpawnPoint.position : impactPosition;
            GameObject blood = Instantiate(bloodEffectPrefab, finalPosition, Quaternion.identity);
            Destroy(blood, effectDestroyTime);
        }
    }
}