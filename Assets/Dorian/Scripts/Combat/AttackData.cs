using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackData", menuName = "Combat/Attack Data")]
public class AttackData : ScriptableObject
{
    public AttackType RequiredInput;
    public float Damage;
    public float Energy;
    public float FalterPoints;
    public int WindUpFrames;
    public int ActiveFrames;
    public int RecoveryFrames;
    public float ComboWindowStartPercentage;
    public float ComboWindowEndPercentage;
    public AttackData NextComboNode;
    public string AnimationTriggerName;
    public HitboxType HitboxSize;
}