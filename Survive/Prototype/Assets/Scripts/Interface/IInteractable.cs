using DefaultNamespace.ResourceSystem;
using UnityEngine;

public interface IInteractable
{
    public bool OutlineMe { get; set; } // part of ui
    public bool CanBeCollected { get; set; }
    public GameObject Gm { get; set; }
    public bool isHit { get; set; }
    public Vector3 hitPos { get; set; }
    public virtual void ExecuteAction(){}

    public bool CanDisplay { get; set; }
    public string UseMeDescription { get; set; }
    public string Description { get; set; }
    public bool CanUse { get; set; }
    public bool CanHarvest { get; set; }
    public bool CanCraft { get; set; }
    public void Craft();
    public void Harvest();
    public void UseMe();
}