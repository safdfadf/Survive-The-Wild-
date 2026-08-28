using DefaultNamespace.ResourceSystem;
using UnityEngine;

public interface ICollectable// ToDo: replace with more suitable name
{
     public bool outlineMe{get;set;}// part of ui
     public  bool canBeCollected { get; set; }
     public GameObject Gm{get; set; }
}
