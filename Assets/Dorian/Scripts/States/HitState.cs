using UnityEngine;

public class HitState : ICombatState
{
    #region Constants
    private const float CrossFadeDuration = 0.1f;
    private const float NormalizedEndTime = 0.95f;
    private const int BaseLayerIndex = 0;
    private const float MaxStateDuration = 1.0f;
    #endregion

    #region Private Fields
    private readonly CombatController controller;
    private readonly int hitAnimationHash = Animator.StringToHash("HitReaction");
    private float stateTimer;
    #endregion

    #region Initialization
    public HitState(CombatController controller)
    {
        this.controller = controller;
    }
    #endregion

    #region State Logic
    public void Enter()
    {
        stateTimer = 0f;
        controller.CombatInputBuffer.Clear();
        controller.CharacterAnimator.CrossFade(hitAnimationHash, CrossFadeDuration);
    }

    public void Execute()
    {
        stateTimer += Time.deltaTime;

        AnimatorStateInfo stateInfo = controller.CharacterAnimator.GetCurrentAnimatorStateInfo(BaseLayerIndex);

        bool animationFinished = stateInfo.shortNameHash == hitAnimationHash && stateInfo.normalizedTime >= NormalizedEndTime;
        bool failsafeTriggered = stateTimer >= MaxStateDuration;

        if (animationFinished || failsafeTriggered)
        {
            controller.ChangeState(new IdleState(controller));
        }
    }

    public void Exit()
    {
    }
    #endregion
}