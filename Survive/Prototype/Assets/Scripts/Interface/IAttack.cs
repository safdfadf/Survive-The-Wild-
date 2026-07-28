using UnityEngine;

public interface IAttack
{
   
   public bool IsFire { get; }
   public bool IsPoison { get; }
   public bool IsStun { get; }
   public int Damage { get; }
}
