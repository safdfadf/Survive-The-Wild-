using System;

namespace DefaultNamespace.EventBus
{
    public class QuestEvent
    {
     public Action<string> onQuestComplete;

     public void QuestComplete(string quest)
     {
         onQuestComplete(quest);
     }
    }
}