using UnityEngine;
using TMPro;
using System.Collections;

public class ComboUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private float displayDuration = 2.0f;
    [SerializeField] private float pulseScale = 1.3f;
    [SerializeField] private float pulseDuration = 0.1f;

    private float hideTime;
    private int lastComboValue;
    private GameObject currentActiveAttacker;
    private Vector3 originalScale;
    private Coroutine pulseCoroutine;

    private void Awake()
    {
        if (comboText != null)
        {
            originalScale = comboText.transform.localScale;
        }
    }

    private void OnEnable()
    {
        RobotCombat.OnComboExecuted += HandleComboExecuted;
        AttackCoordinator.OnTargetChanged += HandleTargetChanged;
    }

    private void OnDisable()
    {
        RobotCombat.OnComboExecuted -= HandleComboExecuted;
        AttackCoordinator.OnTargetChanged -= HandleTargetChanged;
    }

    private void HandleTargetChanged(GameObject target)
    {
        currentActiveAttacker = target;

        if (comboText)
        {
            comboText.gameObject.SetActive(false);
            comboText.transform.localScale = originalScale;
        }
        lastComboValue = 0;
    }

    private void HandleComboExecuted(GameObject sender, string robotName, int comboStep)
    {
        if (sender != currentActiveAttacker) return;

        if (comboStep > 1)
        {
            comboText.text = $"{robotName} Combo: x{comboStep}!";
            comboText.gameObject.SetActive(true);
            hideTime = Time.time + displayDuration;

            if (comboStep > lastComboValue)
            {
                TriggerPulse();
            }
        }
        else if (comboStep == 0 && lastComboValue > 1)
        {
            comboText.text = $"{robotName} Combo: 0!";
            comboText.gameObject.SetActive(true);
            hideTime = Time.time + displayDuration;
            TriggerPulse();
        }

        lastComboValue = comboStep;
    }

    private void TriggerPulse()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
        }
        pulseCoroutine = StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        float elapsed = 0;
        Vector3 targetScale = originalScale * pulseScale;

        while (elapsed < pulseDuration)
        {
            comboText.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / pulseDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0;
        while (elapsed < pulseDuration)
        {
            comboText.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / pulseDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        comboText.transform.localScale = originalScale;
    }

    private void Update()
    {
        if (comboText != null && comboText.gameObject.activeSelf && Time.time > hideTime)
        {
            comboText.gameObject.SetActive(false);
            lastComboValue = 0;
            comboText.transform.localScale = originalScale;
        }
    }
}