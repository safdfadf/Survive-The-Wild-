using System.Collections;
using Player;
using UnityEngine;

namespace SurviveTheWild.Animal.AtkBehaviors
{
    public class PredatorAtk : Attacks
    {
        public override void Execute()
        {
            StartCoroutine(AtkOn());
        }

        private IEnumerator AtkOn()
        {
            bool onArrived = false;
            while (!onArrived)
            {
                var pos = PlayerRepository.instance.GetApproachPos().position;
                animal.MoveTo(pos, () => onArrived = true,null,true);
                if (onArrived)
                    break;
                yield return null;
            }

            var atkOn = true;
            while (atkOn)
            {
                if (animal.IsLowHealth() && Random.value < 0.5f)
                {
                    animal.GetOutOfChunk();
                    yield break;
                }

                animal.DoDamage(attack);
                yield return new WaitForSeconds(2f);
            }
        }
    }
}