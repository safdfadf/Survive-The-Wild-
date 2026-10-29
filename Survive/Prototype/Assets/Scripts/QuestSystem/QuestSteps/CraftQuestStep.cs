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
            SceneRoot.Instance.EventManager.CraftEvents.OnCraft += CheckSubmitResource;
        }

        private void OnDisable()
        {
            SceneRoot.Instance.EventManager.CraftEvents.OnCraft -= CheckSubmitResource;
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