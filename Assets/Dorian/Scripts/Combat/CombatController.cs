using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(HitboxManager))]
public class CombatController : MonoBehaviour
{
    public AttackData NeutralLightStartNode;
    public AttackData NeutralHeavyStartNode;
    public float InputBufferDuration;
    public float AttackCooldown;

    public InputActionReference LightAttackAction;
    public InputActionReference HeavyAttackAction;

    public ICombatState CurrentState { get; private set; }
    public InputBuffer CombatInputBuffer { get; private set; }
    public Animator CharacterAnimator { get; private set; }
    public HitboxManager HitboxExecutor { get; private set; }
    public float LastAttackEndTime { get; private set; }

    private void Awake()
    {
        CombatInputBuffer = new InputBuffer(InputBufferDuration);
        CharacterAnimator = GetComponentInChildren<Animator>();
        HitboxExecutor = GetComponent<HitboxManager>();
        LastAttackEndTime = -AttackCooldown;
    }

    private void Start()
    {
        ChangeState(new IdleState(this));
    }

    private void OnEnable()
    {
        LightAttackAction?.action.Enable();
        HeavyAttackAction?.action.Enable();
    }

    private void OnDisable()
    {
        LightAttackAction?.action.Disable();
        HeavyAttackAction?.action.Disable();
    }

    private void Update()
    {
        if (LightAttackAction != null && LightAttackAction.action.WasPressedThisFrame())
        {
            CombatInputBuffer.RegisterInput(AttackType.Light);
        }

        if (HeavyAttackAction != null && HeavyAttackAction.action.WasPressedThisFrame())
        {
            CombatInputBuffer.RegisterInput(AttackType.Heavy);
        }

        CurrentState?.Execute();
    }

    public void ChangeState(ICombatState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
    }

    public void MarkSequenceEnd()
    {
        LastAttackEndTime = Time.time;
    }
}