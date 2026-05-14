using UnityEngine;

public class InputBuffer
{
    private AttackType currentInput;
    private float inputTimestamp;
    private readonly float bufferDuration;

    public InputBuffer(float duration)
    {
        bufferDuration = duration;
        inputTimestamp = CombatConstants.DefaultTimeValue;
    }

    public void RegisterInput(AttackType input)
    {
        currentInput = input;
        inputTimestamp = Time.time;
    }

    public bool ConsumeInput(AttackType expectedInput)
    {
        float timeSinceInput = Time.time - inputTimestamp;
        if (currentInput == expectedInput && timeSinceInput <= bufferDuration)
        {
            Clear();
            return true;
        }
        return false;
    }

    public void Clear()
    {
        currentInput = AttackType.None;
        inputTimestamp = CombatConstants.DefaultTimeValue;
    }
}