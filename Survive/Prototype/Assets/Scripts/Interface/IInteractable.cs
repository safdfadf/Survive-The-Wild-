using DefaultNamespace.ResourceSystem;
using UnityEngine;

public interface IInteractable
{
     public bool outlineMe{get;set;}// part of ui
     public  bool canBeCollected { get; set; }
     public GameObject Gm{get; set; }
     public bool isHit { get; set; }
     public Vector3 hitPos{get;set;}
     public void ExecuteAction();
     
     public bool canDisplay { get; set; }
     public string useMeDescription{get;set;}
     public string Description{get;set;}
     public bool canUse{get;set;}
     public bool canHarvest{get;set;}
     public bool canCraft{get;set;}
     public void Craft();
     public void Harvest();
     public void UseMe();
     
}
