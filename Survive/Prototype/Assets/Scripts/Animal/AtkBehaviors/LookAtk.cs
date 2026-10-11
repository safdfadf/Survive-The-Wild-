using System.Collections;
using UnityEngine;

namespace DefaultNamespace.Animal.AtkBehaviors
{
    [CreateAssetMenu(fileName = "RamAttackBehavior", menuName = "Scriptable Objects/Attacks/LookAtkBehavior")]
    public class LookAtk : AnimalAtkBehaviour
    {
        public override IEnumerator Execute(AnimalBase animal)
        {
            return animal.LookAttack();
        }
    }
}