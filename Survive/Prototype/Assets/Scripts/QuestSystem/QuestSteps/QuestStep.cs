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


        public bool IsFinished => isfinished;
        public string StepName => stepName;
        
        public string QuestId => questId;

        [SerializeField] public string displayTask;
        // Quest Details : how do we show quest details: quest details should be shown 

        public void Initialize(string questId, QuestState questState)
        {
            this.questId = questId;
            this.questState = questState;
        }

        protected void FinishQuest()
        {
            if (!isfinished)

                isfinished = true;
            Debug.Log("Quest " + questId + " finished");
            EventManager.Instance.questEvent.QuestComplete(questId);
            Destroy(gameObject);
        }
    }
}