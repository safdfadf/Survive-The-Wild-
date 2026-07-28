using UnityEngine;

public class AnimalAttack: IAttack
{
    public bool IsFire { get; }
    public bool IsPoison { get; }
    public bool IsStun { get; }
    public int Damage { get; }

    public AnimalAttack(int damage, bool isPoison, bool isStun, bool isFire)
    {
        IsFire = isFire;
        IsPoison = isPoison;
        IsStun = isStun;
        Damage = damage;
    }

}
