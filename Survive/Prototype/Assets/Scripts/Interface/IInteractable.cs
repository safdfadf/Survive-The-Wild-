using DefaultNamespace.ResourceSystem;
using UnityEngine;

public interface IInteractable
{
     public bool outlineMe{get;set;}// part of ui
     public  bool canBeCollected { get; set; }
     public GameObject Gm{get; set; }
     public bool isHit { get; set; }
     public Vector3 hitPos{get;set;}
}
