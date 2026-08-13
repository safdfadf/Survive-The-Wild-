using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "RamAttackBehavior", menuName = "Scriptable Objects/Attacks/RamAttackBehavior")]
public class RamAttackBehaviour : AnimalAtkBehaviour
{
    public override IEnumerator Execute(AnimalBase animal)
    {
        return animal.RamAttack();
    }
}
