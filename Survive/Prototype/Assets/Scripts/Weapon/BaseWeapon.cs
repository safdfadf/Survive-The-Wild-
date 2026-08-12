using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class BaseWeapon : Obj<ResourceSo>
{
    [SerializeField] protected Transform arrowRestPoint;

    protected CraftingSO CraftingSo;
    protected PlayerInputs playerInputs;
    protected MovementHandler player;
    protected PlayerInventory playerInventory;
    protected Transform aimTarget;
    [Header("Right Hand")] protected Transform RightHandPos;
    public Vector3 RightHandRotAngle;
    public Vector3 RightHandAngle;
    public float inventoryRotAngle;

    public bool isLeftHanded { get; protected set; }
    private GameObject crosshair;
    protected PlayerAnimator animator;

    public int MaxDamage { get; private set; }
    public bool isAimable { get; protected set; }
    public bool isAiming; // will be shared by spear
    protected float drawTime;
    protected float drawPercent;
    protected float drawOffset;
    [SerializeField] protected float maxDrawTime = 1f;

    private Transform cameraTransform;
    private Vector3 crossHairPoint;
    
    private void OnEnable()
    {
        EventBus.onAttack += TryAttack;
    }

    private void OnDisable()
    {
        EventBus.onAttack -= TryAttack;
    }

    protected override void Awake()
    {
        base.Awake();
        canUseButton = true;
        playerInventory = GetComponentInParent<PlayerInventory>();
        rb = null; // ToDo : remove this 
    }

    public void SetCraftingSo(CraftingSO weaponSo)
    {
        CraftingSo = weaponSo;
    }

    protected virtual void Update()
    {
        if (isAiming)
        {
            UpdateAimTarget();
            UpdateCrosshair();
        }
    }

    public void IniTialize(MovementHandler movementHandler, PlayerInventory inventory, PlayerAnimator animator,
        Transform aimTarget, Transform rightHandDrawPoint, GameObject Crosshair) // who will initialize this 
    {
        this.player = movementHandler;
        this.playerInventory = inventory;
        this.animator = animator;
        this.aimTarget = aimTarget;

        this.RightHandPos = rightHandDrawPoint;
        this.cameraTransform = Camera.main.transform;
        this.crosshair = Crosshair;

        crosshair.SetActive(false);
    }

    public virtual void StartAiming()
    {
        if (!isAimable) return;

        drawTime = 0;
        crosshair.SetActive(true);
        isAiming = true;
        animator.Aim(isAiming);
    }

    public virtual void StopAiming()
    {
        if (!isAimable) return;
        isAiming = false;
        animator.Aim(false);
        drawTime = 0f;
        crosshair.SetActive(false);
    }

    private void UpdateAimTarget()
    {
        Vector3 camPos = cameraTransform.position;
        Vector3 camForward = cameraTransform.forward;
        Vector3 crosshairWorldPoint = camPos + camForward * 100f;


        // horizontal offset based on bow position
        float aimOffsetX = Vector3.Dot(
            arrowRestPoint.position - camPos,
            cameraTransform.right
        );

        // shift ray origin sideways
        Vector3 offsetOrigin = camPos + cameraTransform.right * aimOffsetX;

        // target point must be in front of the *offset origin*, not the camera
        Vector3 targetPoint = offsetOrigin + camForward * 100f;

        Vector3 rayDirection = (crosshairWorldPoint - offsetOrigin).normalized;

        // ray direction should remain camera forward
        Ray ray = new Ray(offsetOrigin, rayDirection);

        crossHairPoint = targetPoint;
        int mask = ~LayerMask.GetMask("Resources");

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, mask))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * 100f;
        }


        aimTarget.position = targetPoint;
    }

    private void UpdateCrosshair()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(crossHairPoint);
        crosshair.transform.position = screenPos;
    }

    protected virtual void Shoot()
    {
    }

    protected virtual void Attack()
    {
    }

    protected virtual void Block(int damage)
    {
    }
    public override void UseMe()
    {
        player.EquipItem(this);
    }
    private void TryAttack()
    {
        if (isAimable)
        {
            Shoot();
        }
        else
        {
            Attack();
        }
    }

    private void AttackAnimStart()
    {
        player.isAttacking = true;
    }

    private void AttackAnimStop()
    {
        player.isAttacking = false;
    }

    protected IEnumerator StartAttacking()
    {
        AttackAnimStart();
        yield return new WaitForSeconds(1f);
        AttackAnimStop();
    }
}