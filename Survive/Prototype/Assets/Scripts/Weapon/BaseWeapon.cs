using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Weapon;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class BaseWeapon : Obj<ObjSo>
{
    [SerializeField] private WeaponAbility ability;
    private WeaponBehaviour[] behaviours;
    [SerializeField] private WeaponData data = new();
    public WeaponBehaviour _activeBehaviour { get; private set; }
    protected CraftingSO CraftingSo;

    protected MovementHandler player;
    protected PlayerInventory playerInventory;

    [Header("Right Hand")] protected Transform RightHandPos;
    public Vector3 RightHandRotAngle;
    public Vector3 RightHandAngle;
    public float inventoryRotAngle;

    public bool isLeftHanded;
    public GameObject crosshair { get; set; }
    public PlayerAnimator animator { get; private set; }

    private Vector3 crossHairPoint;

    public bool isEquipped { get; set; }
    public WeaponAbility Ability => ability;
    public bool RestrictUse { get; set; }

    // what if weapon uses both behaviours 
    private ProjectileParent _projectileAttack;
    private WeaponBehaviour _meleeAtk;

    protected override void Awake()
    {
        base.Awake();
        useMeDescription = "EquipMe";
        canUse = true;
        playerInventory = GetComponentInParent<PlayerInventory>();
        behaviours = GetComponentsInChildren<WeaponBehaviour>();

        _projectileAttack = HasRangeAttack();
        Debug.Log(_projectileAttack);
        _meleeAtk = GetMeleeAtk();
        _activeBehaviour = _meleeAtk == null ? _projectileAttack : _meleeAtk;
    }

    public void SetCraftingSo(CraftingSO weaponSo)
    {
        CraftingSo = weaponSo;
    }

    public void IniTialize(MovementHandler movementHandler, PlayerInventory inventory, PlayerAnimator animator,
        Transform aimTarget, Transform rightHandDrawPoint, GameObject Crosshair)
    {
        this.player = movementHandler;
        this.playerInventory = inventory;
        this.animator = animator;
        this.RightHandPos = rightHandDrawPoint;
        this.crosshair = Crosshair;
        data.aimTarget = aimTarget;
        data.weaponSo = So as WeaponSo;
        foreach (var b in behaviours)
        {
            b.Initialize(data, animator, this, player);
        }

        crosshair.SetActive(false);
    }


    public override void UseMe()
    {
        isEquipped = true;
        _activeBehaviour.OnEquip();
        player.EquipItem(this);
        //   PlayerRepository.instance.RemoveWeapon(So as WeaponSo);
    }

    public void UnEquipMe()
    {
        isEquipped = false;
        _activeBehaviour.OnUnEquip();
    }

    public void DeliverDamage()
    {
        _activeBehaviour.DeliverDamage();
    }

    protected override void SetUiBools()
    {
        canCraft = true;
        canHarvest = false;
        canUse = true;
    }

    public void SwitchWeaponBehavior(InputAction.CallbackContext ctx)
    {
        if (behaviours.Length == 1) // if there is only one behaviour 
        {
            _activeBehaviour.OnInput(ctx);
            return;
        }
        if (ctx.interaction is TapInteraction  && ctx.phase == InputActionPhase.Performed)
        {
            Debug.Log("Tap → Melee");
            _activeBehaviour = _meleeAtk;
            _activeBehaviour.OnInput(ctx);
            return;
        }
        if (ctx.interaction is HoldInteraction && ctx.phase == InputActionPhase.Performed)
        {
            Debug.Log("Hold Start → Projectile");
            _activeBehaviour = _projectileAttack;
            _activeBehaviour.OnInput(ctx);
            return;
        }

        // HOLD RELEASE (Canceled)
        if (ctx.interaction is HoldInteraction && ctx.phase == InputActionPhase.Canceled)
        {
            Debug.Log("Hold Release → Projectile");
            _activeBehaviour = _projectileAttack;
            _activeBehaviour.OnInput(ctx);
            return;
        }

        // TAP (Click)
    }

    private ProjectileParent HasRangeAttack()
    {
        foreach (var b in behaviours)
        {
            if (b.TryGetComponent<ProjectileParent>(out var range)) return range;
        }

        return null;
    }

    private WeaponBehaviour GetMeleeAtk()
    {
        foreach (var w in behaviours)
        {
            if (w.TryGetComponent<SwingAttack>(out var melee))
            {
                if (melee != null)
                    return melee;
            }

            if (w.TryGetComponent<ThrustAttack>(out var thrust))
            {
                if (thrust != null)
                    return thrust;
            }
        }

        return null;
    }
}


public class WeaponData
{
    public Transform aimTarget;
    [HideInInspector] public WeaponSo weaponSo;
}