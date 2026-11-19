using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "RamAttackBehavior", menuName = "Scriptable Objects/Attacks/RamAttackBehavior")]
public class RamAttackBehaviourSo : AnimalAtkBehaviourSo
{
    public override IEnumerator Execute(AnimalBase animal)
    {
    yield return null;    
    }
}
