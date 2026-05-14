using UnityEngine;

public class AttackState : ICombatState
{
    private readonly CombatController controller;
    private readonly AttackData attackData;
    private float stateStartTime;
    private bool comboTriggered;

    public AttackState(CombatController combatController, AttackData data)
    {
        controller = combatController;
        attackData = data;
    }

    public void Enter()
    {
        stateStartTime = Time.time;
        comboTriggered = false;
        controller.CharacterAnimator.SetTrigger(attackData.AnimationTriggerName);
        controller.HitboxExecutor.PrepareHitbox(attackData);
    }

    public void Execute()
    {
        float elapsedTime = Time.time - stateStartTime;
        float currentFrame = elapsedTime * CombatConstants.TargetFramerate;
        float totalFrames = attackData.WindUpFrames + attackData.ActiveFrames + attackData.RecoveryFrames;

        controller.HitboxExecutor.ProcessHitboxState(currentFrame, attackData);

        if (attackData.NextComboNode != null && !comboTriggered)
        {
            float comboStartFrame = (attackData.ComboWindowStartPercentage / CombatConstants.PercentageDivisor) * totalFrames;
            float comboEndFrame = (attackData.ComboWindowEndPercentage / CombatConstants.PercentageDivisor) * totalFrames;

            if (currentFrame >= comboStartFrame && currentFrame <= comboEndFrame)
            {
                if (controller.CombatInputBuffer.ConsumeInput(attackData.NextComboNode.RequiredInput))
                {
                    comboTriggered = true;
                }
            }
        }

        if (currentFrame >= totalFrames)
        {
            if (comboTriggered)
            {
                controller.ChangeState(new AttackState(controller, attackData.NextComboNode));
            }
            else
            {
                controller.MarkSequenceEnd();
                controller.ChangeState(new IdleState(controller));
            }
        }
    }

    public void Exit()
    {
        controller.HitboxExecutor.DeactivateHitbox();
    }
}