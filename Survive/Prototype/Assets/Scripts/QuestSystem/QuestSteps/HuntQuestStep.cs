using System;
using System.Collections.Generic;
using DefaultNamespace.EventBus;
using DefaultNamespace.QuestSystem;
using UnityEngine;

namespace QuestSystem.QuestSteps
{
    public class HuntQuestStep : QuestStep
    {
        [SerializeField] private List<RequiredAnimal> requiredData;

        [SerializeField] private bool trackAnimalState;

        [SerializeField] private int extraLevelExp;

        // an extra condition is need track animal state 
        private void Awake()
        {
            SetQuestName();
        }

        private void OnEnable()
        {
            EventManager.Instance.AnimalEvents.onAnimalDeath += CheckSubmission;
        }

        private void OnDisable()
        {
            EventManager.Instance.AnimalEvents.onAnimalDeath -= CheckSubmission;
        }

        private void CheckSubmission(AnimalBase animal)
        {
            AnimalSo so = animal.AnimalSo;
            for (int i = requiredData.Count - 1; i >= 0; i--)
            {
                {
                    if (requiredData[i].so != so) continue;
                    requiredData[i].amount--;
                    if (requiredData[i].amount == 0)
                    {
                        requiredData.Remove(requiredData[i]);
                    }
                }
            }

            CheckToFinish();
        }

        private void CheckToFinish()
        {
            if (requiredData.Count != 0) return;
            FinishQuest();
        }

        private void SetQuestName()
        {
            List<string> names = new();
            foreach (var data in requiredData)
            {
                string n = data.so.id + " x " +
                           "" + data.amount;
                names.Add(n);
            }

            stepName = string.Join(", ", names.ToArray());
        }
    }
}

[System.Serializable]
public class RequiredAnimal
{
    public AnimalSo so;
    public int amount;
}