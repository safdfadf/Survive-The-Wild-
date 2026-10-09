using Player;
using UnityEngine;

namespace FoodSystem
{
    public class WaterObj : Food
    {
        [SerializeField] private Material waterMat;
        [SerializeField] private ParticleSystem steamParticles;
        private bool _isCooking = false;

        protected override void SetUiBools()
        {
            CanDisplay = false;
        }

        public override void ExecuteCooking(float deltaTime)
        {
            if (!canCookMe) return;

            if (!_isCooking)
            {
                steamParticles.Play();
                _isCooking = true;
            }

            cookTime -= deltaTime;

            if (cookTime <= 0)
            {
                FinishCooking();
            }
        }

        private void FinishCooking()
        {
            canCookMe = false;

            if (steamParticles.isPlaying)
                steamParticles.Stop();

            data.SelfAttack = null;
            data.SanityEffect = cookState.sanityEffect;
            currentState = cookState;
        }
    }
}

public enum WaterState
{
    Safe,
    UnSafe,
    Dirty
}

public class WaterData
{
    public WaterState waterState;
}