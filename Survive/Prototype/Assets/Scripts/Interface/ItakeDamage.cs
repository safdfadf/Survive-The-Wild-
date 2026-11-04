using UnityEngine;

public interface ItakeDamage
{
    public bool IsEnvironment{ get; set; }
    public virtual void TakeDamage(IAttack attack){}
}
