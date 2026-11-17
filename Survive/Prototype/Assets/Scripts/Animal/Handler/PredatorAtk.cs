using System;
using Player;
using UnityEngine;

namespace SurviveTheWild.Animal.Handler
{
    // this will be handle by the who handles when predetor attack is happening 
    public class PredatorAtk : MonoBehaviour
    {
        private float predictionTime = 2f;
        private Vector3? candidatePosition;
        private Vector3 _lastValidationPos;
        private float validChecks;
        private float requiredValidChecks = 5;
        private Vector3 _lastPlayerPos;
        private AnimalSo animalSo;
        private bool canStart;

        private void OnEnable()
        {
            SceneRoot.Instance.EventManager.TimeEvents.onOneSecondPass += EvaluateAmbushLocation;
        }

        private void OnDisable()
        {
            SceneRoot.Instance.EventManager.TimeEvents.onOneSecondPass -= EvaluateAmbushLocation;
        }

        public void Initialize(AnimalSo so)
        {
            animalSo = so;
            _lastPlayerPos = PlayerRepository.instance.GetPlayerTransform().position;
            canStart = true;
        }

        private void EvaluateAmbushLocation() // this being called every second 
        {
            if (!canStart)
            {
                print("can start was false");
                return;
            }

            var predictedPos = PredictPlayerPosition();

            if (candidatePosition == null)
            {
                candidatePosition = ChunkManager.Instance.GetPos(predictedPos);

                if (candidatePosition != null)
                    validChecks = 1;
                print("candidate was null");
                return;
            }

            if (ValidateLocation(candidatePosition.Value))
            {
                validChecks++;
                print("valid");
                if (validChecks >= requiredValidChecks)
                {
                    SpawnPredator(candidatePosition.Value);
                }
            }
            else
            {
                print("not valid");
            }
        }

        private Vector3 PredictPlayerPosition()
        {
            Vector3 playerPos = PlayerRepository.instance.GetPlayerTransform().position;

            Vector3 velocity =
                playerPos - _lastPlayerPos;

            _lastPlayerPos = playerPos;

            return playerPos + velocity * predictionTime;
        }

        private Vector3? FindNearByMesh(Vector3 predictedPosition) // will be used later 
        {
            /*
            List<Vector3> bushes =
                VegetationManager.Instance.GetBushesNearPosition(
                    predictedPosition,
                    30f);

            if (bushes.Count == 0)
                return null;

            return bushes[
                Random.Range(0, bushes.Count)];
            */
            // instead of this ask chunk manager 
            // from chunk manager get position 
            return Vector3.zero;
        }

        private bool ValidateLocation(Vector3 bushPos)//
        {
            var playerPos = PlayerRepository.instance.GetPlayerTransform().position;
            var playerVelocity =
                (playerPos - _lastValidationPos).normalized;

            _lastValidationPos = playerPos;

            Vector3 dirToBush =
                (bushPos - playerPos).normalized;

            float dot =
                Vector3.Dot(playerVelocity, dirToBush);

            bool stillMovingTowardAmbush =
                dot > 0.35f;

            bool reasonableDistance =
                Vector3.Distance(
                    playerPos,
                    bushPos) < 50f;
            print(stillMovingTowardAmbush + "" + reasonableDistance + " " + bushPos);
            return
                stillMovingTowardAmbush &&
                reasonableDistance;
        }

        private void SpawnPredator(Vector3? position)
        {
            print("spawning predator");
           GameObject obj =  GameObject.CreatePrimitive(PrimitiveType.Capsule);
           obj.transform.position = position.Value;
        }
    }
}