using System;
using System.Collections.Generic;
using DefaultNamespace.EventBus;
using UnityEngine;

namespace DefaultNamespace.QuestSystem
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private List<QuestInfoSo> allQuests = new();
        private Dictionary<string, Quest> questsMap = new();
        private int _currentQuestIndex = 0;
        private int _currentPlayerLevel = 0;

        private void Awake()
        {
            CreateQuestMap();
            UpdateQuestState(1);
        }

        private void OnEnable()
        {
            EventManager.Instance.questEvent.onQuestComplete += FinishQuest;
        }

        private void OnDisable()
        {
            EventManager.Instance.questEvent.onQuestComplete -= FinishQuest;
        }

        private void CreateQuestMap()
        {
            foreach (var questInfo in allQuests)
            {
                Quest q = new Quest(questInfo);
                questsMap.Add(questInfo.id, q);
            }
        }

        private void StartNewQuest() // who will call this function maybe Game manager 
        {
            Debug.Log(_currentQuestIndex);
            string id = allQuests[_currentQuestIndex].id;
            Quest quest = questsMap[id];
            if (quest is not { questState: QuestState.CanStart })
            {
                return;
            }

            _currentQuestIndex++;
            quest.SpawnQuest(this.transform);
        }

        private void FinishQuest(string id) // will be called by Quest Step 
        {
            //ClaimReward
            Quest q = questsMap[id];
            if (q.IsNextQuestAvailable())
            {
                q.SpawnQuest(this.transform);
            }
            else
            {
                StartNewQuest();
            }
        }

        // when player level is increased 
        public void UpdateQuestState(int currentPlayerLevel) // called when player level increases 
        {
            foreach (var q in questsMap.Values)
            {
                q.UpdateQuestInfo(currentPlayerLevel);
            }

            StartNewQuest();
        }
    }
}

public enum QuestState
{
    RequirementNotMet,
    CanStart,
    InProgress,
    CanFinish
}