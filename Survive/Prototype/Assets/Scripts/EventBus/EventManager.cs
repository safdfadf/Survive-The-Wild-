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
        public PlayerEvents playerEvents;
        public CraftEvents CraftEvents;
        public AnimalEvents AnimalEvents;
        public StructureEvents StructureEvents;
        public UIEvents UiEvents;

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

            InitializeEvents();
        }

        private void InitializeEvents()
        {
            questEvent = new QuestEvent();
            reseourceEvent = new ResourceEvents();
            playerEvents = new PlayerEvents();
            CraftEvents = new CraftEvents();
            AnimalEvents = new AnimalEvents();
            StructureEvents = new StructureEvents();
            UiEvents = new UIEvents();
        }
    }
}