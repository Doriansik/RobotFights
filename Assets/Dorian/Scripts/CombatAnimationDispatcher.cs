using System;
using UnityEngine;

public class CombatAnimationDispatcher : MonoBehaviour
{
    public event Action OnHitboxEnableRequested;
    public event Action OnHitboxDisableRequested;

    public void TriggerEnableHitboxes()
    {
        OnHitboxEnableRequested?.Invoke();
    }

    public void TriggerDisableHitboxes()
    {
        OnHitboxDisableRequested?.Invoke();
    }
}