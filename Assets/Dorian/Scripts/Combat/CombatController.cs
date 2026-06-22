using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(HitboxManager), typeof(PlayerEnergy), typeof(PlayerHealth))]
public class CombatController : MonoBehaviour
{
    #region Configuration
    [SerializeField] private AttackData neutralLightStartNode;
    [SerializeField] private AttackData neutralHeavyStartNode;
    [SerializeField] private float inputBufferDuration;
    [SerializeField] private float attackCooldown;
    #endregion

    #region Input References
    [SerializeField] private InputActionReference lightAttackAction;
    [SerializeField] private InputActionReference heavyAttackAction;
    #endregion

    #region Properties
    public event Action OnDamageTaken;

    public AttackData NeutralLightStartNode => neutralLightStartNode;
    public AttackData NeutralHeavyStartNode => neutralHeavyStartNode;
    public float AttackCooldown => attackCooldown;
    public ICombatState CurrentState { get; private set; }
    public InputBuffer CombatInputBuffer { get; private set; }
    public Animator CharacterAnimator { get; private set; }
    public HitboxManager HitboxExecutor { get; private set; }
    public PlayerEnergy EnergyManager { get; private set; }
    public float LastAttackEndTime { get; private set; }
    #endregion

    #region Private Fields
    private PlayerHealth playerHealth;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        CombatInputBuffer = new InputBuffer(inputBufferDuration);
        CharacterAnimator = GetComponentInChildren<Animator>();
        HitboxExecutor = GetComponent<HitboxManager>();
        EnergyManager = GetComponent<PlayerEnergy>();
        playerHealth = GetComponent<PlayerHealth>();
        LastAttackEndTime = -attackCooldown;
    }

    private void Start()
    {
        ChangeState(new IdleState(this));
    }

    private void OnEnable()
    {
        lightAttackAction?.action.Enable();
        heavyAttackAction?.action.Enable();

        if (playerHealth != null)
        {
            playerHealth.OnDamageTaken += HandleDamageTaken;
        }
    }

    private void OnDisable()
    {
        lightAttackAction?.action.Disable();
        heavyAttackAction?.action.Disable();

        if (playerHealth != null)
        {
            playerHealth.OnDamageTaken -= HandleDamageTaken;
        }
    }

    private void Update()
    {
        HandleInput();
        CurrentState?.Execute();
    }
    #endregion

    #region State Management & Logic
    private void HandleInput()
    {
        if (lightAttackAction != null && lightAttackAction.action.WasPressedThisFrame())
        {
            CombatInputBuffer.RegisterInput(AttackType.Light);
        }

        if (heavyAttackAction != null && heavyAttackAction.action.WasPressedThisFrame())
        {
            CombatInputBuffer.RegisterInput(AttackType.Heavy);
        }
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

    private void HandleDamageTaken()
    {
        ChangeState(new HitState(this));
    }
    #endregion
}