using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

public class MovementHandler : MonoBehaviour
{
    public float radius;
    public float maxDistance;
    [SerializeField] private ResourceInventory resourceInventory;
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float jumpForce = 5f;


    [SerializeField] private float mouseSensitivity = 100f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.3f;

    [SerializeField] private Transform LeftSpwnPoint;
    [SerializeField] private Transform rightSpwnPoint;

    [SerializeField] private float hourlyScentInc = 0.01f;
    [SerializeField] private float moveScentInc = 0.005f;

    [FormerlySerializedAs("animalApproachPoint")]
    public Transform animalApproachPos;

    [Header("Rotation Lock")]
    //f  private int 
    public bool isHuntingSenseActive { get; set; }

    [SerializeField] private Transform aimTarget;

    // References //
    public Camera playerCamera;
    private CharacterController characterController;
    private PlayerBow playerBow;
    private PlayerInventory _playerInventory;
    private PlayerAnimator animator;
    private PlayerUI _ui;
    private PlayerScentEmitter _playerScentEmitter;
    private PlayerNoiseEmitter _noiseEmitter;
    private PlayerVitalStats _playerVitalStats;


    private GameObject currentlyHighlighted;

    private PlayerInputs _controls;
    public Vector2 MoveInput { get; set; }
    public bool isMoving { get; set; }
    public Vector2 LookInput { get; set; }

    public bool _isSprinting { get; set; }
    public bool _isWalking { get; set; }
    public bool IsCrouching { get; set; }
    private bool _isJumping;
    private bool _isGrounded;
    private bool _canMove = true;

    private float _xRotation = 0f;
    private Vector3 _velocity;

    public bool isAttacking { get; set; }

    [SerializeField] private Material outlineMaterial;
    [SerializeField] private GameObject SpineController;
    private Material[] _originalMaterials;

    public BaseWeapon CurrentWeapon { get; private set; }
    private GameObject crosshair;
    private float scentTracker;
    private Coroutine noiseRoutine;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        characterController = GetComponent<CharacterController>();
        _playerInventory = GetComponent<PlayerInventory>();
        animator = GetComponent<PlayerAnimator>();
        _ui = GetComponent<PlayerUI>();
        _playerScentEmitter = GetComponent<PlayerScentEmitter>();
        _noiseEmitter = GetComponent<PlayerNoiseEmitter>();
        _playerVitalStats = GetComponent<PlayerVitalStats>();
        SetSpineControl(false);
    }

    public void SetSpineControl(bool isAiming)
    {
        if (!isAiming) return;
        float pitch = Camera.main.transform.localEulerAngles.x;
        if (pitch > 180) pitch -= 360;

        pitch = Mathf.Clamp(pitch, -30, 30);

        SpineController.transform.localRotation = Quaternion.Euler(0f, 0, pitch);
    }

    private void Start()
    {
        resourceInventory.gameObject.SetActive(false);
        crosshair = _ui.GetCrosshair();
        crosshair.SetActive(false);
    }

    private void Update()
    {
        MovePlayer();
        RotatePlayer();
        ExtraGravity();
        ShootRay(); // condition needed 
        CollectCheck();
    }

    private void OnEnable()
    {
        EventBus.OnHourChanged += AddScentHourly;
    }

    private void OnDisable()
    {
        EventBus.OnHourChanged -= AddScentHourly;
    }

    public void ToggleHunterSense()
    {
        if (!isHuntingSenseActive)
        {
            isHuntingSenseActive = true;
        }
        else
        {
            isHuntingSenseActive = false;
        }

        EventBus.OnHunterSenseToggle?.Invoke(isHuntingSenseActive);
        UIManager.instance.ToggleSoundUI(isHuntingSenseActive);
    }

    public void ToggleInventory() // currently we are toggling resource inventory i want to toggle the whole inventory 
    {
    }

    private void MovePlayer()
    {
        if (_canMove && isMoving)
        {
            Vector3 moveDirection = transform.right * MoveInput.x + transform.forward * MoveInput.y;

            animator.MovePlayer(MoveInput.y, MoveInput.x);
            float speed = 0;

            if (_isSprinting)
            {
                float dt = Time.deltaTime;
                _isWalking = false;
                _noiseEmitter.AddNoise(_noiseEmitter.sprintNoise, _isSprinting);
                speed = sprintSpeed;
                _playerVitalStats.DrainStamina(dt);
            }
            else if (IsCrouching)
            {
                _isWalking = false;
                speed = crouchSpeed;
            }
            else
            {
                _isWalking = true;
                speed = walkSpeed;
                _noiseEmitter.AddNoise(_noiseEmitter.walkNoise, !_isSprinting);
            }

            characterController.Move(moveDirection * (speed * Time.deltaTime));
            animator.TriggerSprint(_isSprinting);
        }
        else
        {
            animator.MovePlayer(0, 0);
        }
    }

    private void OnMoveStarted()
    {
        if (noiseRoutine == null)
            noiseRoutine = StartCoroutine(FeedBackLoop());
    }

    public void OnMoveStopped()
    {
        if (noiseRoutine != null)
        {
            StopCoroutine(noiseRoutine);
            noiseRoutine = null;
        }
    }

    private IEnumerator FeedBackLoop()
    {
        while (true)
        {
            float noiseAmount = 0;
            float scentAmount = moveScentInc;
            if (_isSprinting)
            {
                noiseAmount = _noiseEmitter.sprintNoise;
                scentAmount *= 3f;
            }
            else
            {
                noiseAmount = _noiseEmitter.walkNoise;
            }


            _playerScentEmitter.AddScent(scentAmount);
            yield return new WaitForSeconds(0.1f); // configurable interval
        }
    }

    private void RotatePlayer()
    {
        if (_canMove)
        {
            float mouseX = LookInput.x * mouseSensitivity * Time.deltaTime;
            float mouseY = LookInput.y * mouseSensitivity * Time.deltaTime;


            _xRotation -= mouseY;
            if (_xRotation >= 54)
            {
                _xRotation = 54;
            }
            else if (_xRotation <= -80)
            {
                _xRotation = -80;
            }

            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);
        }
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
        }
    }

    private void ExtraGravity()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += Physics.gravity.y * Time.deltaTime;
        characterController.Move(_velocity * Time.deltaTime);
    }

    public void ToggleCursor()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void ShootRay()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.SphereCast(ray, radius, out RaycastHit hit, maxDistance))
        {
            BaseStructure structure = hit.collider.GetComponent<BaseStructure>();
            if (structure != null && structure.isActiveAndEnabled) // ray hits structure which is in ghost mode 
            {
                ResourceSo so = GetRequiredResources(structure);
                _playerInventory.SetSubmitResource(so, structure);
                structure.ToggleDescription(true);
            }

            ICollectable collectable = hit.collider.GetComponent<ICollectable>();
            MeshRenderer renderer = hit.collider.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                renderer = hit.collider.GetComponentInChildren<MeshRenderer>();
            }

            if (collectable != null && renderer != null)
            {
                if (currentlyHighlighted != hit.collider.gameObject)
                {
                    ClearHighlight();
                    ApplyOutline(renderer, collectable);
                    currentlyHighlighted = hit.collider.gameObject;
                }

                return;
            }
        }

        ClearHighlight();
    }

    private void CollectCheck()
    {
        if (currentlyHighlighted != null && Input.GetKeyDown(KeyCode.E))
        {
            ICollectable collectable = currentlyHighlighted.GetComponent<ICollectable>();
            if (collectable != null && collectable.canBeCollected)
            {
                collectable.Collect(_playerInventory);
                ClearHighlight();
            }
        }
    }

    private void ApplyOutline(MeshRenderer renderer, ICollectable collectable)
    {
        _originalMaterials = renderer.materials;
        Material[] newMaterials = new Material[_originalMaterials.Length + 1];
        _originalMaterials.CopyTo(newMaterials, 0);
        newMaterials[newMaterials.Length - 1] = outlineMaterial;
        renderer.materials = newMaterials;
    }

    private void ClearHighlight()
    {
        if (currentlyHighlighted != null)
        {
            MeshRenderer renderer = currentlyHighlighted.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                renderer = currentlyHighlighted.GetComponentInChildren<MeshRenderer>();
            }

            if (renderer != null && _originalMaterials != null)
            {
                renderer.materials = _originalMaterials;
            }

            currentlyHighlighted = null;
            _originalMaterials = null;
        }
    }

    public ResourceSo GetRequiredResources(BaseStructure structure)
    {
        ResourceSo so = structure.GetNextRequiredResource();
        if (so != null)
            return so;
        return null;
    }

    public void TogglePlayerLock(bool isLocked)
    {
        _canMove = isLocked;
    }

    public void InitializeWeapon(BaseWeapon weapon)
    {
        weapon.IniTialize(this, _playerInventory, animator, aimTarget, rightSpwnPoint, crosshair);
    }

    public void EquipItem(BaseWeapon weapon)
    {
        CurrentWeapon = weapon;
        if (CurrentWeapon.isLeftHanded)
        {
            CurrentWeapon.transform.SetParent(LeftSpwnPoint);
        }
        else
        {
            CurrentWeapon.transform.SetParent(rightSpwnPoint);
        }

        CurrentWeapon.transform.localPosition = CurrentWeapon.RightHandAngle;
        CurrentWeapon.transform.localRotation = Quaternion.Euler(CurrentWeapon.RightHandRotAngle);
    }

    private void AddScentHourly(int hour)
    {
        _playerScentEmitter.AddScent(hourlyScentInc);
    }

    public Transform GetPlayerTransform()
    {
        return gameObject.transform;
    }

    public bool IsSprinting()
    {
        return _isSprinting;
    } //hunter
}