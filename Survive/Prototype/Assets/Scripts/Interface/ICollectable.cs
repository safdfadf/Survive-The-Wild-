using UnityEngine;

public interface ICollectable
{
     public  bool canBeCollected { get; set; }
     public GameObject Gm{get;}
     public void ToggleMenu();
}
