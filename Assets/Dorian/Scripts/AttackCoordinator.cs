using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackCoordinator : MonoBehaviour
{
    public static AttackCoordinator Instance { get; private set; }
    public static event Action<GameObject> OnTargetChanged;

    [SerializeField] private int maxSimultaneousAttackers = 1;

    private readonly List<RobotMovement> activeAttackers = new();
    private readonly List<RobotMovement> waitingAttackers = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool TryGetAttackToken(RobotMovement robot)
    {
        if (activeAttackers.Contains(robot)) return true;

        if (!waitingAttackers.Contains(robot))
        {
            waitingAttackers.Add(robot);
        }

        while (waitingAttackers.Count > 0 && waitingAttackers[0] == null)
        {
            waitingAttackers.RemoveAt(0);
        }

        if (activeAttackers.Count < maxSimultaneousAttackers && waitingAttackers.Count > 0 && waitingAttackers[0] == robot)
        {
            waitingAttackers.RemoveAt(0);
            activeAttackers.Add(robot);
            NotifyTargetChanged();
            return true;
        }

        return false;
    }

    public void ReleaseToken(RobotMovement robot)
    {
        if (activeAttackers.Remove(robot))
        {
            PromoteWaitingAttackers();
            NotifyTargetChanged();
        }
        else
        {
            waitingAttackers.Remove(robot);
        }
    }

    private void PromoteWaitingAttackers()
    {
        waitingAttackers.RemoveAll(r => r == null);

        while (activeAttackers.Count < maxSimultaneousAttackers && waitingAttackers.Count > 0)
        {
            RobotMovement nextAttacker = waitingAttackers[0];
            waitingAttackers.RemoveAt(0);
            activeAttackers.Add(nextAttacker);
        }
    }

    private void NotifyTargetChanged()
    {
        activeAttackers.RemoveAll(r => r == null);

        if (activeAttackers.Count > 0)
        {
            OnTargetChanged?.Invoke(activeAttackers[0].gameObject);
        }
        else
        {
            OnTargetChanged?.Invoke(null);
        }
    }
}