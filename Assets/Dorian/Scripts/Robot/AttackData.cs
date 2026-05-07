using UnityEngine;

[System.Serializable]
public struct AttackData
{
    public int Damage;
    public float AttackRange;
    public float AttackRadius;
    public string AnimationTrigger;
    public float CooldownBeforeNextAttack;
}