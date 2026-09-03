using UnityEngine;

namespace DefaultNamespace.QuestSystem
{
    public class Quest
    {
        public int currentQuestIndex;
        public QuestInfoSo questInfo;
        public QuestState questState; // do i need quest state ?

        public Quest(QuestInfoSo questInfo)
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
            if (questState != QuestState.CanStart)
            {
                return;
            }

            GameObject obj = Object.Instantiate(questInfo.QuestSteps[currentQuestIndex], transform);
            QuestStep step = obj.GetComponent<QuestStep>();
            step.Initialize(questInfo.id, questState);
            currentQuestIndex++;
        }

        public bool IsNextQuestAvailable()
        {
            return (currentQuestIndex >= questInfo.QuestSteps.Length);
        }

        public GameObject GetCurrentQuest()
        {
            GameObject questPrefab = questInfo.QuestSteps[GetCurrentQuestIndex()];
            return questPrefab;
        }

        public void UpdateQuestInfo(int playerLevel)
        {
            if (playerLevel >= questInfo.PlayerLevelRequired)
            {
                questState = QuestState.CanStart;
            }
        }

        public bool CanStartQuest(int playerLevel)
        {
            if (playerLevel >= questInfo.PlayerLevelRequired)
            {
                return true;
            }

            return false;
        }
    }
}