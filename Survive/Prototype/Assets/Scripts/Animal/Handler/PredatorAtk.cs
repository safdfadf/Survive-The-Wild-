using System;
using System.Collections.Generic;
using Player;
using UnityEngine;

namespace SurviveTheWild.Animal.Handler
{
    // this will be handle by the who handles when predetor attack is happening 
    public class PredatorAtk : MonoBehaviour
    {
        [SerializeField] private float predictionTime = 3f;
        [SerializeField] private float ambushRadius = 30f;
        [SerializeField] private int requiredValidChecks = 5;

        private readonly List<Vector3> ambushPositions = new();

        private Vector3 lastPlayerPos;
        private Vector3 lastValidationPos;

        private int validChecks;

        private Vector3 currentBestPosition;

        private bool hasBestPosition;
        private bool canStart;

        private AnimalSo animalSo;

        private static readonly float[] SearchAngles =
        {
            -90f,
            -60f,
            -30f,
            0f,
            30f,
            60f,
            90f
        };

        private void OnEnable()
        {
            SceneRoot.Instance.EventManager.TimeEvents.onOneSecondPass +=
                EvaluateAmbushLocation;
        }

        private void OnDisable()
        {
            SceneRoot.Instance.EventManager.TimeEvents.onOneSecondPass -=
                EvaluateAmbushLocation;
        }

        public void Initialize(AnimalSo so)
        {
            animalSo = so;

            var playerPos =
                PlayerRepository.instance.GetPlayerTransform().position;

            lastPlayerPos = playerPos;
            lastValidationPos = playerPos;

            canStart = true;

            GenerateAmbushPositions();
        }

        private void EvaluateAmbushLocation()
        {
            if (!canStart)
                return;
            print("evaluating ambush position");
            if (ambushPositions.Count == 0)
            {
                GenerateAmbushPositions();

                if (ambushPositions.Count == 0)
                {
                    print("No ambush positions found");
                    return;
                }
            }

            var bestPosition = GetBestAmbushPosition();

            if (bestPosition == Vector3.zero)
            {
                print("best position is zerp");
                return;
            }

            if (!hasBestPosition)
            {
                currentBestPosition = bestPosition;
                hasBestPosition = true;
                validChecks = 1;
                print("returning");
                return;
            }

            bool sameCandidate =
                Vector3.Distance(
                    currentBestPosition,
                    bestPosition) < 5f;

            if (sameCandidate)
            {
                print("valid check ");
                validChecks++;
            }
            else
            {
                currentBestPosition = bestPosition;
                validChecks = 1;
                print("new valid check");
            }

            if (validChecks >= requiredValidChecks)
            {
                SpawnPredator(currentBestPosition);

                canStart = false;
            }
        }

        private void GenerateAmbushPositions()
        {
            ambushPositions.Clear();

            var playerPos =
                PlayerRepository.instance.GetPlayerTransform().position;

            var predictedPos =
                PredictPlayerPosition(); // if player is not moving in that case this will be quite close to the player 
            var movementDir =
                (predictedPos - playerPos).normalized;

            if (movementDir == Vector3.zero)
                movementDir = PlayerRepository.instance
                    .GetPlayerTransform()
                    .forward;

            foreach (float angle in SearchAngles)
            {
                Vector3 dir =
                    Quaternion.Euler(0, angle, 0) *
                    movementDir;

                Vector3 searchPos =
                    predictedPos +
                    dir * ambushRadius;

                Vector3? coverPos =
                    ChunkManager.Instance.GetPos(searchPos);
                //   GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //  obj.transform.position = coverPos.Value;

                //  print(searchPos+" " +coverPos.Value);
                if (coverPos.HasValue)
                {
                    ambushPositions.Add(
                        coverPos.Value);
                }
            }
        }

        private Vector3 PredictPlayerPosition()
        {
            Vector3 playerPos =
                PlayerRepository.instance.GetPlayerTransform().position;

            Vector3 velocity =
                playerPos - lastPlayerPos;

            lastPlayerPos = playerPos;

            return playerPos + velocity * predictionTime;
        }

        private Vector3 GetBestAmbushPosition()
        {
            var playerPos =
                PlayerRepository.instance.GetPlayerTransform().position;

            var velocity =
                playerPos - lastValidationPos;

            lastValidationPos = playerPos;

            if (velocity.sqrMagnitude < 0.01f)
            {
                print("returning zero");
                return Vector3.zero;
            }

            velocity.Normalize();

            Vector3 bestPosition = Vector3.zero;
            float bestScore = float.MinValue;

            foreach (var position in ambushPositions)
            {
                float distance =
                    Vector3.Distance(
                        position,
                        playerPos);

                if (distance > 60f)
                    continue;

                var dirToPosition =
                    (position - playerPos).normalized;

                float score =
                    Vector3.Dot(
                        velocity,
                        dirToPosition);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestPosition = position;
                }
            }

            print(bestPosition);
            return bestPosition;
        }

        private void SpawnPredator(Vector3 position)
        {
            Debug.Log(
                $"Spawning Predator at {position}");

            GameObject obj =
                GameObject.CreatePrimitive(
                    PrimitiveType.Capsule);

            obj.transform.position = position;
        }
    }
}

public class AmbushPoint
{
    public Vector3 Position { get; set; }
    public int Score { get; set; }
}