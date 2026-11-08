using System;
using System.Collections.Generic;
using SurviveTheWild.EventBus;
using SurviveTheWild.Helpers;
using SurviveTheWild.Interface;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public GraphicRaycaster graphicRaycaster;
    private MovementHandler _player;
    private PlayerInventory _playerInventory;
    private PlayerUI _playerUI;
    private PlayerInputs _controls;
    private CraftingHandler _craftingHandler;
    private Camera _camera;
    private WatchUI _phone;
    private PlayerAnimator _playerAnimator;
    private float scrollCooldown = 0.2f; // adjust to taste
    private float lastScrollTime = 0f;
    private bool toggleInventory = false;
    private Action<InputAction.CallbackContext> _movePerformed;
    private Action<InputAction.CallbackContext> _moveCanceled;
    private Action<InputAction.CallbackContext> _lookPerformed;
    private Action<InputAction.CallbackContext> _lookCanceled;
    private Action<InputAction.CallbackContext> _sprintPerformed;
    private Action<InputAction.CallbackContext> _sprintCanceled;
    private Action<InputAction.CallbackContext> _crouchPerformed;
    private Action<InputAction.CallbackContext> _crouchCanceled;
    private Action<InputAction.CallbackContext> _jumpPerformed;
    private Action<InputAction.CallbackContext> _aimStarted;
    private Action<InputAction.CallbackContext> _rightHoldCancel;
    private Action<InputAction.CallbackContext> _shootPerformed;
    private Action<InputAction.CallbackContext> _cursorToggle;
    private Action<InputAction.CallbackContext> _inventoryToggle;
    private Action<InputAction.CallbackContext> _resourceMenuToggle;
    private Action<InputAction.CallbackContext> _toggleHuntetSenses;
    private Action<InputAction.CallbackContext> _toggleTarckMenu;
    private Action<InputAction.CallbackContext> _toggleRecipeBook;
    private IRequestInput _requestInput;

    private void Awake()
    {
        _camera = Camera.main;
        _player = GetComponent<MovementHandler>();
        _playerInventory = GetComponent<PlayerInventory>();
        _craftingHandler = GetComponent<CraftingHandler>();
        _playerUI = GetComponent<PlayerUI>();
        _controls = new PlayerInputs();
        _phone = GetComponentInChildren<WatchUI>();
        _playerAnimator = GetComponent<PlayerAnimator>();

        // Initialize all delegates
        _movePerformed = ctx =>
        {
            _player.MoveInput = ctx.ReadValue<Vector2>();
            _player.isMoving = true;
        };
        _moveCanceled = ctx =>
        {
            _player.MoveInput = Vector2.zero;
            _player.isMoving = false;
            _player._isWalking = false;
            _player.OnMoveStopped();
        };

        _lookPerformed = ctx => _player.LookInput = ctx.ReadValue<Vector2>();
        _lookCanceled = ctx => _player.LookInput = Vector2.zero;

        _sprintPerformed = ctx => _player._isSprinting = true;
        _sprintCanceled = ctx => _player._isSprinting = false;

        _crouchPerformed = ctx => _player.IsCrouching = true;
        _crouchCanceled = ctx => _player.IsCrouching = false;

        _jumpPerformed = ctx => _player.Jump();
        _toggleHuntetSenses = ctx =>
        {
            _player.ToggleHunterSense(); // maybe use a different input
            _player.DropAll();
        };


        _rightHoldCancel = ctx => { UIManager.instance.DeactivateSubMenu(); };
        _shootPerformed = ctx => // 
        {
            var weapon = _player.CurrentWeapon;

            if (weapon != null)
            {
                EventBus.onAttack.Invoke();
            }
        };
        _inventoryToggle = ctx => { ToggleInventory(); };
        _resourceMenuToggle = ctx => ToggleCollectableMenu();
        _toggleTarckMenu = ctx => PressE();
    }

    private void OnEnable()
    {
        if (_controls != null)
        {
            _controls.Enable();
        }

        if (SceneRoot.Instance.EventManager == null)
        {
            print("scene root is null");
        }

        SceneRoot.Instance.EventManager.playerEvents.onDeliverRequestInput += SetRequestedInput;
        SceneRoot.Instance.EventManager.playerEvents.onRemoveRequestInput += ResetRequestedInput;

        _controls.PlayerMovement.Move.performed += _movePerformed;
        _controls.PlayerMovement.Move.canceled += _moveCanceled;

        _controls.PlayerMovement.Look.performed += _lookPerformed;
        _controls.PlayerMovement.Look.canceled += _lookCanceled;

        _controls.PlayerMovement.Sprint.performed += _sprintPerformed;
        _controls.PlayerMovement.Sprint.canceled += _sprintCanceled;
        _controls.PlayerMovement.Crouch.performed += _crouchPerformed;
        _controls.PlayerMovement.Crouch.canceled += _crouchCanceled;
        _controls.PlayerMovement.Jump.performed += _jumpPerformed;
        _controls.PlayerMovement.HunerSense.performed += _toggleHuntetSenses;

        _controls.PlayerInteract.Right.started += HandleRightClick;
        _controls.PlayerInteract.Right.canceled += _rightHoldCancel;
        _controls.PlayerInteract.Left.performed += HandleLeftClick;
        _controls.PlayerInteract.Left.canceled += HandleLeftClick;

        //    _controls.PlayerInteract.CursorOnOf.performed += _cursorToggle;
        _controls.PlayerInteract.Inventory.performed += ctx => ToggleInventory();
        _controls.PlayerInteract.ResourceMenu.performed += _resourceMenuToggle;
        _controls.PlayerInteract.Interact.performed += _toggleTarckMenu;
        _controls.PlayerInteract.Phone.started += ctx => ShowWatch(true);
        _controls.PlayerInteract.Phone.canceled += ctx => ShowWatch(false);
        _controls.PlayerInteract.Scroll.performed += Scroll;
        _controls.PlayerInteract.Book.performed += _toggleRecipeBook;
        _controls.PlayerInteract.Escape.performed += ctx => RemoveRequestedInput();
    }

    private void OnDisable()
    {
        SceneRoot.Instance.EventManager.playerEvents.onDeliverRequestInput -= SetRequestedInput;
        SceneRoot.Instance.EventManager.playerEvents.onRemoveRequestInput -= ResetRequestedInput;

        _controls.PlayerMovement.Move.performed -= _movePerformed;
        _controls.PlayerMovement.Move.canceled -= _moveCanceled;

        _controls.PlayerMovement.Look.performed -= _lookPerformed;
        _controls.PlayerMovement.Look.canceled -= _lookCanceled;

        _controls.PlayerMovement.Sprint.performed -= _sprintPerformed;
        _controls.PlayerMovement.Sprint.canceled -= _sprintCanceled;

        _controls.PlayerMovement.Jump.performed -= _jumpPerformed;

        _controls.PlayerInteract.Right.started -= HandleRightClick;
        _controls.PlayerInteract.Right.canceled -= _rightHoldCancel;
        _controls.PlayerInteract.Left.performed -= HandleLeftClick;
        _controls.PlayerInteract.Left.canceled -= HandleLeftClick;
        _controls.PlayerInteract.Scroll.canceled -= Scroll;
        //   _controls.PlayerInteract.CursorOnOf.performed -= _cursorToggle;
        //   _controls.PlayerInteract.Inventory.performed -= ToggleInventory();
        _controls.PlayerInteract.ResourceMenu.performed -= _resourceMenuToggle;
        _controls.PlayerMovement.HunerSense.performed -= _toggleHuntetSenses;
        _controls.PlayerInteract.Interact.performed -= _toggleTarckMenu;
        _controls.PlayerInteract.Book.performed -= _toggleRecipeBook;

        _controls.Disable();
    }

    private void ToggleCollectableMenu()
    {
        var eventData = new PointerEventData(EventSystem.current);
        eventData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();

        graphicRaycaster.Raycast(eventData, results);

        var slot = results[0].gameObject.GetComponent<Slot>();
        if (slot == null)
        {
            print("slot is null");
            return;
        }

        var item = slot.occupiedItem[0];
        if (item != null)
        {
            item.Toggle();
        }
        else
        {
            print("ocusdada");
        }

/*
        foreach (var result in results)
        {
            var slot = result.gameObject.GetComponent<Slot>();
            if (slot == null)
            {
                print(result.gameObject.name);
                return;
            }

            var item = slot.occupiedItem;
            if (item != null)
            {
                item.Toggle();
                return;
            }
        }
        */
    }

    public void SetRequestedInput(IRequestInput input)
    {
        _requestInput = input;
    }

    public void ResetRequestedInput()
    {
        _requestInput = null;
    }

    public void RemoveRequestedInput()
    {
        _requestInput.ResetInput();
        _requestInput = null;
    }

    private void HandleLeftClick(InputAction.CallbackContext ctx)
    {
        if (_requestInput != null)
        {
            _requestInput.UseInput(ctx);
            return;
        }


        var weapon = _player.CurrentWeapon;
        if (weapon == null) return;
        weapon.HandleWeapon(ctx);
    }

    private void HandleRightClick(InputAction.CallbackContext ctx)
    {
        if (_requestInput != null)
        {
            _requestInput.UseInput(ctx);
            return;
        }

        UIManager.instance.ActivateSubMenu();
    }

    private void PressE()
    {
        EventBus.PressingE?.Invoke();
    }


    private void Scroll(InputAction.CallbackContext ctx)
    {
        if (_phone.IsPhoneActive)
        {
            _phone.Scroll(ctx);
            return;
        }

        if (Time.time < lastScrollTime + scrollCooldown)
            return;

        lastScrollTime = Time.time;

        var movement = ctx.ReadValue<Vector2>();
        var scrollUp = movement.y > 0;

        _player.SwitchWeapon(scrollUp);
    }

    private void ToggleInventory()
    {
        toggleInventory = !toggleInventory;
        SceneRoot.Instance.EventManager.UiEvents.ToggleInventory(toggleInventory);
    }

    private void ShowWatch(bool show)
    {
        _phone.IsPhoneActive = show;

        _playerAnimator.ShowWatch(show);
    }

    private void OnDestroy()
    {
        _controls.Disable();
    }
}