using System;
using DefaultNamespace.EventBus.Events;
using UnityEngine;

namespace DefaultNamespace.EventBus
{
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance;
        public QuestEvent questEvent;
        public ResourceEvents reseourceEvent;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            questEvent = new QuestEvent();
        }
    }
}