using System;
using SurviveTheWild.EventBus.Events;
using UnityEngine;

namespace SurviveTheWild.EventBus
{
    public class EventManager : MonoBehaviour
    {
        public QuestEvent questEvent;
        public InventoryEvents inventoryEvents;
        public PlayerEvents playerEvents;
        public CraftEvents CraftEvents;
        public AnimalEvents AnimalEvents;
        public StructureEvents StructureEvents;
        public UIEvents UiEvents;
        public TimeEvents TimeEvents;
        public MiniGamesEvents MiniGamesEvents;
        public ChunkEvents ChunkEvents;

        private void Awake()
        {
            InitializeEvents();
        }

        private void InitializeEvents()
        {
            questEvent = new QuestEvent();
            inventoryEvents = new InventoryEvents();
            playerEvents = new PlayerEvents();
            CraftEvents = new CraftEvents();
            AnimalEvents = new AnimalEvents();
            StructureEvents = new StructureEvents();
            UiEvents = new UIEvents();
            TimeEvents = new TimeEvents();
            MiniGamesEvents = new MiniGamesEvents();
            ChunkEvents = new ChunkEvents();
        }
    }
}