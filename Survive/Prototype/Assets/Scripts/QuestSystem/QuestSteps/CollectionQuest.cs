using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.EventBus;
using UnityEngine;

namespace DefaultNamespace.QuestSystem.QuestSteps
{
    public class CollectionQuest : QuestStep
    {
        [SerializeField] private List<CollectionData> requirements;

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
            Debug.Log(gm.name + " resource submitted");
            Obj<ObjSo> obj = gm.GetComponent<Obj<ObjSo>>();
            if (obj == null)
            {
                Debug.Log("object is null");
                return;
            }

            foreach (var req in requirements.Where(req => req.so == obj.So))
            {
                req.amount--;
            }

            if (!CheckSubmissionCount())
            {
                Debug.Log("keep it comming");
                return;
            }

            FinishQuest();
        }

        private bool CheckSubmissionCount()
        {
            foreach (var req in requirements)
            {
                if (req.amount > 0)
                    return false;
            }

            return true;
        }
    }
}

[System.Serializable]
public class CollectionData
{
    public ObjSo so;
    public int amount;
}