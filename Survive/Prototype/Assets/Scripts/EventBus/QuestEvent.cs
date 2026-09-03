using System;

namespace DefaultNamespace.EventBus
{
    public class QuestEvent
    {
     public Action<string> onQuestComplete;

     public void OnQuestComplete(string quest)
     {
         onQuestComplete(quest);
     }
    }
}