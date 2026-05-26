using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerDamageEffectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bloodEffectPrefab;
    [SerializeField] private float effectDestroyTime = 2.0f;
    [SerializeField] private Transform bloodSpawnPoint;

    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        playerHealth.OnDamageTakenWithPosition += SpawnBlood;
    }

    private void OnDisable()
    {
        playerHealth.OnDamageTakenWithPosition -= SpawnBlood;
    }

    private void SpawnBlood(Vector3 impactPosition)
    {
        if (bloodEffectPrefab != null)
        {
            Vector3 finalSpawnPosition = bloodSpawnPoint != null ? bloodSpawnPoint.position : impactPosition;
            GameObject blood = Instantiate(bloodEffectPrefab, finalSpawnPosition, Quaternion.identity);
            Destroy(blood, effectDestroyTime);
        }
    }
}