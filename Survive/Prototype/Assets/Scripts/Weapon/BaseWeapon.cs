using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Weapon;
using Player;
using UnityEngine;
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

    public WeaponAbility Ability => ability;
    protected override void Awake()
    {
        base.Awake();
        useMeDescription = "EquipMe";
        canUse = true;
        playerInventory = GetComponentInParent<PlayerInventory>();
        behaviours = GetComponentsInChildren<WeaponBehaviour>();
        _activeBehaviour = behaviours[0]; // fix this 
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
            b.Initialize(data, animator, this);
        }

        behaviours[0].isEquipped = true;
        _activeBehaviour.OnEquip();
        crosshair.SetActive(false);
    }


    public override void UseMe()
    {
        player.EquipItem(this);
        PlayerRepository.instance.RemoveWeapon(So as WeaponSo);
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
}


public class WeaponData
{
    public Transform aimTarget;
    [HideInInspector] public WeaponSo weaponSo;
}