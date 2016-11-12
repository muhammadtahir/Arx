﻿using ArxGame.Components.Combat;
using CommonInterfaces.Controllers;
using CommonInterfaces.Weapons;
using Extensions;
using GenericComponents.Controllers.Characters;
using GenericComponents.Enums;
using GenericComponents.Interfaces.States.PlatformerCharacter;
using GenericComponents.StateMachine;
using GenericComponents.StateMachine.States.PlatformerCharacter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(CombatModule))]
public class MainPlatformerController : PlatformerCharacterController, IPlatformerCharacterController
{
    private CombatModule _combatModule;
    private StateManager<IPlatformerCharacterController, PlatformerCharacterAction> _stateManager;

    [SerializeField]
    private float _rollingDuration = 1;

    private float _move;
    private float _vertical;
    private bool _jump;

    private AttackType _attackAction;

    public StateManager<IPlatformerCharacterController, PlatformerCharacterAction> StateManager
    {
        get
        {
            return _stateManager;
        }
    }

    public IWeapon Weapon
    {
        get
        {
            return _combatModule.Weapon;
        }
        set
        {
            _combatModule.Weapon = value;
        }
    }

    public bool IsAttackOver { get; private set; }

    public bool Attacking { get; private set; }

    public int ComboNumber
    {
        get
        {
            return _combatModule.ComboNumber;
        }
    }

    public WeaponType? WeaponType
    {
        get
        {
            return _combatModule.WeaponType;
        }
    }

    public float RollingDuration
    {
        get
        {
            return _rollingDuration;
        }
    }

    public bool IsCharging { get; private set; }

    public float AimAngle { get; set; }

    public void Move(float move, float vertical, bool jump)
    {
        _move = move;
        _vertical = vertical;
        _jump = jump;
    }

    public void LightAttack()
    {
        _attackAction = AttackType.Primary;
    }

    public void StrongAttack()
    {
        _attackAction = AttackType.Secundary;
    }

    public void ChargeAttack()
    {
        IsCharging = true;
        _attackAction = AttackType.Primary;
    }

    public void ReleaseChargeAttack()
    {
        IsCharging = false;
        _attackAction = AttackType.None;
    }

    public void DoPrimaryGroundAttack()
    {
        Attacking = _combatModule.PrimaryGroundAttack();
        IsAttackOver = !Attacking;
        _attackAction = AttackType.None;
    }

    public void DoSecundaryGroundAttack()
    {
        Attacking = _combatModule.SecundaryGroundAttack();
        IsAttackOver = !Attacking;
        _attackAction = AttackType.None;
    }

    public void DoPrimaryAirAttack()
    {
        Attacking = _combatModule.PrimaryAirAttack();
        IsAttackOver = !Attacking;
        _attackAction = AttackType.None;
    }

    public void DoSecundaryAirAttack()
    {
        Attacking = _combatModule.SecundaryAirAttack();
        IsAttackOver = !Attacking;
        _attackAction = AttackType.None;
    }

    public void DoChargeAttack()
    {
        Attacking = _combatModule.ChargeAttack();
        IsAttackOver = !Attacking;
        _attackAction = AttackType.None;
    }

    public void DoReleaseChargeAttack()
    {
        _combatModule.ReleaseChargeAttack();
    }

    public override void Duck()
    {
        _combatModule.ComboNumber = 0;
        Attacking = false;
        base.Duck();
    }

    public override void Stand()
    {
        _combatModule.ComboNumber = 0;
        Attacking = false;
        base.Stand();
    }

    public void StartIddle() { }
    public void StopIddle() { }

    public void FlipToSlideDownDirection()
    {
        if(CharacterController2D.SlopeNormal.x == 0)
        {
            return;
        }
        Flip(
            CharacterController2D.SlopeNormal.x > 0 
            ? CommonInterfaces.Enums.Direction.Right 
            : CommonInterfaces.Enums.Direction.Left);
    }

    public void AirSlash()
    {
        VelocityMultiplier = new Vector3(VelocityMultiplier.x, VelocityMultiplier.y * 4f);
        _combatModule.StartDiveAttack();
    }

    public void StopAirSlash()
    {
        VelocityMultiplier = Vector2.one;
        _combatModule.EndDiveAttack();
        OnAttackFinishHandler();
    }

    protected override void Awake()
    {
        base.Awake();
        _combatModule = GetComponent<CombatModule>();
        _stateManager = new PlatformerCharacterStateManager(this, _rollingDuration);
        _combatModule.OnAttackFinish += OnAttackFinishHandler;
    }

    protected override void Update()
    {
        base.Update();
        _combatModule.AimAngle = AimAngle;
        var action = new PlatformerCharacterAction(_move, _vertical, _jump, _attackAction);
        _stateManager.Perform(action);
        _move = 0;
        _vertical = 0;
        _jump = false;
    }

    private void OnAttackFinishHandler()
    {
        IsAttackOver = true;
    }
}
