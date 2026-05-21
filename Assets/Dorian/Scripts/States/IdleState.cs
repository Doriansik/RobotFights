using UnityEngine;

public class IdleState : ICombatState
{
    private readonly CombatController controller;

    public IdleState(CombatController combatController)
    {
        controller = combatController;
    }

    public void Enter()
    {
        controller.CombatInputBuffer.Clear();
    }

    public void Execute()
    {
        if (Time.time - controller.LastAttackEndTime < controller.AttackCooldown) return;

        if (controller.CombatInputBuffer.ConsumeInput(AttackType.Light))
        {
            if (controller.EnergyManager.HasEnoughEnergy(controller.NeutralLightStartNode.Energy))
            {
                controller.ChangeState(new AttackState(controller, controller.NeutralLightStartNode));
            }
        }
        else if (controller.CombatInputBuffer.ConsumeInput(AttackType.Heavy))
        {
            if (controller.EnergyManager.HasEnoughEnergy(controller.NeutralHeavyStartNode.Energy))
            {
                controller.ChangeState(new AttackState(controller, controller.NeutralHeavyStartNode));
            }
        }
    }

    public void Exit()
    {
    }
}