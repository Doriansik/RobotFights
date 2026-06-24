using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MeleeHitbox : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float hitEffectDestroyTime = 2.0f;
    [SerializeField] private Transform hitParticleSpawnPoint;

    private int attackDamage;
    private bool isHitboxActive;
    private Collider triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
        triggerCollider.enabled = false;
    }

    public void ActivateHitbox(int damage)
    {
        attackDamage = damage;
        isHitboxActive = true;
        triggerCollider.enabled = true;
    }

    public void DeactivateHitbox()
    {
        isHitboxActive = false;
        triggerCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isHitboxActive) return;

        if (((1 << other.gameObject.layer) & targetLayer) == 0) return;

        IDamageable damageableTarget = other.GetComponentInParent<IDamageable>();

        if (damageableTarget != null)
        {
            if (hitEffectPrefab != null)
            {
                Vector3 spawnPosition = hitParticleSpawnPoint != null ? hitParticleSpawnPoint.position : other.transform.position;

                GameObject hitEffect = Instantiate(hitEffectPrefab, spawnPosition, Quaternion.identity);
                Destroy(hitEffect, hitEffectDestroyTime);
            }

            Vector3 impactPoint = other.ClosestPoint(transform.position);
            damageableTarget.TakeDamage(attackDamage, impactPoint);
        }
    }
}