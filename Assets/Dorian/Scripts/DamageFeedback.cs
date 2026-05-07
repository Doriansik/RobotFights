using UnityEngine;
using System.Collections;
using System;

public class DamageFeedback : MonoBehaviour
{
    [SerializeField] private float knockbackDistance = 0.5f;
    [SerializeField] private float tiltAngle = 15f;
    [SerializeField] private float effectDuration = 0.2f;
    [SerializeField] private Transform modelTransform;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Coroutine activeCoroutine;
    private IDamageable damageable;

    private void Awake()
    {
        if (modelTransform == null)
        {
            modelTransform = transform;
        }

        damageable = GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.OnDamageTaken += PlayFeedback;
        }
    }

    private void OnDestroy()
    {
        if (damageable != null)
        {
            damageable.OnDamageTaken -= PlayFeedback;
        }
    }

    private void PlayFeedback()
    {
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            modelTransform.localPosition = originalPosition;
            modelTransform.localRotation = originalRotation;
        }
        else
        {
            originalPosition = modelTransform.localPosition;
            originalRotation = modelTransform.localRotation;
        }

        activeCoroutine = StartCoroutine(FeedbackRoutine());
    }

    private IEnumerator FeedbackRoutine()
    {
        float elapsed = 0f;
        float halfDuration = effectDuration / 2f;
        Vector3 targetPosition = originalPosition - Vector3.forward * knockbackDistance;
        Quaternion targetRotation = originalRotation * Quaternion.Euler(tiltAngle, 0f, 0f);

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float time = elapsed / halfDuration;
            modelTransform.localPosition = Vector3.Lerp(originalPosition, targetPosition, time);
            modelTransform.localRotation = Quaternion.Lerp(originalRotation, targetRotation, time);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float time = elapsed / halfDuration;
            modelTransform.localPosition = Vector3.Lerp(targetPosition, originalPosition, time);
            modelTransform.localRotation = Quaternion.Lerp(targetRotation, originalRotation, time);
            yield return null;
        }

        modelTransform.localPosition = originalPosition;
        modelTransform.localRotation = originalRotation;
        activeCoroutine = null;
    }
}