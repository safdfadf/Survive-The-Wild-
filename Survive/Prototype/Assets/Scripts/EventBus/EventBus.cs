using System;
using UnityEngine;

public class EventBus // Todo Remove This Class 
{
    public static Action OnActivateChunk;

    //crafting system 
    public static Action<ObjSo, InventoryItem> OnCraftResource; // Invoker: BaseResource subscribed by Crafting Handler
    public static Action<ObjSo, InventoryItem> OnUnCraftResource;

    public static Action onAttack;

    // Animal 
    public static Action<RegionType, Bounds> CreateAnimalData; // Invoker : ChunkManager  Subscribers: Animal Handler


    public static Action OnRndmTimePassed; // Invoker : Time Manager Subscriber : Wind System
    public static Action On5SecondsPassed;

    public static Action<Vector3, float> OnWindChanged; // Invoker: Wind System  Subscriber : AnimalStateManager

    // Gpu Instancing
    public static Action<Chunk> OnGpuActivateInChunk;
    public static Action<Chunk> OnGpuDeactivateInChunk;

    //Hunter Sense
    public static Action<bool> OnHunterSenseToggle;

    // Toggle Tracks Menu
    public static Action PressingE;

    // Send Resource For Building 
    public static Action<ObjSo> OnResourceSubmit; //Invoker : inventory system // Subscriber : Base Structure  
}