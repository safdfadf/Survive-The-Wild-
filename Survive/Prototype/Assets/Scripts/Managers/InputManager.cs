using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private MovementHandler _player;
    private PlayerInventory _playerInventory;
    private PlayerUI _playerUI;
    private PlayerInputs _controls;
    private CraftingHandler _craftingHandler;
    private Camera _camera;
    private PhoneScript _phone;


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
    private Action<InputAction.CallbackContext> _aimCanceled;
    private Action<InputAction.CallbackContext> _shootPerformed;
    private Action<InputAction.CallbackContext> _cursorToggle;
    private Action<InputAction.CallbackContext> _inventoryToggle;
    private Action<InputAction.CallbackContext> _resourceMenuToggle;
    private Action<InputAction.CallbackContext> _toggleHuntetSenses;
    private Action<InputAction.CallbackContext> _toggleTarckMenu;
    private Action<InputAction.CallbackContext> _toggleRecipeBook;

    private void Awake()
    {
        _camera = Camera.main;
        _player = GetComponent<MovementHandler>();
        _playerInventory = GetComponent<PlayerInventory>();
        _craftingHandler = GetComponent<CraftingHandler>();
        _playerUI = GetComponent<PlayerUI>();
        _controls = new PlayerInputs();
        _phone = GetComponentInChildren<PhoneScript>();
        

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
        _toggleHuntetSenses = ctx => _player.ToggleHunterSense();
        // call player to eable inventory and then ask inventory to open recipe 
        //    _toggleRecipeBook = ctx => _playerUI.EnableRecipeBook();


        _aimStarted = ctx =>
        {
            var weapon = _player.CurrentWeapon;
            if (weapon != null && weapon.isAimable)
                weapon.StartAiming();
        };

        _aimCanceled = ctx =>
        {
            var weapon = _player.CurrentWeapon;
            if (weapon != null && weapon.isAimable)
                weapon.StopAiming();
        };
        _shootPerformed = ctx =>
        {
            var weapon = _player.CurrentWeapon;

            if (weapon != null)
            {
                EventBus.onAttack.Invoke();
            }
        };
        _cursorToggle = ctx => _player.ToggleCursor();
        _inventoryToggle = ctx => { _playerUI.ToggleInventory(); };
        _resourceMenuToggle = ctx => ToggleCollectableMenu();
        _toggleTarckMenu = ctx => ToggleTracksMenu();
    }

    private void OnEnable()
    {
        if (_controls != null)
        {
            _controls.Enable();
        }

        // subscribe using stored delegates
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

        _controls.PlayerInteract.Aim.started += _aimStarted;
        _controls.PlayerInteract.Aim.canceled += _aimCanceled;
        _controls.PlayerInteract.Shoot.performed += _shootPerformed;
        _controls.PlayerInteract.CursorOnOf.performed += _cursorToggle;
        _controls.PlayerInteract.Inventory.performed += _inventoryToggle;
        _controls.PlayerInteract.ResourceMenu.performed += _resourceMenuToggle;
        _controls.PlayerInteract.Interact.performed += _toggleTarckMenu;
        _controls.PlayerInteract.Phone.started += ctx => _phone.MoveInPhone();
        _controls.PlayerInteract.Phone.canceled += ctx => _phone.MoveOutPhone();
        _controls.PlayerInteract.Scroll.performed += _phone.Scroll;
        _controls.PlayerInteract.Book.performed += _toggleRecipeBook;
    }

    private void OnDisable()
    {
        _controls.PlayerMovement.Move.performed -= _movePerformed;
        _controls.PlayerMovement.Move.canceled -= _moveCanceled;

        _controls.PlayerMovement.Look.performed -= _lookPerformed;
        _controls.PlayerMovement.Look.canceled -= _lookCanceled;

        _controls.PlayerMovement.Sprint.performed -= _sprintPerformed;
        _controls.PlayerMovement.Sprint.canceled -= _sprintCanceled;

        _controls.PlayerMovement.Jump.performed -= _jumpPerformed;

        _controls.PlayerInteract.Aim.started -= _aimStarted;
        _controls.PlayerInteract.Aim.canceled -= _aimCanceled;
        _controls.PlayerInteract.Shoot.performed -= _shootPerformed;
        _controls.PlayerInteract.CursorOnOf.performed -= _cursorToggle;
        _controls.PlayerInteract.Inventory.performed -= _inventoryToggle;
        _controls.PlayerInteract.ResourceMenu.performed -= _resourceMenuToggle;
        _controls.PlayerMovement.HunerSense.performed -= _toggleHuntetSenses;
        _controls.PlayerInteract.Interact.performed -= _toggleTarckMenu;
        _controls.PlayerInteract.Book.performed -= _toggleRecipeBook;
        _controls.Disable();
    }

    private void ToggleCollectableMenu()
    {
        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        int mask = LayerMask.GetMask("Resources");
        if (Physics.SphereCast(ray, .5f, out RaycastHit hit, 3f, mask))
        {
            ICollectable collectable = hit.collider.GetComponent<ICollectable>();
            if (collectable != null)
            {
                collectable.ToggleMenu();
            }
        }
    }

    private void ToggleTracksMenu()
    {
        EventBus.OnToggleTracksMenu?.Invoke();
        _playerInventory.SubmitResource();
    }

    private void OnDestroy()
    {
        _controls.Disable();
    }
}