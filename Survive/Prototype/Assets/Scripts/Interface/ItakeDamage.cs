using UnityEngine;

public interface ItakeDamage
{
    public bool IsEnvironment{ get; set; }
    public void TakeDamage(IAttack attack);
}
