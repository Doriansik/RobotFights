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

        if (isActiveFrame)
        {
            DetectHitsManually();
        }
    }

    public void DeactivateHitbox()
    {
        DamageCollider.enabled = false;
        currentAttackData = null;
        alreadyHitColliders.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentAttackData == null || !DamageCollider.enabled) return;

        if (DamageCollider.bounds.Intersects(other.bounds))
        {
            ProcessHit(other);
        }
    }

    private void DetectHitsManually()
    {
        if (currentAttackData == null) return;

        Collider[] hits = new Collider[0];

        if (DamageCollider is BoxCollider box)
        {
            Vector3 center = box.transform.TransformPoint(box.center);
            Vector3 lossy = box.transform.lossyScale;
            Vector3 scale = new Vector3(Mathf.Abs(lossy.x), Mathf.Abs(lossy.y), Mathf.Abs(lossy.z));
            Vector3 halfExtents = Vector3.Scale(box.size, scale) * 0.5f;

            hits = Physics.OverlapBox(center, halfExtents, box.transform.rotation);
        }
        else if (DamageCollider is SphereCollider sphere)
        {
            Vector3 center = sphere.transform.TransformPoint(sphere.center);
            float maxScale = Mathf.Max(Mathf.Abs(sphere.transform.lossyScale.x), Mathf.Abs(sphere.transform.lossyScale.y), Mathf.Abs(sphere.transform.lossyScale.z));
            float radius = sphere.radius * maxScale;

            hits = Physics.OverlapSphere(center, radius);
        }
        else
        {
            hits = Physics.OverlapBox(
                DamageCollider.bounds.center,
                DamageCollider.bounds.extents,
                Quaternion.identity
            );
        }

        foreach (Collider hitCollider in hits)
        {
            ProcessHit(hitCollider);
        }
    }

    private void ProcessHit(Collider hitCollider)
    {
        if (hitCollider.transform.root == transform.root) return;

        if (alreadyHitColliders.Contains(hitCollider)) return;

        IDamageable target = hitCollider.GetComponent<IDamageable>();
        if (target == null)
        {
            target = hitCollider.GetComponentInParent<IDamageable>();
        }

        if (target != null)
        {
            alreadyHitColliders.Add(hitCollider);
            target.TakeDamage(Mathf.RoundToInt(currentAttackData.Damage));

            SpawnHitEffect(hitCollider);
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

        GameObject effect = Instantiate(hitEffectPrefab, spawnPosition, Quaternion.identity);
        Destroy(effect, 2f);
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