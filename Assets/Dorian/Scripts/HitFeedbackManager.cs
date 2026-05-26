using UnityEngine;

public class HitFeedbackManager : MonoBehaviour
{
    [SerializeField] private HitboxManager hitboxManager;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private float cameraShakeStress;
    [SerializeField] private float hitStopDuration;
    [SerializeField] private float effectDestroyDelay = 2f;

    private void OnEnable()
    {
        if (hitboxManager != null)
        {
            hitboxManager.OnHitDetected += HandleHitFeedback;
        }
    }

    private void OnDisable()
    {
        if (hitboxManager != null)
        {
            hitboxManager.OnHitDetected -= HandleHitFeedback;
        }
    }

    private void HandleHitFeedback(Vector3 hitPosition)
    {
        if (hitEffectPrefab != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, hitPosition, Quaternion.identity);
            Destroy(effect, effectDestroyDelay);
        }

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.InduceStress(cameraShakeStress);
        }

        if (HitStopManager.Instance != null)
        {
            HitStopManager.Instance.TriggerHitStop(hitStopDuration);
        }
    }
}