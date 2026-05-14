using UnityEngine;
using System.Collections.Generic;

public class HitboxManager : MonoBehaviour
{
    public HitboxConfig Configuration;
    public Collider DamageCollider;

    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private Transform hitEffectSpawnPoint;
    [SerializeField] private float cameraShakeStress;
    [SerializeField] private float hitStopDuration;

    private AttackData currentAttackData;
    private HashSet<Collider> alreadyHitColliders = new HashSet<Collider>();

    public void PrepareHitbox(AttackData attackData)
    {
        currentAttackData = attackData;
        DamageCollider.transform.localScale = Configuration.GetScale(attackData.HitboxSize);
        DamageCollider.enabled = false;
        alreadyHitColliders.Clear();
    }

    public void ProcessHitboxState(float currentFrame, AttackData attackData)
    {
        bool isActiveFrame = currentFrame >= attackData.WindUpFrames && currentFrame <= (attackData.WindUpFrames + attackData.ActiveFrames);
        DamageCollider.enabled = isActiveFrame;
    }

    public void DeactivateHitbox()
    {
        DamageCollider.enabled = false;
        currentAttackData = null;
        alreadyHitColliders.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentAttackData == null || alreadyHitColliders.Contains(other)) return;

        IDamageable target = other.GetComponent<IDamageable>();

        if (target == null)
        {
            target = other.GetComponentInParent<IDamageable>();
        }

        if (target != null)
        {
            alreadyHitColliders.Add(other);
            target.TakeDamage(Mathf.RoundToInt(currentAttackData.Damage));
            SpawnHitEffect(other);
            TriggerCameraShake();
            TriggerHitStop();
        }
    }

    private void SpawnHitEffect(Collider hitCollider)
    {
        if (hitEffectPrefab == null) return;

        Vector3 spawnPosition = hitEffectSpawnPoint != null
            ? hitEffectSpawnPoint.position
            : hitCollider.ClosestPoint(transform.position);

        Instantiate(hitEffectPrefab, spawnPosition, Quaternion.identity);
    }

    private void TriggerCameraShake()
    {
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.InduceStress(cameraShakeStress);
        }
    }

    private void TriggerHitStop()
    {
        if (HitStopManager.Instance != null)
        {
            HitStopManager.Instance.TriggerHitStop(hitStopDuration);
        }
    }
}