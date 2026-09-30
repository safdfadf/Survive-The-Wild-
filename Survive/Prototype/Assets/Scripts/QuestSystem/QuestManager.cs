using System;
using System.Collections.Generic;
using DefaultNamespace.EventBus;
using UnityEngine;

namespace DefaultNamespace.QuestSystem
{
    public class QuestManager : MonoBehaviour
    {
        [SerializeField] private List<QuestInfoSo> allQuests = new();
        private Dictionary<string, Mission> questsMap = new();
        private int _currentQuestIndex = 0;
        private Mission _currentMission;

        private void Awake()
        {
            CreateQuestMap();
            UpdateQuestState(1);
        }

        private void OnEnable()
        {
            EventManager.Instance.questEvent.OnQuestStepComplete += FinishQuest;
        }

        private void OnDisable()
        {
            EventManager.Instance.questEvent.OnQuestStepComplete -= FinishQuest;
        }

        private void CreateQuestMap()
        {
            foreach (var questInfo in allQuests)
            {
                Mission q = new Mission(questInfo);
                questsMap.Add(questInfo.id, q);
            }
        }

        private void StartNewQuest()
        {
            if (_currentQuestIndex + 1 > allQuests.Count) return;
            string id = allQuests[_currentQuestIndex].id;
            _currentMission = questsMap[id];
            if (_currentMission is not { questState: QuestState.CanStart } || !_currentMission.CanStartQuest())
            {
                return;
            }

            _currentQuestIndex++;
            _currentMission.SpawnQuest(transform);
        }

        private void FinishQuest(string id) // will be called by Quest Step 
        {
            Mission q = questsMap[id];
            if (q.IsNextQuestAvailable())
            {
                q.UpdateQuest();
            }
            else
            {
                EventManager.Instance.questEvent.MissionComplete(q.questInfo.RewardData);
                StartNewQuest();
            }
        }

        private void UpdateQuestState(int currentPlayerLevel) // called when player level increases 
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