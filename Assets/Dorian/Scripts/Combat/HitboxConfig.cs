using UnityEngine;

[CreateAssetMenu(fileName = "HitboxConfig", menuName = "Combat/Hitbox Config")]
public class HitboxConfig : ScriptableObject
{
    public Vector3 SmallHitboxScale;
    public Vector3 MediumArchHitboxScale;
    public Vector3 BigArchHitboxScale;
    public Vector3 SmallVerticalHitboxScale;
    public Vector3 DefaultHitboxScale;

    public Vector3 GetScale(HitboxType type)
    {
        switch (type)
        {
            case HitboxType.Small: return SmallHitboxScale;
            case HitboxType.MediumArch: return MediumArchHitboxScale;
            case HitboxType.BigArch: return BigArchHitboxScale;
            case HitboxType.SmallVertical: return SmallVerticalHitboxScale;
            default: return DefaultHitboxScale;
        }
    }
}