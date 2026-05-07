using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackCoordinator : MonoBehaviour
{
    public static AttackCoordinator Instance { get; private set; }
    public static event Action<GameObject> OnTargetChanged;

    [SerializeField] private int maxSimultaneousAttackers = 1;

    private readonly List<RobotMovement> currentAttackers = new();

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
        if (currentAttackers.Contains(robot)) return true;

        if (currentAttackers.Count < maxSimultaneousAttackers)
        {
            currentAttackers.Add(robot);
            OnTargetChanged?.Invoke(robot.gameObject);
            return true;
        }
        return false;
    }

    public void ReleaseToken(RobotMovement robot)
    {
        if (currentAttackers.Contains(robot))
        {
            currentAttackers.Remove(robot);

            if (currentAttackers.Count > 0)
            {
                OnTargetChanged?.Invoke(currentAttackers[0].gameObject);
            }
            else
            {
                OnTargetChanged?.Invoke(null);
            }
        }
    }
}