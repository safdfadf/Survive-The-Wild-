using System;
using DefaultNamespace.EventBus;
using UnityEngine;

namespace DefaultNamespace.QuestSystem
{
    public class QuestStep : MonoBehaviour
    {
        protected bool isfinished;
        protected string questId;
        protected QuestState questState;

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
            // maybe fire an event that quest is finished 
        }
    }
}