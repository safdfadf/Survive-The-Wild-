using System;

namespace DefaultNamespace.EventBus
{
    public class QuestEvent
    {
        public Action<string> OnQuestStepComplete;
        public Action<string> OnQuestStepFailed;
        public Action<RewardData> OnMissionComplete;

        public void QuestComplete(string quest)
        {
            OnQuestStepComplete(quest);
        }

        public void MissionComplete(RewardData data)
        {
            OnMissionComplete(data);
        }
    }
}

[System.Serializable]
public class RewardData
{
   public int LevelExp;// used by player stats 
    public CraftingSO RewardRecipe;
}