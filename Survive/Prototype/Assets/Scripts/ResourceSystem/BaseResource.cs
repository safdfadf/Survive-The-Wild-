using System;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public enum ResourceType
{
    Wood,
    Stone,
    Leaf,
    Metal,
    Log,
    LongWood,
    Stick,
    Rope
}

public class BaseResource : Resource<ResourceSo>
{
    private float timeCount;


    protected override void Awake()
    {
        Gm = gameObject;
        base.Awake();
    }

    protected override void CraftMe()
    {
        EventBus.OnResourceAdd.Invoke(this);
    }

    protected override void RemoveMeCraftingList()
    {
        _isInCraftingList = false;
        EventBus.OnResourceRemove.Invoke(this);
    }

    protected override void RemoveFromInventory()
    {
        if (_isInCraftingList) return;
        PlayerRepository.instance.RemoveResourceFromInventory(So, gameObject);
    }

    private void HarvestMe()
    {
    }
}