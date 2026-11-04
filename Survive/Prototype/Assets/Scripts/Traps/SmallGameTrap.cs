using System.Collections.Generic;
using SurviveTheWild.Helpers;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace SurviveTheWild.Traps
{
    public class SmallGameTrap : BaseStructure
    {
        [FormerlySerializedAs("AnimalS")] [SerializeField]
        private List<AnimalSo> animalSos = new();

        [SerializeField] private Transform spawnPos;
        private float choosenHour;
        private float currentHour = 0;
        private bool canSpawn = true;
        private AnimalBase selectedAnimal;

        private void OnEnable()
        {
            SceneRoot.Instance.EventManager.TimeEvents.onOneHourPass += UpdateCurrentHour;
            SceneRoot.Instance.EventManager.ChunkEvents.ActiveChunk += CheckPosition;
        }

        private void OnDisable()
        {
            SceneRoot.Instance.EventManager.TimeEvents.onOneHourPass -= UpdateCurrentHour;
            SceneRoot.Instance.EventManager.ChunkEvents.ActiveChunk -= CheckPosition;
        }

        protected override void Awake()
        {
            base.Awake();
            choosenHour = Random.Range(0f, 24f);
            spawnPos = transform;
        }

        private void UpdateCurrentHour(int hour)
        {
            if (currentHour > 24)
            {
                currentHour = 0;
            }

            currentHour++;

            if (currentHour >= choosenHour)
            {
                canSpawn = true;
            }
        }

        private void CheckPosition(Chunk chunk)
        {
            if (ChunkHelper.IsInChunkXZ(transform.position, chunk.bounds))
                SpawnAnimal();
        }

        private void SpawnAnimal()
        {
            if (!canSpawn) return;
            var count = Random.Range(0, animalSos.Count);
            var obj = GlobalPool.instance.Get(animalSos[count].prefab, spawnPos.position);
            selectedAnimal = obj.GetComponent<AnimalBase>();
            selectedAnimal.DeadAnimal();
            // after spawning animate trap to it original pos 
            CanDisplay = true;
            Description = "To Reset";
        }


        public override void ExecuteAction()
        {
            choosenHour = Random.Range(0f, 24f);
            CanDisplay = false;
        }
    }
}