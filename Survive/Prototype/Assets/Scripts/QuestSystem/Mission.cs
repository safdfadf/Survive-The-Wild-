using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.QuestSystem
{
    public class Mission
    {
        public int currentQuestIndex;
        public QuestInfoSo questInfo;
        public QuestState questState;
        public List<QuestStep> _currentQuestSteps = new();

        public Mission(QuestInfoSo questInfo)
        {
            this.questInfo = questInfo;
            questState = QuestState.RequirementNotMet;
        }

        public void MoveToNextQuest()
        {
            currentQuestIndex++;
        }

        public int GetCurrentQuestIndex()
        {
            return currentQuestIndex;
        }

        public void SpawnQuest(Transform transform)
        {
            foreach (var q in questInfo.QuestSteps)
            {
                GameObject obj = Object.Instantiate(q, transform);
                QuestStep step = obj.GetComponent<QuestStep>();
                step.Initialize(questState, questInfo);
                step.gameObject.SetActive(false);
                _currentQuestSteps.Add(step);
            }

            Debug.Log(_currentQuestSteps.Count);
            RBookHandler uiHandler = Object.FindAnyObjectByType<RBookHandler>();
            uiHandler.SetQuestSteps(_currentQuestSteps);
            currentQuestIndex = 0;
            ActivateNextQuest();
        }

        public void UpdateQuest()
        {
            currentQuestIndex++;
            for (int i = 0; i < _currentQuestSteps.Count; i++)
            {
                if (_currentQuestSteps[i].gameObject == null)
                {
                    _currentQuestSteps.RemoveAt(i);
                }
            }

            UIManager.instance.AddQuestName(_currentQuestSteps);
            ActivateNextQuest();
        }

        private void ActivateNextQuest()
        {
            Debug.Log(_currentQuestSteps.Count);
            QuestStep currentSteps = _currentQuestSteps[currentQuestIndex];
            currentSteps.gameObject.SetActive(true);
        }

        public bool IsNextQuestAvailable()
        {
            return (currentQuestIndex >= questInfo.QuestSteps.Length);
        }

        public GameObject GetCurrentQuest()
        {
            GameObject questPrefab = questInfo.QuestSteps[GetCurrentQuestIndex()];
            return questPrefab.gameObject;
        }

        public void UpdateQuestInfo(int playerLevel)
        {
            if (playerLevel >= questInfo.PlayerLevelRequired)
            {
                questState = QuestState.CanStart;
            }
        }

        public bool CanStartQuest()
        {
            return currentQuestIndex < questInfo.QuestSteps.Length;
        }
    }
}