using UnityEngine;

[System.Serializable]
public struct AttackDataStats
{
    public int Damage;
    public float AttackRange;
    public float AttackRadius;
    public string AnimationTrigger;
    public float CooldownBeforeNextAttack;
}