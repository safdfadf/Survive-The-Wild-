using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.EventBus;
using UnityEngine;

namespace DefaultNamespace.QuestSystem.QuestSteps
{
    public class CollectionQuest : QuestStep
    {
        [SerializeField] private List<CraftingSO> requiredData;

        private Dictionary<ObjSo, int> _allRequirements = new();

        private void Awake()
        {
            foreach (var data in requiredData)
            {
                foreach (var ing in data.ingredients)
                {
                    _allRequirements.Add(ing.objSo, ing.amount);
                }
            }
            SetQuestStepName();
        }

        private void OnEnable()
        {
            EventManager.Instance.reseourceEvent.onGatherResource += CheckSubmitResource;
        }

        private void OnDisable()
        {
            EventManager.Instance.reseourceEvent.onGatherResource -= CheckSubmitResource;
        }


        private void CheckSubmitResource(GameObject gm)
        {
            Obj<ObjSo> obj = gm.GetComponent<Obj<ObjSo>>();
            if (obj == null)
                return;
            if (!_allRequirements.ContainsKey(obj.So)) return;

            if (_allRequirements[obj.So] <= 0)
            {
                _allRequirements.Remove(obj.So);
            }
            else
            {
                _allRequirements[obj.So]--;
            }

            CheckToFinish();
        }

        private void CheckToFinish()
        {
            if (_allRequirements.Count <= 0)
                FinishQuest();
        }

        private void SetQuestStepName()
        {
            List<string> names = new();
            foreach (var data in requiredData)
            {
                foreach (var ing in data.ingredients)
                {
                    string n = "Collect "+ing.objSo.itemName + " x " + ing.amount;
                    names.Add(n);
                }
            }

            stepName = string.Join(", ", names.ToArray());
        }
    }
}