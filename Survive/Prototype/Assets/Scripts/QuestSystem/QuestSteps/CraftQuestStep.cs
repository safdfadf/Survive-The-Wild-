using System;
using DefaultNamespace.EventBus;
using DefaultNamespace.QuestSystem;
using UnityEngine;

namespace QuestSystem.QuestSteps
{
    public class CraftQuestStep : QuestStep
    {
        // this class will listen every time something is crafted and will see
        [SerializeField] private ObjSo requiredObject;

        private void OnEnable()
        {
            EventManager.Instance.CraftEvents.OnCraft += CheckSubmitResource;
        }

        private void OnDisable()
        {
            EventManager.Instance.CraftEvents.OnCraft -= CheckSubmitResource;
        }

        private void CheckSubmitResource(ObjSo craftedObj)
        {
            if (craftedObj == requiredObject)
            {
                FinishQuest();
            }
        }
    }
}