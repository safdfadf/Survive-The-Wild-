using System.Collections;
using UnityEngine;

namespace SurviveTheWild.Animal.AtkBehaviors
{
    public class LookAtk : Attacks
    {
        public override void Execute()
        {
            LookAttack();
        }

        private void LookAttack()
        {
            print("doing look attack damage ");
            animal.DoDamage(attack);
        }
    }
}