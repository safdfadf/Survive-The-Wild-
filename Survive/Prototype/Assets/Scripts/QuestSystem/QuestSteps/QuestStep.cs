using System;
using DefaultNamespace.EventBus;
using UnityEngine;

namespace DefaultNamespace.QuestSystem
{
    public class QuestStep : MonoBehaviour
    {
        [SerializeField] private string stepName;
        protected bool isfinished;
        protected string questId;
        protected QuestState questState;
        private QuestInfoSo _questInfo;

        public bool IsFinished => isfinished;
        public string StepName => stepName;
        
        public string QuestId => questId;

        [SerializeField] public string displayTask;
        // Quest Details : how do we show quest details: quest details should be shown 

        public void Initialize(QuestState questState,QuestInfoSo so)
        {
            _questInfo = so;
            this.questId = so.id;
            this.questState = questState;
        }

        protected void FinishQuest()
        {
            if (!isfinished)

                isfinished = true;
            Debug.Log("Quest " + questId + " finished");
            EventManager.Instance.questEvent.QuestComplete(questId);
            EventManager.Instance.playerEvents.AddExperience(_questInfo.ExperienceReward);
            Destroy(gameObject);
        }
    }
}