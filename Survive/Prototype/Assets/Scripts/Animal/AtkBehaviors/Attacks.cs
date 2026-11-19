using UnityEngine;

namespace SurviveTheWild.Animal.AtkBehaviors
{
    public abstract class Attacks : MonoBehaviour
    {
        [SerializeField] protected AnimalAttack attack;
        protected AnimalBase animal;

        public void Initialize(AnimalBase animal)
        {
            this.animal = animal;
        }

        public abstract void Execute();

    }
}