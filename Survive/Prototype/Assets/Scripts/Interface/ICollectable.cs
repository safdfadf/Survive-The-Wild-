using DefaultNamespace.ResourceSystem;
using UnityEngine;

public interface ICollectable
{
     public ResourceUI resourceUI { get; }
     public bool outlineMe{get;set;}
     public  bool canBeCollected { get; set; }
     public GameObject Gm{get;}
}
