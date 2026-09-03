using System;
using UnityEngine;

namespace DefaultNamespace.QuestSystem
{
    [CreateAssetMenu(fileName = "QuestInfoSo", menuName = "ScriptableObjects/QuestInfoSo", order = 1)]
    public class QuestInfoSo : ScriptableObject
    {
        [field: SerializeField] public string id { get; private set; }
        public string displayName;
        public int PlayerLevelRequired;

        public GameObject[] QuestSteps;

        //public QuestStep[] questStep; // list of quests in a quest 
        //recipe unlocked 
        private void OnValidate()
        {
#if UNITY_EDITOR
            id = this.name;
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}