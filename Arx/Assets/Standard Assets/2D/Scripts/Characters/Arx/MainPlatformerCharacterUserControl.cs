﻿using ArxGame.Components;
using ArxGame.UI;
using CommonInterfaces.Controllers;
using CommonInterfaces.Inventory;
using InventorySystem;
using InventorySystem.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using CommonInterfaces.Controllers.Interaction;
using MathHelper;
using MathHelper.Extensions;
using CommonInterfaces.Weapons;
using Assets.Standard_Assets.QuestSystem.QuestStructures;
using Assets.Standard_Assets.QuestSystem;

[RequireComponent(typeof(MainPlatformerController))]
[RequireComponent(typeof(ItemFinderController))]
[RequireComponent(typeof(InventoryComponent))]
[RequireComponent(typeof(QuestLogComponent))]
[RequireComponent(typeof(UiController))]
[RequireComponent(typeof(EquipmentController))]
public class MainPlatformerCharacterUserControl : MonoBehaviour, IQuestSubscriber, IItemOwner, IPlayerControl
{
    private enum InputAction
    {
        None,
        AttackButtonDown,
        ChargingAttack
    }

    private const float MinAttackChargeTime = 0.2f;

    private bool _grabRopePressed;
    private InputAction _currentInputAction = InputAction.None;
    private float _attackButtonDownTime;

    private MainPlatformerController _characterController;
    private ItemFinderController _itemFinderController;
    private InventoryComponent _inventoryComponent;
    private QuestLogComponent _questLogComponent;
    private UiController _uiController;
    private EquipmentController _equipmentController;
    private HudManager _hud;
    private Vector3 _interactionPosition;
    private IInteractionTriggerController _currentInteraction;

    [SerializeField]
    private InteractionFinder interactionFinder;
    [SerializeField]
    private GameObject HudPrefab;
    [SerializeField]
    private float _stopInteractionDistance = 3f;
    [SerializeField]
    private GameObject _aimingArm;

    public event OnInventoryAdd OnInventoryItemAdd;
    public event OnInventoryRemove OnInventoryItemRemove;
    public event OnKill OnKill;

    public MainPlatformerController PlatformerCharacterController
    {
        get
        {
            return _characterController;
        }
    }

    public void AssignQuest(Quest quest)
    {
        _questLogComponent.GiveQuest(quest);
    }

    public Quest GetQuest(string id)
    {
        return _questLogComponent.GetQuest(id);
    }

    public bool HasQuestActive(Quest quest)
    {
        return _questLogComponent.HasQuestActive(quest);
    }

    private void Awake()
    {
        _characterController = GetComponent<MainPlatformerController>();
        _itemFinderController = GetComponent<ItemFinderController>();
        _inventoryComponent = GetComponent<InventoryComponent>();
        _questLogComponent = GetComponent<QuestLogComponent>();
        _equipmentController = GetComponent<EquipmentController>();
        _uiController = GetComponent<UiController>();
    }

    private void Start()
    {
        _itemFinderController.OnInventoryItemFound += OnInventoryItemFoundHandler;
        _hud = Instantiate(HudPrefab).GetComponent<HudManager>();
        PlatformerCharacterController.CloseCombatWeapon = _equipmentController.EquippedCloseCombatWeapon;
        PlatformerCharacterController.ShooterWeapon = _equipmentController.EquippedShooterWeapon;
        PlatformerCharacterController.ChainThrowWeapon = _equipmentController.EquippedChainThrowWeapon;
    }

    private void Update()
    {
        var inputDevice = InputManager.GetInputDevice();

        var horizontal = inputDevice.GetAxis(DeviceAxis.Movement).x;
        var vertical = inputDevice.GetAxis(DeviceAxis.Movement).y;
        var roll = inputDevice.GetButton(DeviceButton.Jump);
        var jump = inputDevice.GetButtonDown(DeviceButton.Jump);
        var releaseRope = inputDevice.GetButtonDown(DeviceButton.Interact);
        var aiming = inputDevice.GetButton(DeviceButton.AimWeapon);
        var teleport = inputDevice.GetButtonDown(DeviceButton.Vertical);

        if (teleport)
        {
            var teleporter = FindTeleporter();
            if(teleporter != null)
            {
                teleporter.Teleport(this.gameObject);
            }
        }

        SetAimAngle(inputDevice);
        HandleInteraction();

        _characterController.Move(horizontal, vertical, jump, roll, releaseRope, aiming);

        HandleAttack(inputDevice);
        SwitchActiveWeapon(inputDevice);
    }

    private void LateUpdate()
    {
        if (!Input.GetButtonDown("InGameMenu"))
        {
            return;
        }
        _uiController.Toggle();
    }

    private void HandleInteraction()
    {
        var distanceFromInteractionPoint = Vector3.Distance(_interactionPosition, transform.position);
        if (distanceFromInteractionPoint > _stopInteractionDistance && _currentInteraction != null)
        {
            _currentInteraction.StopInteraction();
            _currentInteraction = null;
            return;
        }

        if (!Input.GetButtonDown("Interact"))
        {
            return;
        }
        if (_currentInteraction == null)
        {
            _currentInteraction = interactionFinder.GetInteractionTrigger();
            _interactionPosition = transform.position;
        }

        if (_currentInteraction != null)
        {
            _currentInteraction.Interact(this.gameObject);
        }
    }

    private void OnInventoryItemFoundHandler(IInventoryItem item)
    {
        _inventoryComponent.Inventory.AddItem(item);
        _hud.Toast("Item found: " + item.Name, _hud.Short);
        if(OnInventoryItemAdd != null)
        {
            OnInventoryItemAdd.Invoke(item);
        }
    }

    private ITeleporter FindTeleporter()
    {
        var collider = 
            Physics2D
                .OverlapPointAll(this.transform.position)
                .FirstOrDefault(c => c.GetComponent<ITeleporter>() != null);

        if(collider == null)
        {
            return null;
        }

        return collider.GetComponent<ITeleporter>();
    }

    private void HandleAttack(IInputDevice inputDevice)
    {
        if(_characterController.WeaponType == null)
        {
            return;
        }

        switch (_currentInputAction)
        {
            case InputAction.None: NoneAttackState(inputDevice); break;
            case InputAction.AttackButtonDown: AttackButtonDownState(inputDevice); break;
            case InputAction.ChargingAttack: ChargingAttackState(inputDevice); break;
        }
    }

    private void SwitchActiveWeapon(IInputDevice inputDevice)
    {
        if (_characterController.Attacking)
        {
            return;
        }
        var setWeapon1 = inputDevice.GetButtonDown(DeviceButton.SetWeaponSocket1);
        var setWeapon2 = inputDevice.GetButtonDown(DeviceButton.SetWeaponSocket2);

        if (setWeapon1)
        {
            _equipmentController.ActiveCloseCombatSocket = WeaponSocket.ClosedCombarWeapon1;
            PlatformerCharacterController.CloseCombatWeapon = _equipmentController.EquippedCloseCombatWeapon;
        }
        else if (setWeapon2)
        {
            _equipmentController.ActiveCloseCombatSocket = WeaponSocket.ClosedCombatWeapon2;
            PlatformerCharacterController.CloseCombatWeapon = _equipmentController.EquippedCloseCombatWeapon;
        }
    }

    private void NoneAttackState(IInputDevice inputDevice)
    {
        var aiming = inputDevice.GetButton(DeviceButton.AimWeapon);

        if (aiming)
        {
            var shoot = inputDevice.GetButtonDown(DeviceButton.ShootWeapon);
            var @throw = inputDevice.GetButtonDown(DeviceButton.Throw);
            if (shoot)
            {
                _characterController.Shoot();
            }
            else if (@throw)
            {
                _characterController.Throw();
            }
            return;
        }

        var primary = inputDevice.GetButtonDown(DeviceButton.PrimaryAttack);
        var secundary = inputDevice.GetButtonDown(DeviceButton.SecundaryAttack);

        if (primary)
        {
            _attackButtonDownTime = 0;
            _currentInputAction = InputAction.AttackButtonDown;
        }

        if (secundary)
        {
            _characterController.StrongAttack();
        }
    }

    private void AttackButtonDownState(IInputDevice inputDevice)
    {
        _attackButtonDownTime += Time.deltaTime;
        if(_attackButtonDownTime > MinAttackChargeTime)
        {
            _characterController.ChargeAttack();
            _currentInputAction = InputAction.ChargingAttack;
            return;
        }

        var primary = inputDevice.GetButtonUp(DeviceButton.PrimaryAttack);
        if (primary)
        {
            _characterController.LightAttack();
            _currentInputAction = InputAction.None;
        }
    }

    private void ChargingAttackState(IInputDevice inputDevice)
    {
        var primary = inputDevice.GetButtonUp(DeviceButton.PrimaryAttack);

        if (primary)
        {
            _characterController.ReleaseChargeAttack();
            _currentInputAction = InputAction.None;
        }
    }

    private void SetAimAngle(IInputDevice inputDevice)
    {
        if(inputDevice.MouseSupport)
        {
            var center = _aimingArm.transform.position;
            var aimPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var degrees = FloatUtils.AngleBetween(center, aimPosition).ReduceToSingleTurn();
            _characterController.AimAngle = degrees;
        }
        else
        {
            var x = inputDevice.GetAxis(DeviceAxis.AimAnalog).x;
            var y = inputDevice.GetAxis(DeviceAxis.AimAnalog).y;
            var angle = Vector2.Angle(Vector2.right, new Vector2(x, y));
            if(y < 0)
            {
                angle = 360 - angle;
            }
            _characterController.AimAngle = angle;
        }
    }
}

