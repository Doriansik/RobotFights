using UnityEngine;

public class BlockState : ICombatState
{
    #region Constants
    private const float CrossFadeDuration = 0.1f;
    private const int BaseLayerIndex = 0;
    #endregion

    #region Private Fields
    private readonly CombatController controller;
    private readonly int blockAnimationHash = Animator.StringToHash("Block");
    private int previousStateHash;
    #endregion

    #region Properties
    public float StartTime { get; private set; }
    #endregion

    #region Initialization
    public BlockState(CombatController controller)
    {
        this.controller = controller;
    }
    #endregion

    #region State Logic
    public void Enter()
    {
        StartTime = Time.time;
        previousStateHash = controller.CharacterAnimator.GetCurrentAnimatorStateInfo(BaseLayerIndex).shortNameHash;

        controller.CombatInputBuffer.Clear();
        controller.CharacterAnimator.CrossFade(blockAnimationHash, CrossFadeDuration);
    }

    public void Execute()
    {
        if (!controller.BlockAction.action.IsPressed())
        {
            controller.ChangeState(new IdleState(controller));
        }
    }

    public void Exit()
    {
        controller.CharacterAnimator.CrossFade(previousStateHash, CrossFadeDuration);
    }
    #endregion
}