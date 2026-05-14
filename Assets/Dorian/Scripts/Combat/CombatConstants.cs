public enum AttackType
{
    None,
    Light,
    Heavy
}

public enum HitboxType
{
    Small,
    MediumArch,
    BigArch,
    SmallVertical
}

public static class CombatConstants
{
    public const string LightAttackInput = "LightAttack";
    public const string HeavyAttackInput = "HeavyAttack";
    public const float TargetFramerate = 60f;
    public const float PercentageDivisor = 100f;
    public const float DefaultTimeValue = 0f;
    public const float NormalTimeScale = 1f;
    public const float StoppedTimeScale = 0f;
}