using System.Collections;
using UnityEngine;

public class HitStopManager : MonoBehaviour
{
    public static HitStopManager Instance { get; private set; }

    private bool isStopping;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void TriggerHitStop(float duration)
    {
        if (isStopping) return;
        StartCoroutine(HitStopCoroutine(duration));
    }

    private IEnumerator HitStopCoroutine(float duration)
    {
        isStopping = true;
        Time.timeScale = CombatConstants.StoppedTimeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = CombatConstants.NormalTimeScale;
        isStopping = false;
    }
}