using Player;
using UnityEngine;

namespace SurviveTheWild.Animal
{
    public class Predator : UnScheduleAnimal
    {
        public override void AlertBehavior()
        {
            print("moving towards player");
            Vector3 playerPos = PlayerRepository.instance.GetApproachPos().position;
            MoveTo(playerPos,null,alertSpeed);
        }
    }
}